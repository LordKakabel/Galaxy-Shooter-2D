using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beholder : MonoBehaviour
{
    [SerializeField] private float _speed = 1f;
    [SerializeField] private EnemyFire[] _enemyFires = null;
    [SerializeField] private LavosBit[] _lavosBits = null;
    [SerializeField] private NullBeam _nullBeam = null;
    [SerializeField] private float _nullBeamDuration = 9f;
    [SerializeField] private float _finalYPosition = 2.5f;

    private bool _isInPosition = false;
    
    private void AllSystemsActive()
    {
        ActivateBits();
        _nullBeam.gameObject.SetActive(true);
        _nullBeam.ShootBeam(_nullBeamDuration);
        EnableFiring();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isInPosition)
        {
            transform.Translate(Vector3.down * _speed * Time.deltaTime);

            if (transform.position.y <= _finalYPosition) _isInPosition = true;

            if (_isInPosition) AllSystemsActive();
        }
    }

    private void EnableFiring()
    {
        foreach (var item in _enemyFires)
        {
            item.enabled = true;
        }
    }

    private void ActivateBits()
    {
        foreach (var bit in _lavosBits)
        {
            bit.gameObject.SetActive(true);
            bit.Activate();
        }
    }
}
