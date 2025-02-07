using BaseTemplate.Behaviours;
using DG.Tweening;
using System.Collections;
using UnityEngine;


public class TimeManager : MonoSingleton<TimeManager>
{
    Coroutine _lagCoroutine;

    public void Init()
    {
        SetTime(1);
    }

    public void DoLagTime(float intensity = .2f, float timeBeforeLerp = .1f)
    {
        if (_lagCoroutine != null)
        {
            StopCoroutine(_lagCoroutine);
            _lagCoroutine = null;
        }

        _lagCoroutine = StartCoroutine(LagCoroutine(intensity, timeBeforeLerp));
    }

    IEnumerator LagCoroutine(float intensity, float timeBeforeReset)
    {
        SetTime(intensity);

        yield return new WaitForSecondsRealtime(timeBeforeReset);

        SetTime(1);
    }

    void SetTime(float intensity = .2f)
    {
        Time.timeScale = intensity;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }
}
