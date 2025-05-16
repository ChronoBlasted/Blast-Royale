using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildBattleOfferPopup : Popup
{
    [SerializeField] List<OfferLayout> _offersLayout;

    public void Init(List<Offer> offers)
    {
        for (int i = 0; i < _offersLayout.Count; i++)
        {
            _offersLayout[i].Init(offers[i], i);
        }
    }

    public override void OpenPopup(bool openBlackShade = true, bool openCloseButton = true)
    {
        base.OpenPopup(openBlackShade, openCloseButton);

        UIManager.Instance.BlackShadeView.ShadeButton.onClick.RemoveAllListeners();
    }
}
