using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;

public class ProgressionLayout : MonoBehaviour
{
    [SerializeField] ProgressionSlotLayout _permanentSlot;
    [SerializeField] List<ProgressionSlotLayout> slots;           // Logique
    [SerializeField] List<RectTransform> slotRects;               // UI
    [SerializeField] RectTransform slotsContainer;
    [SerializeField] CanvasGroup _cg;

    [SerializeField] float duration = 1f;

    int _index;

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
        transform.localScale = Vector3.zero;

        transform.DOScale(Vector3.one, .2f).SetEase(Ease.OutBack);
        _cg.DOFade(1f, .2f);

        StartCoroutine(HideCor());
    }

    public void Hide()
    {
        _cg.DOFade(0f, .2f);
    }

    IEnumerator HideCor()
    {
        yield return new WaitForSeconds(duration + 1f);
        Hide();
    }
}
