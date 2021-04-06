using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText = null;
    [SerializeField] private Image _livesImage = null;
    [SerializeField] private Sprite[] _livesSprites = null;
    [SerializeField] private GameObject _gameOverDisplay = null;
    [SerializeField] private GameObject _restartDisplay = null;
    [SerializeField] private float _flickerDelay = 0.5f;
    [Tooltip("Should be an odd number.")]
    [SerializeField] private int _timesToFlicker = 51;

    // Start is called before the first frame update
    void Start()
    {
        UpdateScore(0);
    }

    public void UpdateScore(int score)
    {
        _scoreText.text = "Score: " + score;
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
}
