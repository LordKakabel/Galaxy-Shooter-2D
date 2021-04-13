using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class WaveEntry
{
    public GameObject GameObjectPrefab = null;
    public int StartingWave = 1;
    [Tooltip("Three is three times as rare as one, two is twice as rare as one.")]
    public int RarityWeight = 1;
}

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private Transform _enemyContainer = null;
    [SerializeField] private Transform _enemyPrefab = null;
    [SerializeField] private float _xSpawnRange = 8f;
    [SerializeField] private float _ySpawnPoint = 7f;
    [SerializeField] private float _zSpawnPoint = 0f;
    [SerializeField] private float _spawnDelay = 5f;
    [SerializeField] private Transform[] _powerupPrefabs = null;
    [SerializeField] private int _powerupSpawnDelayMin = 3;
    [SerializeField] private int _powerupSpawnDelayMax = 7;
    [SerializeField] private float _enemyBeginSpawningDelay = 2f;
    [SerializeField] private float _powerupBeginSpawningDelay = 5f;
    [SerializeField] private WaveEntry[] _waveEntries;
    [SerializeField] private int _enemiesInWave1 = 5;
    [SerializeField] private float _minSpawnDelayWave1 = 5f;
    [SerializeField] private float _maxSpawnDelayWave1 = 7f;
    [Tooltip("2 ^ this number is the wave the spawn delays are cut in half. 3 = Wave 8, 4 = Wave 16.")]
    [SerializeField] private int _rootDivisor = 4;
    [SerializeField] private float _waveStartDisplayDelay = 1.5f;
    [SerializeField] private Vector3 _offscreenSpawnPoint = new Vector3(0, 8.5f, 0);

    private bool _continueSpawning = true;
    private int _waveNumber = 1;
    private List<WaveEntry> _waveTable;
    private int _enemiesRemaining;
    private UIManager _uiManager;

    private void Start()
    {
        _uiManager = FindObjectOfType<UIManager>();
        if (_uiManager == null) { Debug.LogError(name + ": UI Manager not found."); }
    }

    public void BeginSpawning()
    {
        //StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
        StartCoroutine(PopulateWave());
    }

    /*private IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(_enemyBeginSpawningDelay);

        while (_continueSpawning)
        {
            Vector3 spawnPosition = new Vector3(Random.Range(-_xSpawnRange, _xSpawnRange), _ySpawnPoint, _zSpawnPoint);
            Instantiate(_enemyPrefab, spawnPosition, Quaternion.identity, _enemyContainer);
            yield return new WaitForSeconds(_spawnDelay);
        }
    }*/

    private IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(_powerupBeginSpawningDelay);

        while (_continueSpawning)
        {
            Vector3 spawnPosition = new Vector3(Random.Range(-_xSpawnRange, _xSpawnRange), _ySpawnPoint, _zSpawnPoint);
            int powerupIndex = Random.Range(0, _powerupPrefabs.Length);
            Instantiate(_powerupPrefabs[powerupIndex], spawnPosition, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(_powerupSpawnDelayMin, _powerupSpawnDelayMax + 1));
        }
    }

    public void OnPlayerDeath()
    {
        _continueSpawning = false;
        Destroy(_enemyContainer.gameObject);
    }

    private IEnumerator PopulateWave()
    {
        // Display new wave number
        _uiManager.DisplayWave(_waveNumber);
        yield return new WaitForSeconds(_waveStartDisplayDelay);
        // Turn off wave display
        _uiManager.HideWaveDisplay();

        _waveTable = new List<WaveEntry>();
        foreach (var waveEntry in _waveEntries)
        {
            if (waveEntry.StartingWave <= _waveNumber)
            {
                for (int i = 0; i < waveEntry.RarityWeight; i++)
                {
                    _waveTable.Add(waveEntry);
                }
            }
        }

        _continueSpawning = true;
        StartCoroutine(StartWave());
        StartCoroutine(SpawnPowerupRoutine());
    }

    private IEnumerator StartWave()
    {
        _enemiesRemaining = _enemiesInWave1 + _waveNumber - 1;

        for (int i = 0; i < _enemiesInWave1 + _waveNumber - 1; i++)
        {
            yield return new WaitForSeconds(Random.Range(
                _minSpawnDelayWave1 / Mathf.Pow(_waveNumber, 1 / _rootDivisor),
                _maxSpawnDelayWave1 / Mathf.Pow(_waveNumber, 1 / _rootDivisor)));

            if (!_continueSpawning) break;

            int index = Random.Range(0, _waveTable.Count);

            Instantiate(_waveTable[index].GameObjectPrefab,
                _offscreenSpawnPoint,
                _waveTable[index].GameObjectPrefab.transform.rotation,
                _enemyContainer);
        }
    }

    public void EnemyDestroyed()
    {
        _enemiesRemaining--;
        if (_enemiesRemaining <= 0) EndWave();
    }

    //! Needs to be public so boss can call upon death once it is implimented
    public void EndWave()
    {
        _continueSpawning = false;
        _waveNumber++;
        StartCoroutine(PopulateWave());
    }
}
