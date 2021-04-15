using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected float _speed = 4f;
    [SerializeField] private float _yRange = 7.5f;
    [SerializeField] protected float _xRange = 9f;
    [SerializeField] private int _scoreValue = 10;
    [SerializeField] private AnimationClip _deathAnimation = null;
    [SerializeField] private AudioClip _explosionSFX = null;
    [SerializeField] private float _xSpawnRange = 8f;
    [SerializeField] private float _ySpawnPoint = 7f;
    [SerializeField] private float _zSpawnPoint = 0f;
    [SerializeField] private bool _canPenetrateShield = false;

    private Player _player;
    private Animator _animator;
    private Collider2D _collider;
    private SpawnManager _spawnManager;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        if (_animator == null) { Debug.LogError(name + ": Animator not found."); }

        _collider = GetComponent<Collider2D>();
        if (_collider == null) { Debug.LogError(name + ": Collider2D not found."); }
    }

    private void Start()
    {
        _player = FindObjectOfType<Player>();
        if (_player == null) { Debug.LogError(name + ": Player not found."); }

        _spawnManager = FindObjectOfType<SpawnManager>();
        if (_spawnManager == null) { Debug.LogError(name + ": SpawnManager not found."); }

        SpawnLocation();
    }

    protected virtual void SpawnLocation()
    {
        transform.position = new Vector3(Random.Range(-_xSpawnRange, _xSpawnRange), _ySpawnPoint, _zSpawnPoint);
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
    }

    protected virtual void CalculateMovement()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        // Respawn at top of screen with a random x position
        if (transform.position.y < -_yRange)
        {
            transform.position = new Vector3(
                Random.Range(-_xRange, _xRange),
                _yRange,
                transform.position.z);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Laser"))
        {
            _player.AddScore(_scoreValue);
            Destroy(collision.gameObject);
            StartCoroutine(DestroySelf());
        }
        else if (collision.CompareTag("Player"))
        {
            collision.transform.GetComponent<Player>().Damage(_canPenetrateShield);
            StartCoroutine(DestroySelf());
        }
    }

    protected virtual IEnumerator DestroySelf()
    {
        _animator.SetTrigger("OnEnemyDeath");
        _speed = 0;
        _collider.enabled = false;
        if (GetComponent<EnemyFire>()) { GetComponent<EnemyFire>().CeaseFire(); }
        AudioSource.PlayClipAtPoint(_explosionSFX, transform.position);
        _spawnManager.EnemyDestroyed();
        yield return new WaitForSeconds(_deathAnimation.length);
        Destroy(gameObject);
    }
}
