using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WildBattleOfferLayout : MonoBehaviour
{
    [SerializeField] Image _lockCover;
    [SerializeField] Canvas _canvas;
    [SerializeField] OfferLayout _offerLayout;
    [SerializeField] Button _clickBtn;

    [SerializeField] ParticleSystem _offerTakenVFX;

    int _index;
    public void Init(Offer offer, int index)
    {
        _index = index;

        _clickBtn.interactable = true;

        transform.localScale = Vector3.one;

        _lockCover.DOFade(0f, 0f);

        _offerTakenVFX.Stop();

        _canvas.sortingOrder = 10;

        _offerLayout.Init(offer);
    }

    public void HandleOnClick()
    {
        UIManager.Instance.WildBattleOfferPopup.OnSelectOffer(_index);
    }

    public void OnSelect()
    {
        _canvas.sortingOrder = 11;

        _clickBtn.interactable = false;

        _offerTakenVFX.Play();

        transform.DOScale(Vector3.one * 1.2f, .5f).SetEase(Ease.OutBack);

        StartCoroutine(ContinueFlow());
    }

    IEnumerator ContinueFlow()
    {
        yield return new WaitForSeconds(2f);
        WildBattleManager.Instance.PlayerChooseOffer(_index);

        UIManager.Instance.WildBattleOfferPopup.ClosePopup();
    }

    public void OnUnselect()
    {
        _clickBtn.interactable = false;

        transform.DOScale(Vector3.one * .8f, .2f);
        _lockCover.DOFade(.8f, .2f);
    }
}
