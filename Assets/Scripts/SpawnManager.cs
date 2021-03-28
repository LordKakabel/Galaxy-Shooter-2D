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

    private bool _continueSpawning = true;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator SpawnRoutine() {
        while (_continueSpawning) {
            Vector3 spawnPosition = new Vector3(Random.Range(-_xSpawnRange, _xSpawnRange), _ySpawnPoint, _zSpawnPoint);
            Instantiate(_enemyPrefab, spawnPosition, Quaternion.identity, _enemyContainer);
            yield return new WaitForSeconds(_spawnDelay);
        }
    }

    public void OnPlayerDeath() {
        _continueSpawning = false;
        Destroy(_enemyContainer.gameObject);
    }
}
