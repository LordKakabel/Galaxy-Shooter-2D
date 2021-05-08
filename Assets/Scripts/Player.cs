using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] private float _speed = 3.5f;
    [SerializeField] private float _speedBoostMultiplier = 1.5f;
    [SerializeField] private float _speedDecreaseMultiplier = 0.5f;
    [SerializeField] private float _xBoundary = 9f;
    [SerializeField] private float _yTopBoundary = 0f;
    [SerializeField] private float _yBottomBoundary = -3.5f;
    [SerializeField] private Transform _pfProjectile = null;
    [SerializeField] private Transform _pfTripleShotProjectile = null;
    [SerializeField] private Transform _pfSideShotProjectile = null;
    [SerializeField] private Vector3 _projectileOffset = new Vector3(0, 0.75f, 0);
    [SerializeField] private float _fireRate = 0.5f;
    [SerializeField] private int _maxLives = 3;
    [SerializeField] private float _powerupDuration = 5f;
    [SerializeField] private GameObject _shield = null;
    [SerializeField] private GameObject[] _engineDamage = new GameObject[2];
    [SerializeField] private AudioClip _laserSFX = null;
    [SerializeField] private AudioClip _explosion = null;
    [SerializeField] private float _invincibilityDuration = 0.5f;
    [SerializeField] private GameObject _hullDamage = null;
    [SerializeField] private float _thrusterSpeedMultiplier = 1.5f;
    [SerializeField] private int _maxShieldHealth = 3;
    [SerializeField] private Color[] _shieldColors = new Color[3];
    [SerializeField] private int _maxAmmo = 15;
    [SerializeField] private float _maxThrusterTime = 5f;
    [SerializeField] private float _minThrusterThreshold = 0.33f;
    [Tooltip("The higher this number, the slower the thrusters will recharge. Cannot be 0.")]
    [SerializeField] private float _thrusterRechargeDivisor = 3f;
    [SerializeField] private int _deleteAmmoPowerupAmount = 5;
    [SerializeField] private float _tractorBeamDrawSpeed = 1.5f;
    [SerializeField] private float _maxTractorBeamTime = 3f;
    [Tooltip("The higher this number, the slower the tractor beam will recharge. Cannot be 0.")]
    [SerializeField] private float _tractorBeamRechargeDivisor = 2f;

    private float _nextFire = 0f;
    private GameManager _gameManager;
    private SpawnManager _spawnManager;
    [SerializeField] private bool _isTripleShotActive = false;
    private bool _isSideShotActive = false;
    private bool _isSpeedBoostActive = false;
    private bool _isSpeedDecreaseActive = false;
    private bool _isShieldActive = false;
    private int _score = 0;
    private UIManager _uiManager;
    private bool _isInvincible = false;
    private int _currentShieldHealth;
    private SpriteRenderer _shieldSpriteRenderer;
    private int _currentAmmo;
    private int _currentLives;
    private float _currentThrusterTimeRemaining;
    private bool _areThrustersActive = false;
    private CameraShake _cameraShake;
    private Coroutine _tripleShotCoroutine;
    private Coroutine _sideShotCoroutine;
    private Coroutine _speedBoostCoroutine;
    private float _currentTractorBeamTimeRemaining;

    private void Awake()
    {
        _currentLives = _maxLives;
        _currentAmmo = _maxAmmo;
        _currentThrusterTimeRemaining = _maxThrusterTime;
        _currentTractorBeamTimeRemaining = _maxTractorBeamTime;
    }

    // Start is called before the first frame update
    void Start()
    {
        _shieldSpriteRenderer = _shield.GetComponent<SpriteRenderer>();
        if (!_shieldSpriteRenderer) Debug.LogError(name + ": Cannot find shield object's Sprite Renderer.");

        _gameManager = FindObjectOfType<GameManager>();
        if (_gameManager == null)
        {
            Debug.LogError(name + ": GameManager not found.");
        }

        _spawnManager = FindObjectOfType<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError(name + ": SpawnManager not found.");
        }

        _uiManager = FindObjectOfType<UIManager>();
        if (_uiManager == null) { Debug.LogError(name + ": UI Manager not found."); }

        _uiManager.UpdateScore(_score);
        _uiManager.UpdateAmmo(_currentAmmo, _maxAmmo);
        _uiManager.UpdateLives(_currentLives);
        _uiManager.UpdateThrusterBar(_currentThrusterTimeRemaining / _maxThrusterTime);
        _uiManager.UpdateTractorBeamBar(_currentTractorBeamTimeRemaining / _maxTractorBeamTime);

        _cameraShake = FindObjectOfType<CameraShake>();
        if (!_cameraShake) Debug.LogError(name + ": CameraShake object not found.");
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _nextFire && _currentAmmo > 0)
        {
            FireProjectile();
        }

        TractorBeam();

        _uiManager.UpdateThrusterBar(_currentThrusterTimeRemaining / _maxThrusterTime);
        _uiManager.UpdateTractorBeamBar(_currentTractorBeamTimeRemaining / _maxTractorBeamTime);
    }

    private void TractorBeam()
    {
        if (Input.GetKey(KeyCode.C) && _currentTractorBeamTimeRemaining > 0)
        {
            // Find all power-ups on the screen
            GameObject[] powerups = GameObject.FindGameObjectsWithTag("Power Up");

            // Move the powerups towards player's position
            foreach (var powerup in powerups)
            {
                powerup.transform.Translate(
                    (transform.position - powerup.transform.position).normalized
                    * _tractorBeamDrawSpeed
                    * Time.deltaTime);
            }

            _currentTractorBeamTimeRemaining = Mathf.Max(_currentTractorBeamTimeRemaining - Time.deltaTime, 0);
        }
        else
        {
            _currentTractorBeamTimeRemaining = Mathf.Min(
                _currentTractorBeamTimeRemaining + (Time.deltaTime / _tractorBeamRechargeDivisor),
                _maxTractorBeamTime);
        }
    }

    void CalculateMovement()
    {
        //! Think of Time.deltaTime as 1 second
        //! transform.Translate(Vector3.right * 5 * Time.deltaTime) will move 5 m per second, independent of framerate

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        float speed = _speed;

        if (_isSpeedBoostActive)
        {
            speed *= _speedBoostMultiplier;
        }

        if (_isSpeedDecreaseActive)
        {
            speed *= _speedDecreaseMultiplier;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift)
            && _currentThrusterTimeRemaining / _maxThrusterTime > _minThrusterThreshold)
        {
            _areThrustersActive = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift) || _currentThrusterTimeRemaining <= 0)
        {
            _areThrustersActive = false;
        }

        if (Input.GetKey(KeyCode.LeftShift) && _areThrustersActive)
        {
            speed *= _thrusterSpeedMultiplier;
            _currentThrusterTimeRemaining = Mathf.Max(_currentThrusterTimeRemaining - Time.deltaTime, 0);
        }
        else
        {
            _currentThrusterTimeRemaining = Mathf.Min(
                _currentThrusterTimeRemaining + (Time.deltaTime / _thrusterRechargeDivisor),
                _maxThrusterTime);
        }

        transform.Translate(
            new Vector3(horizontalInput, verticalInput, transform.position.z).normalized
            * speed
            * Time.deltaTime);

        // If the player position is out of bounds, set the position to the boundary
        transform.position = new Vector3(
            transform.position.x,
            Mathf.Clamp(transform.position.y, _yBottomBoundary, _yTopBoundary),
            transform.position.z);

        // If the player reachs an x boundary, wrap around to the opposite boundary
        if (transform.position.x >= _xBoundary)
        {
            transform.position = new Vector3(
                -_xBoundary,
                transform.position.y,
                transform.position.z);
        }
        else if (transform.position.x <= -_xBoundary)
        {
            transform.position = new Vector3(
                _xBoundary,
                transform.position.y,
                transform.position.z);
        }
    }

    private void FireProjectile()
    {
        _currentAmmo--;
        _uiManager.UpdateAmmo(_currentAmmo, _maxAmmo);

        // Reset the cooldown timer
        _nextFire = Time.time + _fireRate;

        Transform projectile;

        if (_isSideShotActive)
        {
            projectile = _pfSideShotProjectile;
        }
        else if (_isTripleShotActive)
        {
            projectile = _pfTripleShotProjectile;
        }
        else
        {
            projectile = _pfProjectile;
        }

        Instantiate(projectile, transform.position + _projectileOffset, Quaternion.identity);

        AudioSource.PlayClipAtPoint(_laserSFX, transform.position);
    }

    public void EnableTripleShot()
    {
        if (_tripleShotCoroutine != null) StopCoroutine(_tripleShotCoroutine);
        _isTripleShotActive = true;
        _tripleShotCoroutine = StartCoroutine(TripleShotTimer());
    }

    public void EnableSideShot()
    {
        if (_sideShotCoroutine != null) StopCoroutine(_sideShotCoroutine);
        _isSideShotActive = true;
        _sideShotCoroutine = StartCoroutine(SideShotTimer());
    }

    private IEnumerator TripleShotTimer()
    {
        yield return new WaitForSeconds(_powerupDuration);
        _isTripleShotActive = false;
    }

    private IEnumerator SideShotTimer()
    {
        yield return new WaitForSeconds(_powerupDuration);
        _isSideShotActive = false;
    }

    public void EnableSpeedBoost()
    {
        if (_speedBoostCoroutine != null) StopCoroutine(_speedBoostCoroutine);
        _isSpeedBoostActive = true;
        StartCoroutine(SpeedBoostTimer());
    }

    private IEnumerator SpeedBoostTimer()
    {
        yield return new WaitForSeconds(_powerupDuration);
        _isSpeedBoostActive = false;
    }

    public void SpeedDecrease()
    {
        _isSpeedDecreaseActive = true;
        StartCoroutine(SpeedDecreaseTimer());
    }

    private IEnumerator SpeedDecreaseTimer()
    {
        yield return new WaitForSeconds(_powerupDuration);
        _isSpeedDecreaseActive = false;
    }

    public void EnableShield()
    {
        _isShieldActive = true;
        _shield.SetActive(true);
        _currentShieldHealth = _maxShieldHealth;
        ShieldColor();
    }

    private void DamageShield()
    {
        _currentShieldHealth--;

        if (_currentShieldHealth > 0)
        {
            ShieldColor();
        }
        else
        {
            _isShieldActive = false;
            _shield.SetActive(false);
        }
    }

    private void ShieldColor()
    {
        _shieldSpriteRenderer.color = _shieldColors[_currentShieldHealth - 1];
    }

    public void AmmoRefill()
    {
        _currentAmmo = _maxAmmo;
        _uiManager.UpdateAmmo(_currentAmmo, _maxAmmo);
    }

    public void DeleteAmmo()
    {
        _currentAmmo = Mathf.Max(_currentAmmo - _deleteAmmoPowerupAmount, 0);
        _uiManager.UpdateAmmo(_currentAmmo, _maxAmmo);
    }

    public void Damage(bool canPenetrateShield)
    {
        _cameraShake.Shake();

        if (_isShieldActive && !canPenetrateShield)
        {
            DamageShield();
            return;
        }

        _currentLives--;
        _uiManager.UpdateLives(_currentLives);

        if (_currentLives == 2)
        {
            _engineDamage[Random.Range(0, _engineDamage.Length)].SetActive(true);
        }
        else if (_currentLives == 1)
        {
            foreach (var engine in _engineDamage)
            {
                engine.SetActive(true);
            }
        }

        StartCoroutine(Invincible());

        if (_currentLives < 1)
        {
            GameOver();
        }
    }

    public void Heal()
    {
        _currentLives = Mathf.Clamp(_currentLives + 1, 0, _maxLives);
        _uiManager.UpdateLives(_currentLives);

        if (_currentLives == 2)
        {
            _engineDamage[Random.Range(0, _engineDamage.Length)].SetActive(false);
        }
        else if (_currentLives == 3)
        {
            foreach (var engine in _engineDamage)
            {
                engine.SetActive(false);
            }
        }
    }

    private void GameOver()
    {
        _spawnManager.OnPlayerDeath();
        _uiManager.DisplayGameOver();
        _gameManager.GameOver();
        AudioSource.PlayClipAtPoint(_explosion, transform.position);
        Destroy(gameObject);
    }

    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy Laser") && !_isInvincible)
        {
            Laser laser = collision.GetComponent<Laser>();
            if (laser)
            {
                Damage(laser.CanPenetrateShield());
                Destroy(collision.transform.parent.gameObject);
            }
        }
    }

    private IEnumerator Invincible()
    {
        _isInvincible = true;
        _hullDamage.SetActive(true);
        yield return new WaitForSeconds(_invincibilityDuration);
        _isInvincible = false;
        _hullDamage.SetActive(false);
    }

    public void DestroyPowerups()
    {
        if (_tripleShotCoroutine != null) StopCoroutine(_tripleShotCoroutine);
        _isTripleShotActive = false;

        if (_sideShotCoroutine != null) StopCoroutine(_sideShotCoroutine);
        _isSideShotActive = false;

        if (_speedBoostCoroutine != null) StopCoroutine(_speedBoostCoroutine);
        _isSpeedBoostActive = false;

        _isShieldActive = false;
        _shield.SetActive(false);
        _currentShieldHealth = 0;
    }
}