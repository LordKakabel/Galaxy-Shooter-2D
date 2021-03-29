using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private Transform _enemyContainer = null;
    [SerializeField] private Transform _enemyPrefab = null;
    [SerializeField] private float _xSpawnRange = 8f;
    [SerializeField] private float _ySpawnPoint = 7f;
    [SerializeField] private float _zSpawnPoint = 0f;
    [SerializeField] private float _spawnDelay = 5f;
    [SerializeField] private Transform _tripleShotPowerupPrefab = null;
    [SerializeField] private int _powerupSpawnDelayMin = 3;
    [SerializeField] private int _powerupSpawnDelayMax = 7;

    private bool _continueSpawning = true;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }

    private IEnumerator SpawnEnemyRoutine() {
        while (_continueSpawning) {
            Vector3 spawnPosition = new Vector3(Random.Range(-_xSpawnRange, _xSpawnRange), _ySpawnPoint, _zSpawnPoint);
            Instantiate(_enemyPrefab, spawnPosition, Quaternion.identity, _enemyContainer);
            yield return new WaitForSeconds(_spawnDelay);
        }
    }

    private IEnumerator SpawnPowerupRoutine() {
        while (_continueSpawning) {
            Vector3 spawnPosition = new Vector3(Random.Range(-_xSpawnRange, _xSpawnRange), _ySpawnPoint, _zSpawnPoint);
            Instantiate(_tripleShotPowerupPrefab, spawnPosition, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(_powerupSpawnDelayMin, _powerupSpawnDelayMax + 1));
        }
    }

    public void OnPlayerDeath() {
        _continueSpawning = false;
        Destroy(_enemyContainer.gameObject);
    }
}
