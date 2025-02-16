using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlastLayout : MonoBehaviour
{
    [SerializeField] TMP_Text _blastNameTxt, _blastLevelTxt;
    [SerializeField] Image _blastImg, _blastBorder;

    Blast _blast;
    int _index;

    public void Init(Blast blast, int index = -1)
    {
        BlastData blastData = NakamaData.Instance.GetBlastDataById(blast.data_id);

        _blast = blast;
        _index = index;

        _blastNameTxt.text = NakamaData.Instance.GetBlastDataRef(blast.data_id).Name.GetLocalizedString();
        _blastLevelTxt.text = "LVL." + NakamaLogic.CalculateLevelFromExperience(_blast.exp);

        _blastImg.sprite = NakamaData.Instance.GetBlastDataRef(blast.data_id).Sprite;
        _blastBorder.color = ColorManager.Instance.GetTypeColor(blastData.type);
    }

    public void HandleOnInfoButton()
    {
        if (UIManager.Instance.MenuView.SquadPanel.IsSwapMode)
        {
            NakamaManager.Instance.NakamaUserAccount.SwitchPlayerBlast(_index, UIManager.Instance.MenuView.SquadPanel.CurrentIndexStored);

            foreach (Transform child in UIManager.Instance.MenuView.SquadPanel.StoredBlastTransform.transform)
            {
                Destroy(child.gameObject);
            }

            HandleOnSwapDisable();
        }
        else
        {
            UIManager.Instance.BlastInfoPopup.UpdateData(_blast);

            UIManager.Instance.BlastInfoPopup.OpenPopup();
        }
    }

    public void HandleOnSwapEnable()
    {
        UIManager.Instance.MenuView.SquadPanel.IsSwapMode = true;
        UIManager.Instance.MenuView.SquadPanel.CurrentIndexStored = _index;

        // Do feedback on select

        UIManager.Instance.MenuView.SquadPanel.SwitchToSoloBlast(_blast);
    }

    public void HandleOnSwapDisable()
    {
        UIManager.Instance.MenuView.SquadPanel.IsSwapMode = false;
        UIManager.Instance.MenuView.SquadPanel.CurrentIndexStored = -1;

        // Do feedback on select

        UIManager.Instance.MenuView.SquadPanel.QuitSoloBlast();
    }
}
