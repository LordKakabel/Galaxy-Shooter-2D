using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 5f;
    [SerializeField] private float _scalingRange = 0.2f;
    [SerializeField] private float _scalingSpeed = 4f;

    private Vector3 _originalXScale;

    private void Awake()
    {
        _originalXScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.back * _rotationSpeed * Time.deltaTime);

        transform.localScale = new Vector3(
            _originalXScale.x + (Mathf.Sin(Time.time * _scalingSpeed) * _scalingRange),
            _originalXScale.y,
            _originalXScale.z);
    }
}
