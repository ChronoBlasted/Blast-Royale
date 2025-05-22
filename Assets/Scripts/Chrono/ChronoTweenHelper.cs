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

    Tweener _currentTween;

    public UnityEvent TweenAction { get => _tweenAction; }

    void Awake()
    {
        _startPos = transform.position;
        _startLocalPos = transform.localPosition;

        _startLocalRotation = transform.rotation.eulerAngles;

        _startLocalScale = transform.localScale;

        _tweenAction?.Invoke();
    }

    public void DoLocalMoveY()
    {
        _currentTween = transform.DOLocalMoveY(_startLocalPos.y + _amountFloat, _duration).SetDelay(_startDelay).SetEase(_ease).SetLoops(_loopAmount, _loopType);
    }

    public void DoMoveY()
    {
        _currentTween = transform.DOMoveY(_startPos.y + _amountFloat, _duration).SetDelay(_startDelay).SetEase(_ease).SetLoops(_loopAmount, _loopType);
    }

    public void DoRotateZ()
    {
        _currentTween = transform.DORotate(new Vector3(_startLocalRotation.x, _startLocalRotation.y, _startLocalRotation.z + _amountFloat), _duration).SetDelay(_startDelay).SetEase(_ease).SetLoops(_loopAmount, _loopType);
    }

    public void DoScale()
    {
        _currentTween = transform.DOScale(_startLocalScale + _amountVector, _duration).SetDelay(_startDelay).SetEase(_ease).SetLoops(_loopAmount, _loopType);
    }

    private void OnDestroy()
    {
        _currentTween.Kill(true);
    }
}
