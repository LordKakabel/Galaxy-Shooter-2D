using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _speed = 3.5f;
    [SerializeField] private float _speedBoostMultiplier = 1.5f;
    [SerializeField] private float _xBoundary = 9f;
    [SerializeField] private float _yTopBoundary = 0f;
    [SerializeField] private float _yBottomBoundary = -3.5f;
    [SerializeField] private Transform _pfProjectile = null;
    [SerializeField] private Transform _pfTripleShotProjectile = null;
    [SerializeField] private Vector3 _projectileOffset = new Vector3(0, 0.75f, 0);
    [SerializeField] private float _fireRate = 0.5f;
    [SerializeField] private int _lives = 3;
    [SerializeField] private float _powerupDuration = 5f;
    
    private float _nextFire = 0f;
    private SpawnManager _spawnManager = null;
    private bool _isTripleShotActive = false;
    private bool _isSpeedBoostActive = false;

    // Start is called before the first frame update
    void Start()
    {
        // Set the current position to new position (0, 0, 0)
        transform.position = Vector3.zero;

        _spawnManager = FindObjectOfType<SpawnManager>();
        if (_spawnManager == null) {
            Debug.LogError(name + ": SpawnManager not found.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _nextFire)
        {
            FireProjectile();
        }
    }

    void CalculateMovement() 
    {
        //! Think of Time.deltaTime as 1 second
        //! transform.Translate(Vector3.right * 5 * Time.deltaTime) will move 5 m per second, independent of framerate

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        float speed = _speed;
        if (_isSpeedBoostActive) {
            speed *= _speedBoostMultiplier;
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
        if (transform.position.x >= _xBoundary) {
            transform.position = new Vector3(
                -_xBoundary,
                transform.position.y,
                transform.position.z);
        }
        else if (transform.position.x <= -_xBoundary) {
            transform.position = new Vector3(
                _xBoundary,
                transform.position.y,
                transform.position.z);
        }
    }

    private void FireProjectile() {
        // Reset the cooldown timer
        _nextFire = Time.time + _fireRate;

        Transform projectile;

        if (_isTripleShotActive) {
            projectile = _pfTripleShotProjectile;
        }
        else {
            projectile = _pfProjectile;
        }

        Instantiate(projectile, transform.position + _projectileOffset, Quaternion.identity);
    }

    public void EnableTripleShot() {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotTimer());
    }

    private IEnumerator TripleShotTimer() {
        yield return new WaitForSeconds(_powerupDuration);
        _isTripleShotActive = false;
    }

    public void EnableSpeedBoost() {
        _isSpeedBoostActive = true;
        StartCoroutine(SpeedBoostTimer());
    }

    private IEnumerator SpeedBoostTimer() {
        yield return new WaitForSeconds(_powerupDuration);
        _isSpeedBoostActive = false;
    }

    public void Damage() {
        _lives--;

        if (_lives <= 0) {
            GameOver();
        }
    }

    private void GameOver() 
    {
        _spawnManager.OnPlayerDeath();
        Debug.Log("Game Over");
        Destroy(gameObject);
    }
}
