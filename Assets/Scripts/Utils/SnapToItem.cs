using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SnapToItem : MonoBehaviour
{
    [SerializeField] ScrollRect scrollRect;
    public RectTransform contentPanel;
    [SerializeField] HorizontalLayoutGroup HLG;

    public float snapForce;

    RectTransform currentSampleItem;
    bool isSnapped;
    float snapSpeed;

    int currentIndex;

    public int CurrentIndex { get => currentIndex; }

    void Start()
    {
        isSnapped = false;
    }

    void Update()
    {
        if (contentPanel.childCount == 0) return;

        currentSampleItem = contentPanel.GetChild(0).GetComponent<RectTransform>();

        if (currentSampleItem.rect.width == 0) return;

        currentIndex = Mathf.RoundToInt(0 - contentPanel.localPosition.x / (currentSampleItem.rect.width + HLG.spacing));

        if (scrollRect.velocity.magnitude < 100 && !isSnapped)
        {
            scrollRect.velocity = Vector2.zero;
            snapSpeed += snapForce * Time.deltaTime;
            contentPanel.localPosition = new Vector3(
            Mathf.MoveTowards(contentPanel.localPosition.x, 0 - (currentIndex * (currentSampleItem.rect.width + HLG.spacing)), snapSpeed),
            contentPanel.localPosition.y,
            contentPanel.localPosition.z);


            if (contentPanel.localPosition.x == 0 - (currentIndex * (currentSampleItem.rect.width + HLG.spacing)))
            {
                isSnapped = true;
            }
        }
        if (scrollRect.velocity.magnitude > 10)
        {
            isSnapped = false;
            snapSpeed = 0;
        }
    }

    public void GoToItem(int index)
    {
        if (contentPanel.localPosition.x == 0 - (index * (currentSampleItem.rect.width + HLG.spacing))) return;

        scrollRect.velocity = Vector2.zero;
        contentPanel.DOLocalMoveX(0 - (index * (currentSampleItem.rect.width + HLG.spacing)), .5f);
    }
}
