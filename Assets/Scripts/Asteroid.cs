using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [Tooltip("A positive value is counterclockwise.")]
    [SerializeField] private float _rotationSpeed = -10f;
    [SerializeField] private GameObject _explosionPrefab = null;
    [SerializeField] private float _explosionTime = 3f;
    [SerializeField] private float _explosionDelay = 0.25f;
    [SerializeField] private AudioClip _explosionSFX = null;

    private SpawnManager _spawnManager;

    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = FindObjectOfType<SpawnManager>();
        if (_spawnManager == null) { Debug.Log(name + ": Cannot find SpawnManager."); }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.forward * _rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Laser"))
        {
            Destroy(collision.gameObject);
            DestroySelf();
        }
        else if (collision.CompareTag("Player"))
        {
            collision.transform.GetComponent<Player>().Damage(false);
            DestroySelf();
        }
    }

    private void DestroySelf()
    {
        _spawnManager.BeginSpawning();
        GameObject explosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        AudioSource.PlayClipAtPoint(_explosionSFX, transform.position);
        Destroy(explosion, _explosionTime);
        Destroy(gameObject, _explosionDelay);
    }
}
