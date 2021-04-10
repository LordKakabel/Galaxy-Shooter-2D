﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] private float _speed = 3.5f;
    [SerializeField] private float _speedBoostMultiplier = 1.5f;
    [SerializeField] private float _xBoundary = 9f;
    [SerializeField] private float _yTopBoundary = 0f;
    [SerializeField] private float _yBottomBoundary = -3.5f;
    [SerializeField] private Transform _pfProjectile = null;
    [SerializeField] private Transform _pfTripleShotProjectile = null;
    [SerializeField] private Vector3 _projectileOffset = new Vector3 (0, 0.75f, 0);
    [SerializeField] private float _fireRate = 0.5f;
    [SerializeField] private int _lives = 3;
    [SerializeField] private float _powerupDuration = 5f;
    [SerializeField] private GameObject _shield = null;
    [SerializeField] private GameObject[] _engineDamage = new GameObject[2];
    [SerializeField] private AudioClip _laserSFX = null;
    [SerializeField] private AudioClip _explosion = null;
    [SerializeField] private float _invincibilityDuration = 0.5f;
    [SerializeField] private GameObject _hullDamage = null;
    [SerializeField] private float _thrusterSpeedMultiplier = 1.5f;

    private float _nextFire = 0f;
    private GameManager _gameManager;
    private SpawnManager _spawnManager;
    private bool _isTripleShotActive = false;
    private bool _isSpeedBoostActive = false;
    private bool _isShieldActive = false;
    private int _score = 0;
    private UIManager _uiManager;
    private bool _isInvincible = false;

    // Start is called before the first frame update
    void Start()
    {
        // Set the current position to new position (0, 0, 0)
        transform.position = Vector3.zero;

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

        _uiManager.UpdateLives(_lives);
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
        if (_isSpeedBoostActive)
        {
            speed *= _speedBoostMultiplier;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed *= _thrusterSpeedMultiplier;
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
        // Reset the cooldown timer
        _nextFire = Time.time + _fireRate;

        Transform projectile;

        if (_isTripleShotActive)
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
        _isTripleShotActive = true;
        StartCoroutine(TripleShotTimer());
    }

    private IEnumerator TripleShotTimer()
    {
        yield return new WaitForSeconds(_powerupDuration);
        _isTripleShotActive = false;
    }

    public void EnableSpeedBoost()
    {
        _isSpeedBoostActive = true;
        StartCoroutine(SpeedBoostTimer());
    }

    private IEnumerator SpeedBoostTimer()
    {
        yield return new WaitForSeconds(_powerupDuration);
        _isSpeedBoostActive = false;
    }

    public void EnableShield()
    {
        _isShieldActive = true;
        _shield.SetActive(true);
    }

    private void DestroyShield()
    {
        _isShieldActive = false;
        _shield.SetActive(false);
    }

    public void Damage()
    {

        if (_isShieldActive)
        {
            DestroyShield();
            return;
        }

        _lives--;
        _uiManager.UpdateLives(_lives);

        if (_lives == 2)
        {
            _engineDamage[Random.Range(0, _engineDamage.Length)].SetActive(true);
        }
        else if (_lives == 1)
        {
            foreach (var engine in _engineDamage)
            {
                engine.SetActive(true);
            }
        }

        StartCoroutine(Invincible());

        if (_lives <= 0)
        {
            GameOver();
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
            Damage();
            Destroy(collision.transform.parent.gameObject);
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
}
