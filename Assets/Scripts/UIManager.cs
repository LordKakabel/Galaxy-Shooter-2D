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
    [SerializeField] private GameObject _congratzText = null;
    [SerializeField] private GameObject _winText = null;

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
        StartCoroutine(Flicker(_gameOverDisplay));
    }

    public void Win()
    {
        _winText.SetActive(true);
        _restartDisplay.SetActive(true);
        StartCoroutine(Flicker(_congratzText));

        FindObjectOfType<GameManager>().GameOver();
    }

    private IEnumerator Flicker(GameObject objectToFlicker)
    {
        for (int i = 0; i < _timesToFlicker; i++)
        {
            objectToFlicker.SetActive(!objectToFlicker.activeSelf);
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

        // Set color from fill percent = 0% at 50% blue to fill percent 100% = 100% blue
        _tractorBeamRemainingImage.color = new Color(0, 0, (percentFull / 2) + 0.5f);
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
