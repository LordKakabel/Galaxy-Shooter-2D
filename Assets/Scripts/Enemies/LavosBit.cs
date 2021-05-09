﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavosBit : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _minXPos = 3.75f;
    [SerializeField] private float _maxXPos = 9.5f;
    [Tooltip("The higher this number, the more erratic the bits will move.")]
    [Range(1,10)]
    [SerializeField] private int _timeMultiplier = 2;
    [SerializeField] private float _fadeInDuration = 3f;
    [SerializeField] private SpriteRenderer _spriteRenderer = null;
    [SerializeField] private EnemyFire _enemyFire = null;

    private bool _isActive = false;
    private bool _isHeadingRight = true;
    private float _lastXPos;
    private float _fadeInTimer;

    private void Awake()
    {
        _lastXPos = transform.position.x;
    }

    public void Activate()
    {
        StartCoroutine(OnActivate());
    }

    private IEnumerator OnActivate()
    {
        yield return new WaitForSeconds(_fadeInDuration);
        _isActive = true;
        _enemyFire.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isActive)
        {
            _fadeInTimer += Time.deltaTime;
            Color currentColor = _spriteRenderer.color;
            currentColor.a = _fadeInTimer / _fadeInDuration;
            _spriteRenderer.color = currentColor;
        }
        else
        {
            // If we're at least 1 unit away from our last position check, choose a direction
            if (Mathf.Abs(transform.position.x - _lastXPos) >= 1f) ChooseDirection();

            if (_isHeadingRight) transform.Translate(Vector3.right * _speed * Time.deltaTime);
            else transform.Translate(Vector3.left * _speed * Time.deltaTime);
        }
    }

    private void ChooseDirection()
    {
        int check = (int)(Time.time * _timeMultiplier);
        check %= 2;
        if (check == 0) _isHeadingRight = true;
        else _isHeadingRight = false;

        // Boundary check
        if (transform.position.x >= _maxXPos) _isHeadingRight = false;
        if (transform.position.x <= _minXPos) _isHeadingRight = true;

        _lastXPos = transform.position.x;
    }
}