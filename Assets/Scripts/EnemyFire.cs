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

    private float _nextFire;
    private bool _isFiring = true;

    private void Start()
    {
        _nextFire = Time.time + Random.Range(_minFireCooldown, _maxFireCooldown);
    }
    // Update is called once per frame
    void Update()
    {
        if (Time.time > _nextFire && _isFiring)
        {
            FireProjectile();
        }
    }

    private void FireProjectile()
    {
        Instantiate(_projectilePrefab, transform.position + _projectileOffset, Quaternion.identity);

        _nextFire = Time.time + Random.Range(_minFireCooldown, _maxFireCooldown);

        AudioSource.PlayClipAtPoint(_laserSFX, transform.position);
    }

    public void CeaseFire()
    {
        _isFiring = false;
    }
}
