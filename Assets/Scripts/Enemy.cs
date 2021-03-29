using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _speed = 4f;
    [SerializeField] private float _yRange = 7.5f;
    [SerializeField] private float _xRange = 9f;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        // Respawn at top of screen with a random x position
        if (transform.position.y < -_yRange)
        {
            transform.position = new Vector3(
                Random.Range(-_xRange, _xRange),
                _yRange,
                transform.position.z);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Laser")) {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Player")) {
            collision.transform.GetComponent<Player>().Damage();
            Destroy(gameObject);
        }
    }
}
