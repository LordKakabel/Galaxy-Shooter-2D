using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulkCruiser : Enemy
{
    [SerializeField] private LayerMask _layerMask = 0;
    [SerializeField] private float _detectionRadius = 3f;
    [SerializeField] private float _rammingSpeed = 7f;
    [SerializeField] private float _xBoundary = 15f;
    [SerializeField] private Transform _directionProbe = null;

    private enum State { Cruising, Tracking, Ramming };
    private State _state = State.Cruising;
    private Vector3 _playersDetectedPosition;

    protected override void CalculateMovement()
    {
        switch (_state)
        {
            case State.Cruising:
                if (!DetectPlayer())
                {
                    base.CalculateMovement();
                }
                break;
            case State.Tracking:
                AlignWithPlayer();
                break;
            case State.Ramming:
                Ram();
                break;
            default:
                break;
        }
    }

    private bool DetectPlayer()
    {
        Collider2D collider = Physics2D.OverlapCircle(transform.position, _detectionRadius, _layerMask);
        if (collider)
        {
            _playersDetectedPosition = collider.transform.position;
            _state = State.Tracking;
            return true;
        }
        else
        {
            return false;
        }
    }

    private void AlignWithPlayer()
    {
        // Rotate along z-axis (using _speed) toward the position where the player was detected
        Vector3 vectorToTarget = _playersDetectedPosition - transform.position;
        float angle = (Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg) + 90f;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * _speed);

        // If a forward-facing ray hits the player, then switch to Ramming
        RaycastHit2D hit = Physics2D.Raycast(transform.position, _directionProbe.position - transform.position, Mathf.Infinity, _layerMask);
        if (hit.collider != null)
        {
            //Debug.Log(hit.collider.
            _state = State.Ramming;
        }
    }

    private void Ram()
    {
        transform.Translate(Vector3.down * _rammingSpeed * Time.deltaTime);

        // If out of bounds, reset roation, switch to Cruising, and respawn
        if (Mathf.Abs(transform.position.x) >= _xBoundary || Mathf.Abs(transform.position.y) >= _ySpawnPoint)
        {
            transform.rotation = Quaternion.identity;
            _state = State.Cruising;
            SpawnLocation();
        }
    }
}
