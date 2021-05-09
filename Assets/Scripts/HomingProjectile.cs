using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingProjectile : Laser
{
    private Transform _target = null;

    private void Update()
    {
        if (_target != null)
        {
            transform.Translate((_target.position - transform.position).normalized * _speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.up * _speed * Time.deltaTime);

            Enemy[] enemies = FindObjectsOfType<Enemy>();
            _target = GetClosestEnemy(enemies);
        }
    }

    private Transform GetClosestEnemy(Enemy[] enemies)
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (var potentialTarget in enemies)
        {
            Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget.transform;
            }
        }

        return bestTarget;
    }
}
