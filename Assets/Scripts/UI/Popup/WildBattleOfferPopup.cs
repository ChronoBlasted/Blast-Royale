using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WildBattleOfferPopup : Popup
{
    [SerializeField] List<WildBattleOfferLayout> _wildBattleOfferLayout;


    public void Init(List<Reward> offers)
    {
        for (int i = 0; i < _wildBattleOfferLayout.Count; i++)
        {
            _wildBattleOfferLayout[i].Init(offers[i], i);
        }
    }

    public override void OpenPopup(bool openBlackShade = true, bool openCloseButton = true)
    {
        base.OpenPopup(openBlackShade, openCloseButton);

        UIManager.Instance.BlackShadeView.ShadeButton.onClick.RemoveAllListeners();
    }

    public void OnSelectOffer(int index)
    {
        for (int i = 0; i < _wildBattleOfferLayout.Count; ++i)
        {
            if (i == index) continue;

            _wildBattleOfferLayout[i].OnUnselect();
        }


        _wildBattleOfferLayout[index].OnSelect();
    }
}
