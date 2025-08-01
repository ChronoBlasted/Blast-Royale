using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;

public class ProgressionLayout : MonoBehaviour
{
    public ProgressionSlotLayout _permanentSlot;
    [SerializeField] List<ProgressionSlotLayout> slots;           // Logique
    [SerializeField] List<RectTransform> slotRects;               // UI
    [SerializeField] RectTransform slotsContainer;
    [SerializeField] CanvasGroup _cg;

    [SerializeField] float duration = 1f;

    int _index;
    bool _isOpen;
    Coroutine _hideCor;

    public void Init(int currentIndexProgression)
    {
        _index = currentIndexProgression - 1;

        _permanentSlot.Init(_index + 1);
        _permanentSlot.Punch();

        for (int i = 0; i < slots.Count; i++)
        {
            _index++;

            slots[i].Init(_index);

            if (i == 0)
            {
                slots[i].SetActive();
            }
        }
    }

    public void SlideNext()
    {
        Show();

        _index++;

        slots[0].SetInactive();

        float totalMove = 0f;

        for (int i = 0; i < 2 && i < slotRects.Count; i++)
        {
            totalMove += slotRects[i].rect.width;
        }

        slotsContainer.DOAnchorPosX(slotsContainer.anchoredPosition.x - totalMove, duration).SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                _permanentSlot.Init(_index - 3);
                _permanentSlot.Punch();

                for (int i = 0; i < 2; i++)
                {
                    if (i == 0)
                    {
                        var firstSlot = slots[0];

                        firstSlot.Init(_index);

                        slots.RemoveAt(0);
                        slots.Add(firstSlot);
                    }

                    var firstRT = slotRects[0];

                    slotRects.RemoveAt(0);
                    slotRects.Add(firstRT);

                    firstRT.SetAsLastSibling();
                }

                slotsContainer.anchoredPosition += new Vector2(totalMove, 0);

                slots[0].SetActive();
            });
    }

    public void Show()
    {
        DOTween.Kill(transform, true);
        DOTween.Kill(_cg, true);

        if (_isOpen == false)
        {
            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one, .2f).SetEase(Ease.OutBack);
            _cg.DOFade(1f, .2f);

            _isOpen = true;

        }
        if (_hideCor != null)
        {
            StopCoroutine(_hideCor);
            _hideCor = null;
        }

        _hideCor = StartCoroutine(HideCor());
    }

    public void Hide()
    {

        DOTween.Kill(_cg, true);

        _cg.DOFade(0f, .2f);

        _isOpen = false;
    }

    IEnumerator HideCor()
    {
        yield return new WaitForSeconds(duration + 1f);
        Hide();
    }
}
