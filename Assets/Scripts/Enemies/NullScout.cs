using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NullScout : Enemy
{
    [SerializeField] private float _yDestination = 0f;
    [SerializeField] private float _delayAtDestination = 3f;

    private Vector3 _destination;
    private enum State { Descending, Firing, Ascending };
    private State _state = State.Descending;

    protected override void SpawnLocation()
    {
        transform.position = new Vector3(Random.Range(-_xSpawnRange, _xSpawnRange), _ySpawnPoint, _zSpawnPoint);
        
        SetDestination();
    }

    private void SetDestination()
    {
        if (transform.position.x >= 0)
        {
            _destination = new Vector3(
                transform.position.x - _ySpawnPoint,
                _yDestination,
                _zSpawnPoint);
        }
        else
        {
            _destination = new Vector3(
                transform.position.x + _ySpawnPoint,
                _yDestination,
                _zSpawnPoint);
        }

        _state = State.Descending;
    }

    protected override void CalculateMovement()
    {
        // If NOT Firing,
        if (_state != State.Firing)
        {
            // Move toward desitnation
            transform.position = Vector3.MoveTowards(
                transform.position,
                _destination,
                _speed * Time.deltaTime);

            Vector3 vectorToTarget = _destination - transform.position;
            float angle = (Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg) + 90f;
            Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * _speed);

            // If close to destination,
            if (Vector3.Distance(transform.position, _destination) < 0.001f)
            {
                if (_state == State.Descending)
                {
                    // Set new destination
                    _destination = new Vector3(transform.position.x, _ySpawnPoint, _zSpawnPoint);

                    // Hover while firing ray
                    StartCoroutine(Hover());
                }
                else if (_state == State.Ascending)
                {
                    SetDestination();
                }
            }
        }
        else
        {
            // Point stright down while hovering
            Vector3 vectorToTarget = Vector3.down;
            float angle = (Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg) + 90f;
            Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * _speed);
        }
    }

    private IEnumerator Hover()
    {
        _state = State.Firing;
        yield return new WaitForSeconds(_delayAtDestination);
        _state = State.Ascending;
    }
}
