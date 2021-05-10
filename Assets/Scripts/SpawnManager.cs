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
    [Tooltip("Three is three times as common as one, two is twice as common as one.")]
    public int RarityWeight = 1;
}

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private Transform _enemyContainer = null;
    [SerializeField] private Transform _powerupContainer = null;
    [SerializeField] private float _xSpawnRange = 8f;
    [SerializeField] private float _ySpawnPoint = 7f;
    [SerializeField] private float _zSpawnPoint = 0f;
    [SerializeField] private WaveEntry[] _enemyWaveEntries;
    [SerializeField] private WaveEntry[] _powerupWaveEntries;
    [SerializeField] private int _enemiesInWave1 = 5;
    [SerializeField] private float _minSpawnDelayWave1 = 5f;
    [SerializeField] private float _maxSpawnDelayWave1 = 7f;
    [Tooltip("2 ^ this number is the wave the spawn delays are cut in half. 3 = Wave 8, 4 = Wave 16.")]
    [SerializeField] private int _rootDivisor = 4;
    [SerializeField] private float _waveStartDisplayDelay = 1.5f;
    [SerializeField] private Vector3 _offscreenSpawnPoint = new Vector3(0, 8.5f, 0);
    [SerializeField] private AudioClip _alertSFX = null;
    [SerializeField] private AudioClip _bossMusic = null;
    [SerializeField] private AudioClip _winMusic = null;
    [SerializeField] private int _bossWaveNumber = 10;
    [SerializeField] private GameObject _finalPowerups = null;
    [SerializeField] private GameObject _boss = null;
    [SerializeField] private AudioSource _backgroundMusicPlayer = null;
    [SerializeField] private float _delayBeforeBoss = 6f;
    [SerializeField] private int _enemiesInBossStage = 3;

    private bool _continueSpawning = true;
    private int _waveNumber = 1;
    private List<WaveEntry> _enemyWaveTable;
    private List<WaveEntry> _powerupWaveTable;
    private int _enemiesRemaining;
    private UIManager _uiManager;

    private void Start()
    {
        _uiManager = FindObjectOfType<UIManager>();
        if (_uiManager == null) { Debug.LogError(name + ": UI Manager not found."); }
    }

    public void BeginSpawning()
    {
        StartCoroutine(SpawnPowerupRoutine());
        StartCoroutine(PopulateWave());
    }

    private IEnumerator SpawnPowerupRoutine()
    {
        while (_continueSpawning)
        {
            yield return new WaitForSeconds(Random.Range(
                _minSpawnDelayWave1 / Mathf.Pow(_waveNumber, 1 / _rootDivisor),
                _maxSpawnDelayWave1 / Mathf.Pow(_waveNumber, 1 / _rootDivisor)));

            if (!_continueSpawning) break;

            int index = Random.Range(0, _powerupWaveTable.Count);

            Vector3 spawnPosition = new Vector3(
                Random.Range(-_xSpawnRange, _xSpawnRange),
                _ySpawnPoint,
                _zSpawnPoint);

            Instantiate(_powerupWaveTable[index].GameObjectPrefab,
                spawnPosition,
                _powerupWaveTable[index].GameObjectPrefab.transform.rotation,
                _powerupContainer);
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

        _enemyWaveTable = new List<WaveEntry>();
        foreach (var waveEntry in _enemyWaveEntries)
        {
            if (waveEntry.StartingWave <= _waveNumber)
            {
                for (int i = 0; i < waveEntry.RarityWeight; i++)
                {
                    _enemyWaveTable.Add(waveEntry);
                }
            }
        }

        PopulatePowerupWaveTable();

        _continueSpawning = true;
        StartCoroutine(StartWave());
        StartCoroutine(SpawnPowerupRoutine());
    }

    private void PopulatePowerupWaveTable()
    {
        _powerupWaveTable = new List<WaveEntry>();
        foreach (var waveEntry in _powerupWaveEntries)
        {
            if (waveEntry.StartingWave <= _waveNumber)
            {
                for (int i = 0; i < waveEntry.RarityWeight; i++)
                {
                    _powerupWaveTable.Add(waveEntry);
                }
            }
        }
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

            int index = Random.Range(0, _enemyWaveTable.Count);

            Instantiate(_enemyWaveTable[index].GameObjectPrefab,
                _offscreenSpawnPoint,
                _enemyWaveTable[index].GameObjectPrefab.transform.rotation,
                _enemyContainer);
        }
    }

    public void EnemyDestroyed()
    {
        _enemiesRemaining--;
        if (_enemiesRemaining <= 0) EndWave();
    }

    private void EndWave()
    {
        _continueSpawning = false;
        _waveNumber++;

        if (_waveNumber == _bossWaveNumber) FinalStage();
        else if (_waveNumber == _bossWaveNumber + 1) Win();
        else StartCoroutine(PopulateWave());
    }

    private void FinalStage()
    {
        _finalPowerups.SetActive(true);
        StartCoroutine(Alert());
    }

    private IEnumerator Alert()
    {
        yield return new WaitForSeconds(_delayBeforeBoss);

        _backgroundMusicPlayer.clip = _alertSFX;
        _backgroundMusicPlayer.loop = false;
        _backgroundMusicPlayer.Play();

        StartCoroutine(BossMusic());
    }

    private IEnumerator BossMusic()
    {
        yield return new WaitForSeconds(_alertSFX.length);

        _backgroundMusicPlayer.clip = _bossMusic;
        _backgroundMusicPlayer.loop = true;
        _backgroundMusicPlayer.Play();

        PopulatePowerupWaveTable();
        _continueSpawning = true;
        StartCoroutine(SpawnPowerupRoutine());

        _enemiesRemaining = _enemiesInBossStage;

        _boss.SetActive(true);
    }

    private void Win()
    {
        _backgroundMusicPlayer.clip = _winMusic;
        _backgroundMusicPlayer.loop = false;
        _backgroundMusicPlayer.Play();

        _uiManager.Win();
    }
}
