using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChronoTweenSequence : MonoBehaviour
{
    public List<ChronoTweenObject> ObjectsToTween;
    public UnityEvent OnTweenComplete;

    public void Init()
    {
        StartCoroutine(DoTweens());
    }

    IEnumerator DoTweens()
    {
        foreach (ChronoTweenObject obj in ObjectsToTween)
        {
            obj.DOPrepareTweenEffect();
        }

        foreach (ChronoTweenObject obj in ObjectsToTween)
        {
            obj.DOTweenEffect();
            yield return new WaitForSeconds(obj._delayBeforeNext);
        }

        OnTweenComplete?.Invoke();
    }
}
