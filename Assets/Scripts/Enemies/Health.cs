using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 5;
    [SerializeField] private int _scoreValue = 1000;
    [SerializeField] private SpriteRenderer _spriteRenderer = null;

    private int _currentHealth;
    private Player _player;
    

    private void Awake()
    {
        _currentHealth = _maxHealth;
    }
    // Start is called before the first frame update
    void Start()
    {
        _player = FindObjectOfType<Player>();
        if (_player == null) { Debug.LogError(name + ": Player not found."); }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Laser"))
        {
            Damage();
            Destroy(collision.gameObject);
        }
    }

    private void Damage()
    {
        _currentHealth--;

        float healthPercentage = (_currentHealth - 1) / (_maxHealth - 1);
        _spriteRenderer.color = new Color(1f, healthPercentage, healthPercentage);

        if (_currentHealth < 0)
        {
            Death();
        }
    }

    private void Death()
    {
        _player.AddScore(_scoreValue);
        //? StartCoroutine(DestroySelf());
    }
}
