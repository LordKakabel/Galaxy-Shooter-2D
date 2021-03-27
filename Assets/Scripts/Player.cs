using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _speed = 3.5f;
    [SerializeField] private float _xBoundary = 9f;
    [SerializeField] private float _yTopBoundary = 0f;
    [SerializeField] private float _yBottomBoundary = -3.5f;
    [SerializeField] private Transform _pfProjectile = null;
    [SerializeField] private Vector3 _projectileOffset = new Vector3(0, 0.75f, 0);
    [SerializeField] private float _fireRate = 0.5f;
    [SerializeField] private int _lives = 3;

    private float _nextFire = 0f;

    // Start is called before the first frame update
    void Start()
    {
        // Set the current position to new position (0, 0, 0)
        transform.position = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _nextFire)
        {
            FireProjectile();
        }
    }

    void CalculateMovement() 
    {
        //! Think of Time.deltaTime as 1 second
        //! transform.Translate(Vector3.right * 5 * Time.deltaTime) will move 5 m per second, independent of framerate

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        transform.Translate(
            new Vector3(horizontalInput, verticalInput, transform.position.z).normalized
            * _speed
            * Time.deltaTime);

        // If the player position is out of bounds, set the position to the boundary
        transform.position = new Vector3(
            transform.position.x,
            Mathf.Clamp(transform.position.y, _yBottomBoundary, _yTopBoundary),
            transform.position.z);

        // If the player reachs an x boundary, wrap around to the opposite boundary
        if (transform.position.x >= _xBoundary) {
            transform.position = new Vector3(
                -_xBoundary,
                transform.position.y,
                transform.position.z);
        }
        else if (transform.position.x <= -_xBoundary) {
            transform.position = new Vector3(
                _xBoundary,
                transform.position.y,
                transform.position.z);
        }
    }

    private void FireProjectile() {
        // Reset the cooldown timer
        _nextFire = Time.time + _fireRate;

        Instantiate(_pfProjectile, transform.position + _projectileOffset, Quaternion.identity);
    }

    public void Damage() {
        _lives--;

        if (_lives <= 0) {
            GameOver();
        }
    }

    private void GameOver() 
    {
        Destroy(gameObject);
        Debug.Log("Game Over");
    }
}
