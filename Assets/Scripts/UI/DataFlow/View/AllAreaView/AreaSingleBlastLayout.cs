using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AreaSingleBlastLayout : MonoBehaviour
{
    [SerializeField] TMP_Text _blastNameTxt;
    [SerializeField] Image _borderBG, _ico;

    BlastData _blast;
    public void Init(BlastData blast)
    {
        _blast = blast;

        _blastNameTxt.text = _blast.name;

        _ico.sprite = DataUtils.Instance.GetBlastImgByID(_blast.id);
        _borderBG.color = DataUtils.Instance.GetTypeColor(_blast.type);
    }

    public void HandleOnOpenInfo()
    {
        UIManager.Instance.BlastInfoPopup.UpdateData(_blast);

        UIManager.Instance.BlastInfoPopup.OpenPopup();
    }
}
