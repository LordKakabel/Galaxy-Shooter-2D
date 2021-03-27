using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _speed = 4f;
    [SerializeField] private float _yRange = 7.5f;
    [SerializeField] private float _xRange = 9f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

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

    private void OnTriggerEnter(Collider other) {
        Debug.Log("Hit by " + other.name);

        if (other.tag == "Laser") {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
        else if (other.tag == "Player") {
            /*Player player = other.transform.GetComponent<Player>();
            if (player != null) {
                player.Damage();
            }*/
            other.transform.GetComponent<Player>().Damage();

            Destroy(gameObject);
        }
    }
}
