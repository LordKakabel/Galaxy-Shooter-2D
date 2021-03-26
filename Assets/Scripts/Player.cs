using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _speed = 3.5f;
    [SerializeField] private float _xBoundary = 9f;
    [SerializeField] private float _yTopBoundary = 0f;
    [SerializeField] private float _yBottomBoundary = -3.5f;

    // Start is called before the first frame update
    void Start()
    {
        // Set the current position to new position (0, 0, 0)
        transform.position = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
    }

    void CalculateMovement() 
    {
        // Time.deltaTime = 1 second
        // transform.Translate(Vector3.right * 5 * Time.deltaTime) will move 5 m per second, independent of framerate

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
}
