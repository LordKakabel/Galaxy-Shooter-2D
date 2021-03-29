using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] private float _speed = 8f;
    [SerializeField] private float _yBoundary = 8f;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);

        if (transform.position.y > _yBoundary)
        {
            if (transform.parent != null) {
                Destroy(transform.parent.gameObject);
            }

            Destroy(gameObject);
        }
    }
}
