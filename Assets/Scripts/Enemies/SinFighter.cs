using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinFighter : Enemy
{
    [SerializeField] private Vector3 _startPosition = Vector3.zero;
    [SerializeField] private float _frequency = 20f;
    [Tooltip("Half of the world units of up/down range.")]
    [SerializeField] private float _magnitude = 0.5f;

    private Vector3 _axis;
    private Vector3 _position;
    private bool _shouldFlip = true;
    private bool _isAlive = true;

    protected override void SpawnLocation()
    {
        _position = _startPosition;
        _axis = transform.right;
    }

    protected override void CalculateMovement()
    {
        if (_isAlive)
        {
            _position -= transform.up * _speed * Time.deltaTime;
            transform.position = _position + _axis * Mathf.Sin(Time.time * _frequency) * _magnitude;

            // Turn around shortly after leaving screen
            if (Mathf.Abs(transform.position.x) > _xRange && _shouldFlip)
            {
                transform.Rotate(180, 0, 0);
                _shouldFlip = false;
            }

            if (Mathf.Abs(transform.position.x) < _xRange && !_shouldFlip)
            {
                _shouldFlip = true;
            }
        }
    }

    public override void DestroySelf()
    {
        _isAlive = false;
        base.DestroySelf();
    }
}
