using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AreaLayoutFightPanel : MonoBehaviour
{
    [SerializeField] Image _areaImg;

    public void UpdateArea(Sprite newSprite)
    {
        _areaImg.sprite = newSprite;
    }

    public void HandleOpenAllArea()
    {
        UIManager.Instance.ChangeView(UIManager.Instance.AllAreaView);
    }
}
