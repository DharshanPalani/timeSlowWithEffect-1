using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class PlayerTimeDelay : MonoBehaviour
{
    public float slowDownDuration = 5f;
    public float slowTime = 0.1f;
    public float fixedDeltaTime = 0.2f;
    public float moveDuration;

    public UnityEvent slowDownEffectEvent;

    private bool _canSlow = false;

    [SerializeField] private GameObject _vignette;

    private void Update()
    {
        if (_canSlow)
        {
            SlowRealTime();
        }
        //else
        //{
        //    ResetRealTime();
        //}
    }


    private void SlowRealTime()
    {
        Time.timeScale = slowTime;
        Time.fixedDeltaTime = fixedDeltaTime * Time.timeScale;
    }

    public void ResetRealTime()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = fixedDeltaTime;

        _canSlow = false;

        Debug.Log("Time reset!");

        _vignette.SetActive(false);
    }

    private IEnumerator delayTime()
    {
        _vignette.SetActive(true);

        Vector3 playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        Vector3 vignettePosition = _vignette.transform.position;
        float startTime = Time.time;

        while (Time.time - startTime < moveDuration)
        {
            playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
            float t = (Time.time - startTime) / moveDuration;
            _vignette.transform.position = Vector3.Lerp(vignettePosition, playerPosition, t);
            yield return null;
        }

        slowDownEffectEvent.Invoke();

        _canSlow = true;

        Debug.Log("Vignette reached player's position!");

        yield return new WaitForSeconds(slowDownDuration);

        Debug.Log("The delay has ended!");


        vignettePosition = _vignette.transform.position;

        startTime = Time.time;

        playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        _vignette.transform.position = playerPosition;
        Vector3 newPosition = _vignette.transform.position;
        newPosition.y += 5f;
        while (Time.time - startTime < moveDuration)
        {
            float t = (Time.time - startTime) / moveDuration;
            _vignette.transform.position = Vector3.Lerp(vignettePosition, newPosition, t);
            yield return null;
        }

        ResetRealTime();
    }

    public void triggerSlow()
    {
        if (!_canSlow)
        {
            StartCoroutine(delayTime());
        }
    }
}
