using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AreaLayoutFightPanel : MonoBehaviour
{
    [SerializeField] Image _areaImg;

    public void UpdateArea()
    {
        var data = Nakama.TinyJson.JsonParser.FromJson<Metadata>(NakamaManager.Instance.NakamaUserAccount.LastAccount.User.Metadata);

        _areaImg.sprite = NakamaData.Instance.GetAreaDataRef(data.area).Sprite;
    }

    public void HandleOpenAllArea()
    {
        UIManager.Instance.ChangeView(UIManager.Instance.AllAreaView);
    }
}
