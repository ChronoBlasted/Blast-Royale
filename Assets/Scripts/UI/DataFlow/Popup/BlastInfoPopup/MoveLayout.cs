using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MoveLayout : MonoBehaviour
{
    [SerializeField] TMP_Text _moveNameTxt, _moveDescTxt, _movePowerTxt, _moveCostTxt, _movePlatformCostTxt, _lockTxt;
    [SerializeField] Image _moveGradientBG, _damageIco;
    [SerializeField] Button _button;
    [SerializeField] PlatformSlotLayout _platformSlotLayout;
    [SerializeField] CanvasGroup _contentCG;

    [SerializeField] GameObject _platformLayout, _manaLayout, _lockLayout;

    Blast _blast;
    Move _move;
    int _indexMove;
    bool _canUseMove = true;

    public TMP_Text MoveDescTxt { get => _moveDescTxt; }

    public void Init(Move move, Blast blast, int index = -1)
    {
        _blast = blast;
        _move = move;
        _indexMove = index;

        _moveNameTxt.text = NakamaData.Instance.GetMoveDataRef(_move.id).Name.GetLocalizedString();

        SetAttackCostData();

        if (_blast != null) UpdateUI();
        else _moveCostTxt.color = Color.white;

        _moveGradientBG.color = ResourceObjectHolder.Instance.GetTypeDataByType(_move.type).Color;
    }

    void SetAttackCostData()
    {
        if (_move.attackType == AttackType.Special)
        {
            _movePlatformCostTxt.text = _move.cost.ToString();

            _platformSlotLayout.Init(_move.type, _move.cost);

            _platformLayout.SetActive(true);
            _manaLayout.SetActive(false);
        }
        else
        {
            _moveCostTxt.text = _move.cost.ToString();

            _platformLayout.SetActive(false);
            _manaLayout.SetActive(true);
        }


        switch (_move.attackType)
        {
            case AttackType.None:
                break;
            case AttackType.Normal:
                _damageIco.sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.AttackDamage).Sprite;
                _movePowerTxt.text = _move.power.ToString();
                break;
            case AttackType.Status:
                _damageIco.sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.AttackStatus).Sprite;
                _movePowerTxt.text = "";
                break;
            case AttackType.Special:
                _damageIco.sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.AttackDamage).Sprite;
                _movePowerTxt.text = _move.power.ToString();
                break;
        }
    }

    public void UpdateUI()
    {
        _canUseMove = false;

        switch (_move.attackType)
        {
            case AttackType.Normal:
                _movePowerTxt.text = GetMoveDamage().ToString();
                break;
            case AttackType.Status:
                _movePowerTxt.text = "";
                break;
            case AttackType.Special:
                _movePowerTxt.text = GetMoveDamage().ToString();
                break;
        }


        switch (_move.attackType)
        {
            case AttackType.Normal:
            case AttackType.Status:
                _canUseMove = _move.cost <= _blast.Mana;
                _moveCostTxt.color = _canUseMove ? Color.white : Color.red;

                if (_canUseMove) Unlock();
                else Lock(ErrorType.NOT_ENOUGH_MANA);
                break;
            case AttackType.Special:
                _canUseMove = GetAmountPlatformByType() >= _move.cost;
                _movePlatformCostTxt.color = _canUseMove ? Color.white : Color.red;

                if (_canUseMove) Unlock();
                else Lock(ErrorType.NOT_ENOUGH_PLATFORM_CHARGE);
                break;
        }
    }

    public void HandleOnClick()
    {
        if (_canUseMove) WildBattleManager.Instance.PlayerAttack(_indexMove);
        else
        {
            if (_move.attackType == AttackType.Normal || _move.attackType == AttackType.Status)
                ErrorManager.Instance.ShowError(ErrorType.NOT_ENOUGH_MANA);
            else if (_move.attackType == AttackType.Special)
                ErrorManager.Instance.ShowError(ErrorType.NOT_ENOUGH_PLATFORM_CHARGE);
        }
    }

    public void UpdateOnClick(UnityAction action)
    {
        _button.onClick.RemoveAllListeners();

        _button.onClick.AddListener(() => UIManager.Instance.MoveSelectorPopup.NewMoveIndex = _indexMove);
        _button.onClick.AddListener(action);
    }

    public void Lock(ErrorType errorType, bool hardLock = false, string lockReason = "")
    {
        if (hardLock)
        {
            _contentCG.alpha = 1f;
            _lockLayout.SetActive(true);
            _lockTxt.text = lockReason;

        }
        else
        {
            _lockLayout.SetActive(false);
            _contentCG.alpha = .5f;
        }


        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(() => ErrorManager.Instance.ShowError(errorType));
    }

    public void Unlock()
    {
        _contentCG.DOFade(1f, .5f);

        _button.interactable = true;

        _button.onClick.RemoveAllListeners();
    }

    int GetAmountPlatformByType() => UIManager.Instance.GameView.PlayerHUD.BlastInWorld.PlatformLayout.GetAmountOfType(_move.type);

    int GetMoveDamage()
    {
        Blast defender = WildBattleManager.Instance.WildBlast;

        return NakamaLogic.CalculateDamage(_blast.Level, _blast.Attack, defender.Defense, _move.type, NakamaData.Instance.GetBlastDataById(defender.data_id).type, _move.power, WildBattleManager.Instance.Meteo);
    }
}
