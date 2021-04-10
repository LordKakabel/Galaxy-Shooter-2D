using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    // Special thanks to fvts https://gist.github.com/ftvs/5822103

    [Tooltip("Will get the transform of the object this script is attached to if not assigned.")]
    [SerializeField] private Transform _camera;
    [SerializeField] private float _maxShakeDuration = 0.2f;
    [SerializeField] private float _shakeAmount = 0.7f;
    [SerializeField] private float _decreaseFactor = 1f;

    private float _shakeDuration = 0;
    private Vector3 _originalPosition;

    private void Awake()
    {
        if (!_camera) _camera = GetComponent(typeof(Transform)) as Transform;
    }

    private void OnEnable()
    {
        _originalPosition = _camera.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (_shakeDuration > 0)
        {
            _camera.localPosition = _originalPosition + Random.insideUnitSphere * _shakeAmount;
            _shakeDuration -= Time.deltaTime * _decreaseFactor;
        }
        else
        {
            _camera.localPosition = _originalPosition;
        }
    }

    public void Shake()
    {
        _shakeDuration = _maxShakeDuration;
    }
}
