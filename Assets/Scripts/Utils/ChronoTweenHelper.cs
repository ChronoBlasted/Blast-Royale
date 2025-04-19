using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class ChronoTweenHelper : MonoBehaviour
{
    [SerializeField] UnityEvent _tweenAction;

    [Space(5)]
    [Header("Value")]
    [SerializeField] float _amountFloat;
    [SerializeField] Vector3 _amountVector;

    [Space(5)]
    [Header("Timing")]
    [SerializeField] float _duration;
    [SerializeField] Ease _ease;
    [SerializeField] int _loopAmount;
    [SerializeField] LoopType _loopType;

    [Space(5)]
    [Header("Start")]
    [SerializeField] float _startDelay;

    Vector3 _startPos;
    Vector3 _startLocalPos;
    Vector3 _startLocalRotation;
    Vector3 _startLocalScale;

    public void Start()
    {
        _startPos = transform.position;
        _startLocalPos = transform.localPosition;

        _startLocalRotation = transform.rotation.eulerAngles;

        _startLocalScale = transform.localScale;

        _tweenAction?.Invoke();
    }

    public void DoLocalMoveY()
    {
        transform.DOLocalMoveY(_startLocalPos.y + _amountFloat, _duration).SetDelay(_startDelay).SetEase(_ease).SetLoops(_loopAmount, _loopType);
    }

    public void DoMoveY()
    {
        transform.DOMoveY(_startPos.y + _amountFloat, _duration).SetDelay(_startDelay).SetEase(_ease).SetLoops(_loopAmount, _loopType);
    }

    public void DoRotateZ()
    {
        transform.DORotate(new Vector3(_startLocalRotation.x, _startLocalRotation.y, _startLocalRotation.z + _amountFloat), _duration).SetDelay(_startDelay).SetEase(_ease).SetLoops(_loopAmount, _loopType);
    }

    public void DoScale()
    {
        transform.DOScale(_startLocalScale + _amountVector, _duration).SetDelay(_startDelay).SetEase(_ease).SetLoops(_loopAmount, _loopType);
    }

    public void DoScaleXThenY()
    {
        DOTween.Sequence()
            .Join(transform.DOScaleX(_startLocalScale.x + _amountVector.x, _duration).SetEase(_ease))
            .Append(transform.DOScaleY(_startLocalScale.y + _amountVector.y, _duration).SetEase(_ease))
            .SetDelay(_startDelay)
            .SetLoops(_loopAmount, _loopType);
    }
}
