using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFire : MonoBehaviour
{
    [SerializeField] private GameObject _projectilePrefab = null;
    [SerializeField] private Vector3 _projectileOffset = new Vector3(0, 0.5f, 0);
    [SerializeField] private AudioClip _laserSFX = null;
    [SerializeField] private float _minFireCooldown = 3f;
    [SerializeField] private float _maxFireCooldown = 7f;
    [SerializeField] private float _rearViewDetectionWidth = 0.5f;
    [SerializeField] private bool _canFireToRear = false;
    [SerializeField] private Vector3 _rearProjectileOffset = new Vector3(0, 0.5f, 0);

    private float _nextFire;
    private bool _isFiring = true;
    private Player _player;

    private void Start()
    {
        _player = FindObjectOfType<Player>();
        if (_player == null) { Debug.LogError(name + ": Player not found."); }

        _nextFire = Time.time + Random.Range(_minFireCooldown, _maxFireCooldown);
    }
    // Update is called once per frame
    void Update()
    {
        if (Time.time > _nextFire && _isFiring)
        {
            if (_canFireToRear && CheckRearView())
            {
                FireRearProjectile();
            }
            else FireProjectile();
        }
    }

    private void FireProjectile()
    {
        Instantiate(_projectilePrefab, transform.position + _projectileOffset, Quaternion.identity);

        _nextFire = Time.time + Random.Range(_minFireCooldown, _maxFireCooldown);

        AudioSource.PlayClipAtPoint(_laserSFX, transform.position);
    }

    private void FireRearProjectile()
    {
        Instantiate(_projectilePrefab, transform.position + _rearProjectileOffset, Quaternion.Euler(0, 0, 180f));

        _nextFire = Time.time + Random.Range(_minFireCooldown, _maxFireCooldown);

        AudioSource.PlayClipAtPoint(_laserSFX, transform.position);
    }

    public void CeaseFire()
    {
        _isFiring = false;
    }

    private bool CheckRearView()
    {
        // If our Y < the Player's Y,
        if (transform.position.y < _player.transform.position.y)
        {
            // If the Player's X is within our detection width,
            if (Mathf.Abs(transform.position.x - _player.transform.position.x) <= _rearViewDetectionWidth)
            {
                return true;
            }
        }

        return false;
    }
}
