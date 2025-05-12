using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlastLayout : MonoBehaviour
{
    [SerializeField] TMP_Text _blastNameTxt, _blastLevelTxt, _blastIvTxt;
    [SerializeField] Image _blastImg, _bg;
    [SerializeField] HiddenInfoMenu _hiddenInfoMenu;

    [SerializeField] bool _isDeckBlast;

    Blast _blast;
    int _index;

    public Blast Blast { get => _blast; }

    public void Init(Blast blast, int index = -1)
    {
        BlastData blastData = NakamaData.Instance.GetBlastDataById(blast.data_id);

        _blast = blast;
        _index = index;

        _blastNameTxt.text = NakamaData.Instance.GetBlastDataRef(blast.data_id).Name.GetLocalizedString();
        _blastLevelTxt.text = "LVL." + NakamaLogic.CalculateLevelFromExperience(_blast.exp);
        _blastIvTxt.text = "IV:" + _blast.iv;

        _blastImg.sprite = NakamaData.Instance.GetBlastDataRef(blast.data_id).Sprite;
        _bg.color = ResourceObjectHolder.Instance.GetTypeDataByType(blastData.type).Color;
    }

    public void HandleOnInfoButton()
    {
        UIManager.Instance.BlastInfoPopup.UpdateData(_blast);

        UIManager.Instance.BlastInfoPopup.OpenPopup();
    }

    public void HandleOnSwap()
    {
        UIManager.Instance.MenuView.SquadPanel.IsSwapMode = true;
        UIManager.Instance.MenuView.SquadPanel.IsDeckSwap = _isDeckBlast;
        UIManager.Instance.MenuView.SquadPanel.CurrentIndexStored = _index;

        // Do feedback on select

        UIManager.Instance.MenuView.SquadPanel.SwitchToSoloBlast(_blast);
    }

     void DisableSwap()
    {
        UIManager.Instance.MenuView.SquadPanel.IsSwapMode = false;
        UIManager.Instance.MenuView.SquadPanel.CurrentIndexStored = -1;

        // Do feedback on select

        UIManager.Instance.MenuView.SquadPanel.QuitSoloBlast();
    }

    public void HandleOnClick()
    {
        if (UIManager.Instance.MenuView.SquadPanel.IsSwapMode)
        {
            NakamaManager.Instance.NakamaUserAccount.SwitchPlayerBlast(_index, UIManager.Instance.MenuView.SquadPanel.CurrentIndexStored, UIManager.Instance.MenuView.SquadPanel.IsDeckSwap);

            foreach (Transform child in UIManager.Instance.MenuView.SquadPanel.StoredBlastTransform.transform)
            {
                Destroy(child.gameObject);
            }

            DisableSwap();
        }
        else
        {
            _hiddenInfoMenu.HandleOnClick();
        }
    }
}
