using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    private enum Powerup { TripleShot, Speed, Shields };

    [SerializeField] private float _speed = 3f;
    [SerializeField] private float _yBoundary = -8f;
    [SerializeField] private Powerup _powerupID = Powerup.TripleShot;
    [SerializeField] private AudioClip _pickupSFX = null;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < _yBoundary)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.transform.GetComponent<Player>();
            if (player != null)
            {
                switch (_powerupID)
                {
                    case Powerup.TripleShot:
                        player.EnableTripleShot();
                        break;
                    case Powerup.Speed:
                        player.EnableSpeedBoost();
                        break;
                    case Powerup.Shields:
                        player.EnableShield();
                        break;
                    default:
                        break;
                }
            }

            AudioSource.PlayClipAtPoint(_pickupSFX, Camera.main.transform.position);

            Destroy(gameObject);
        }
    }
}
