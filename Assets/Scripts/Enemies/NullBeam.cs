using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NullBeam : MonoBehaviour
{
    [SerializeField] private float _yScaleMax = 2.68f;

    private enum State { Off, Growing, Sustaining, Shrinking };
    private State _state = State.Off;
    private float _duration;
    private float _startTime;
    private float _endTime;

    private void Update()
    {
        switch (_state)
        {
            case State.Off:
                break;
            case State.Growing:
                transform.localScale = new Vector3(
                transform.localScale.x,
                ((Time.time - _startTime) / (_duration / 3)) * _yScaleMax,
                transform.localScale.z);
                break;
            case State.Sustaining:
                break;
            case State.Shrinking:
                transform.localScale = new Vector3(
                transform.localScale.x,
                ((_endTime - Time.time) / (_duration / 3)) * _yScaleMax,
                transform.localScale.z);
                break;
            default:
                break;
        }
    }

    public void ShootBeam(float duration)
    {
        StartCoroutine(ShootBeamCoroutine(duration));
    }

    private IEnumerator ShootBeamCoroutine(float duration)
    {
        _duration = duration;
        _startTime = Time.time;
        _endTime = Time.time + duration;
        _state = State.Growing;

        yield return new WaitForSeconds(duration / 3);

        _state = State.Sustaining;

        yield return new WaitForSeconds(duration / 3);

        _state = State.Shrinking;

        yield return new WaitForSeconds(duration / 3);

        _state = State.Off;
    }

    //? ON Coll call player's Destroy ALL
    //? If a power-up, destory it
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.transform.GetComponent<Player>();
            if (player != null)
            {
                player.DestroyPowerups();
            }
        }
        else if (collision.CompareTag("Power Up"))
        {
            Destroy(collision.gameObject);
        }
    }
}
