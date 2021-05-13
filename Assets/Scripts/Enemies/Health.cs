using System;
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

        float healthPercentage = (_currentHealth - 1f) / (_maxHealth - 1f);
        _spriteRenderer.color = new Color(1f, healthPercentage, healthPercentage);

        if (_currentHealth <= 0)
        {
            _spriteRenderer.color = Color.white;
            Death();
        }
    }

    /* Do the math!
     *    Col A                Col B
     * Max 3 hits:
     * 2 / 3 = 66%          1 / 2 = 50%
     * 1 / 3 = 33%          0 / 2 = 0%
     * 
     * Max 5 hits:
     * 4 / 5 = 80%          3 / 4 = 75%
     * 3 / 5 = 60%          2 / 4 = 50%
     * 2 / 5 = 40%          1 / 4 = 25%
     * 1 / 5 = 20%          0 / 4 = 0%
     * 
     */

    private void Death()
    {
        _player.AddScore(_scoreValue);

        GetComponent<Enemy>().DestroySelf();
    }
}
