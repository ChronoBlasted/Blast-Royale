using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

namespace Chrono.UI
{
    public class CustomButton : Button
    {
        enum ButtonType { NONE, SCALE }

        [SerializeField] ButtonType _type;

        [SerializeField] RectTransform _rt;

        [SerializeField] float _timeOfScale = .2f;



        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);

            //AudioManager.Instance.PlaySound("Button");

            switch (_type)
            {
                case ButtonType.SCALE:
                    ScaleDownElement();
                    break;
            }
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);

            switch (_type)
            {
                case ButtonType.SCALE:
                    ResetScaleElement();
                    break;
            }
        }

        private void ScaleDownElement() => _rt.DOScale(new Vector3(.95f, .95f, .95f), _timeOfScale).SetEase(Ease.OutExpo);
        private void ResetScaleElement() => _rt.DOScale(Vector3.one, _timeOfScale).SetEase(Ease.OutExpo);
    }
}

