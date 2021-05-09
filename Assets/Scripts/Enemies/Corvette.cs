using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corvette : Enemy
{
    [SerializeField] private Vector2 _sensorBoxSize = new Vector2(1f, 32f);
    [SerializeField] private LayerMask _playerLaserLayerMask = 0;

    protected override void CalculateMovement()
    {
        DetectIncomingFire();
        base.CalculateMovement();
    }

    private void DetectIncomingFire()
    {
        Collider2D collider = Physics2D.OverlapBox(
                transform.position,
                _sensorBoxSize,
                0f,
                _playerLaserLayerMask);

        if (collider)
        {
            Vector3 direction = Vector3.right;
            if (transform.position.x > 0) direction = Vector3.left;
            transform.Translate(direction * _speed * Time.deltaTime);
        }
    }
}
