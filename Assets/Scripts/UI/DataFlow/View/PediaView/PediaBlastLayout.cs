using System;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PediaBlastLayout : MonoBehaviour
{
    [SerializeField] TMP_Text _idTxt, _nameTxt, _typeText, _hpTxt, _manaTxt, _attackTxt, _defenseTxt, _speedTxt;
    [SerializeField] Image _mainBG, _blastImg, _typeIcoImg, _rewardImg;
    [SerializeField] GameObject _rewardLayout;
    [SerializeField] Button _rewardButton;
    [SerializeField] NavBar _navBar;
    [SerializeField] Sprite _unknownSpr;
    [SerializeField] NotifChild _notifChildData1, _notifChildData2, _notifChildData3;

    BlastData _data;
    BlastType _currentBlastType;

    string _blastId;
    string _blastTypeString;

    public string BlastId { get => _blastId; }

    public void Init(BlastData data)
    {
        _data = data;

        _blastId = _data.id.ToString();

        _idTxt.text = "ID." + _data.id;
        _nameTxt.text = NakamaData.Instance.GetBlastDataRef(_data.id).Name.GetLocalizedString();
        _typeText.text = _data.type.ToString();
        _hpTxt.text = _data.hp.ToString();
        _manaTxt.text = _data.mana.ToString();
        _attackTxt.text = _data.attack.ToString();
        _defenseTxt.text = _data.defense.ToString();
        _speedTxt.text = _data.speed.ToString();

        TypeData typeData = ResourceObjectHolder.Instance.GetTypeDataByType(_data.type);

        _mainBG.color = typeData.Color;

        _typeIcoImg.sprite = typeData.Sprite;

        _navBar.Init();

        UpdateNotifLayout();
    }

    public void SetVersion(BlastType blastType)
    {
        _currentBlastType = blastType;
        _blastTypeString = ((int)_currentBlastType).ToString();

        switch (_currentBlastType)
        {
            case BlastType.None:
                break;
            case BlastType.Regular:
                _blastImg.sprite = NakamaData.Instance.GetBlastDataRef(_data.id).Sprite;
                break;
            case BlastType.Boss:
                _blastImg.sprite = NakamaData.Instance.GetBlastDataRef(_data.id).BossSprite;
                break;
            case BlastType.Shiny:
                _blastImg.sprite = NakamaData.Instance.GetBlastDataRef(_data.id).ShinySprite;
                break;
        }

        BlastVersionData data = NakamaData.Instance.BlastTracker[_blastId].versions[_blastTypeString];

        IsAlreadyCatch(data.catched);
        CanClaimReward(data.rewardClaimed == false && data.catched);
    }

    void UpdateNotifLayout()
    {
        var blastTracker = NakamaData.Instance.BlastTracker[_blastId].versions;

        bool data1 = blastTracker["1"].catched && blastTracker["1"].rewardClaimed == false;
        bool data2 = blastTracker["2"].catched && blastTracker["2"].rewardClaimed == false;
        bool data3 = blastTracker["3"].catched && blastTracker["3"].rewardClaimed == false;

        if (data1) _notifChildData1.Register();
        else _notifChildData1.Unregister();

        if (data2) _notifChildData2.Register();
        else _notifChildData2.Unregister();

        if (data3) _notifChildData3.Register();
        else _notifChildData3.Unregister();
    }

    void IsDiscovered()
    {
        var blastTracker = NakamaData.Instance.BlastTracker[_blastId].versions;

        bool data1 = blastTracker["1"].catched;
        bool data2 = blastTracker["2"].catched;
        bool data3 = blastTracker["3"].catched;


        if (data1 || data2 || data3)
        {
            _nameTxt.text = NakamaData.Instance.GetBlastDataRef(_data.id).Name.GetLocalizedString();
            _typeText.text = _data.type.ToString();
            _hpTxt.text = _data.hp.ToString();
            _manaTxt.text = _data.mana.ToString();
            _attackTxt.text = _data.attack.ToString();
            _defenseTxt.text = _data.defense.ToString();
            _speedTxt.text = _data.speed.ToString();

            TypeData typeData = ResourceObjectHolder.Instance.GetTypeDataByType(_data.type);

            _mainBG.color = typeData.Color;

            _typeIcoImg.sprite = typeData.Sprite;
        }
        else
        {
            _nameTxt.text = "—";
            _typeText.text = "—";
            _hpTxt.text = "—";
            _manaTxt.text = "—";
            _attackTxt.text = "—";
            _defenseTxt.text = "—";
            _speedTxt.text = "—"; ;

            _mainBG.color = Color.white;

            _typeIcoImg.sprite = _unknownSpr;
        }
    }

    void IsAlreadyCatch(bool isCatch)
    {
        _idTxt.text = "ID." + _data.id;

        if (isCatch)
        {
            _blastImg.color = Color.white;
        }
        else
        {
            _blastImg.color = Color.black;
        }

        IsDiscovered();
    }

    void CanClaimReward(bool canBeClaimed)
    {
        _rewardLayout.SetActive(canBeClaimed);

        _rewardButton.enabled = canBeClaimed;

        if (!canBeClaimed) return;

        switch (_currentBlastType)
        {
            case BlastType.Regular:
                _rewardImg.sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.Coin).Sprite;
                break;
            case BlastType.Boss:
                _rewardImg.sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.CoinThree).Sprite;
                break;
            case BlastType.Shiny:
                _rewardImg.sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.GemThree).Sprite;
                break;
        }
    }

    public async void HandleOnFirstCatch()
    {
        await NakamaManager.Instance.NakamaBlastTracker.ClaimFirstCatchReward(_data.id.ToString(), ((int)_currentBlastType).ToString());

        ShowReward();

        NakamaData.Instance.BlastTracker[_blastId].versions[_blastTypeString].rewardClaimed = true;

        CanClaimReward(false);

        UpdateNotifLayout();
    }

    private void ShowReward()
    {
        Reward reward = new Reward();

        switch (_currentBlastType)
        {
            case BlastType.Regular:
                reward.type = RewardType.Coin;
                reward.amount = 200;
                break;
            case BlastType.Boss:
                reward.type = RewardType.Coin;
                reward.amount = 1000;
                break;
            case BlastType.Shiny:
                reward.type = RewardType.Gem;
                reward.amount = 10;
                break;
        }

        UIManager.Instance.RewardPopup.OpenPopup();
        UIManager.Instance.RewardPopup.UpdateData(reward);
    }
}
