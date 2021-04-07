using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _speed = 4f;
    [SerializeField] private float _yRange = 7.5f;
    [SerializeField] private float _xRange = 9f;
    [SerializeField] private int _scoreValue = 10;
    [SerializeField] private AnimationClip _deathAnimation = null;
    [SerializeField] private AudioClip _explosionSFX = null;

    private Player _player;
    private Animator _animator;
    private Collider2D _collider;

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
    }

    // Update is called once per frame
    void Update()
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
            collision.transform.GetComponent<Player>().Damage();
            StartCoroutine(DestroySelf());
        }
    }

    private IEnumerator DestroySelf()
    {
        _animator.SetTrigger("OnEnemyDeath");
        _speed = 0;
        _collider.enabled = false;
        if (GetComponent<EnemyFire>()) { GetComponent<EnemyFire>().CeaseFire(); }
        AudioSource.PlayClipAtPoint(_explosionSFX, transform.position);
        yield return new WaitForSeconds(_deathAnimation.length);
        Destroy(gameObject);
    }
}
