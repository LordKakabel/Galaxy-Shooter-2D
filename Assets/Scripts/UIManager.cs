using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText = null;
    [SerializeField] private TextMeshProUGUI _ammoText = null;
    [SerializeField] private TextMeshProUGUI _waveText = null;
    [SerializeField] private Color _noAmmoTextColor = Color.red;
    [SerializeField] private Image _livesImage = null;
    [SerializeField] private Sprite[] _livesSprites = null;
    [SerializeField] private GameObject _gameOverDisplay = null;
    [SerializeField] private GameObject _restartDisplay = null;
    [SerializeField] private float _flickerDelay = 0.5f;
    [Tooltip("Should be an odd number.")]
    [SerializeField] private int _timesToFlicker = 51;
    [SerializeField] private RectTransform _thrustRemainingRectTransform = null;
    [SerializeField] private Image _thrustRemainingImage = null;
    [SerializeField] private Color[] _thrustColors = new Color[3];
    [SerializeField] private Image _tractorBeamRemainingImage = null;
    
    private Color _originalAmmoTextColor;
    private Vector2 _thrustRemainingOriginalSize;
    
    void Awake()
    {
        _originalAmmoTextColor = _ammoText.color;
        _thrustRemainingOriginalSize = _thrustRemainingRectTransform.sizeDelta;
    }

    public void UpdateScore(int score)
    {
        _scoreText.text = "Score: " + score;
    }

    public void UpdateAmmo(int ammo, int maxAmmo)
    {
        if (ammo == 0)
        {
            _ammoText.color = _noAmmoTextColor;
        }
        else
        {
            _ammoText.color = _originalAmmoTextColor;
        }

        _ammoText.text = "Ammo: " + ammo + " / " + maxAmmo;
    }

    public void UpdateLives(int lives)
    {
        _livesImage.sprite = _livesSprites[lives];
    }

    public void DisplayGameOver()
    {
        _restartDisplay.SetActive(true);
        StartCoroutine(GameOverFlicker());
    }

    private IEnumerator GameOverFlicker()
    {
        for (int i = 0; i < _timesToFlicker; i++)
        {
            _gameOverDisplay.SetActive(!_gameOverDisplay.activeSelf);
            yield return new WaitForSeconds(_flickerDelay);
        }
    }

    public void UpdateThrusterBar(float percentFull)
    {
        _thrustRemainingRectTransform.sizeDelta = new Vector2(
            percentFull * _thrustRemainingOriginalSize.x,
            _thrustRemainingOriginalSize.y);
        
        _thrustRemainingImage.color = _thrustColors[(int)((percentFull * 3) - 0.1f)];
    }

    public void UpdateTractorBeamBar(float percentFull)
    {
        _tractorBeamRemainingImage.fillAmount = percentFull;

        _tractorBeamRemainingImage.color = new Color(0, 0, percentFull);
    }

    public void DisplayWave(int waveNumber)
    {
        _waveText.text = "Wave " + waveNumber;
    }

    public void HideWaveDisplay()
    {
        _waveText.text = "";
    }
}
