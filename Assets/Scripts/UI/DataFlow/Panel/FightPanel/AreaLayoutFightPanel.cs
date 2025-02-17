using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AreaLayoutFightPanel : MonoBehaviour
{
    [SerializeField] Image _areaImg;

    public void UpdateArea(Sprite sprite)
    {
        _areaImg.sprite = sprite;
    }

    public void HandleOpenAllArea()
    {
        UIManager.Instance.ChangeView(UIManager.Instance.AllAreaView);
    }
}
