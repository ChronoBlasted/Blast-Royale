using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErrorView : View
{
    [SerializeField] RectTransform _errorTransform, _startTransform, _endTransform;

    public override void Init()
    {
    }

    public void AddError(string error)
    {
        var currentErrorGO = PoolManager.Instance[ResourceType.ErrorMsg].Get();

        ErrorLayout currentError = currentErrorGO.GetComponent<ErrorLayout>();

        currentError.transform.SetParent(_errorTransform);

        currentError.transform.position = _startTransform.position;

        currentError.Init(error, _endTransform);
    }
}
