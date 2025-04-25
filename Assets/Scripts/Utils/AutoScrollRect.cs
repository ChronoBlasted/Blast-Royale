using UnityEngine;
using UnityEngine.UI;

public class AutoScrollRect : MonoBehaviour
{
    public ScrollRect scrollRect;
    public float scrollSpeed = 0.1f;
    public bool scrollVertical = true;

    void Update()
    {
        if (scrollRect == null) return;

        if (scrollVertical)
        {
            float newPos = scrollRect.verticalNormalizedPosition - scrollSpeed * Time.deltaTime;
            scrollRect.verticalNormalizedPosition = Mathf.Clamp01(newPos);
        }
        else
        {
            float newPos = scrollRect.horizontalNormalizedPosition + scrollSpeed * Time.deltaTime;
            scrollRect.horizontalNormalizedPosition = Mathf.Clamp01(newPos);
        }
    }
}
