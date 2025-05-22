using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ChronoTweenObject : MonoBehaviour
{
    public float _delayBeforeNext = .05f;

    [SerializeField] TweenBehavior _tweenBehavior;
    [SerializeField] Image _whiteCover;

    [SerializeField] float _duration = .2f;
    [SerializeField] Ease _ease = Ease.OutSine;

    public void DOPrepareTweenEffect()
    {
        switch (_tweenBehavior)
        {
            case TweenBehavior.None:
                break;

            case TweenBehavior.Scale:
                DOTween.Kill(transform);

                transform.localScale = Vector3.zero;
                break;
            case TweenBehavior.ScaleFlash:
                DOTween.Kill(transform);
                DOTween.Kill(_whiteCover);

                transform.localScale = Vector3.zero;

                _whiteCover.DOFade(1, 0);
                break;
            default:
                break;
        }
    }

    public void DOTweenEffect()
    {
        switch (_tweenBehavior)
        {
            case TweenBehavior.None:
                break;
            case TweenBehavior.Scale:
                transform.DOScale(Vector3.one, _duration).SetEase(_ease);
                break;
            case TweenBehavior.ScaleFlash:
                transform.DOScale(Vector3.one, _duration).SetEase(_ease);
                _whiteCover.DOFade(0, _duration / 2f).SetDelay(_duration / 2f);
                break;
            default:
                break;
        }
    }

}

public enum TweenBehavior
{
    None,
    ScaleFlash,
    Scale,
}