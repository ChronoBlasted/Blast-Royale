using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class ProgressionLayout : MonoBehaviour
{
    [SerializeField] List<ProgressionSlotLayout> slots;           // Logique
    [SerializeField] List<RectTransform> slotRects;               // UI
    [SerializeField] RectTransform slotsContainer;

    [SerializeField] float moveDistance = 160f;
    [SerializeField] float duration = 0.2f;

    int _index;

    public void Init(int currentIndexProgression)
    {
        _index = currentIndexProgression;

        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].Init(_index + i);
            if (i == 0)
            {
                slots[i].SetActive();
            }
        }
    }

    public void SlideNext()
    {
        _index++;

        float totalMove = 0f;

        for (int i = 0; i < 2 && i < slotRects.Count; i++)
        {
            totalMove += slotRects[i].rect.width;
        }

        slotsContainer.DOAnchorPosX(slotsContainer.anchoredPosition.x - totalMove, duration).SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
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
}
