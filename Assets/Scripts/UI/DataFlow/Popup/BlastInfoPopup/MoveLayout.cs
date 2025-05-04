using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MoveLayout : MonoBehaviour
{
    [SerializeField] TMP_Text _moveNameTxt, _moveDescTxt, _movePowerTxt, _moveCostTxt, _movePlatformCostTxt;
    [SerializeField] Image _moveGradientBG, _damageIco;
    [SerializeField] Button _button;
    [SerializeField] List<PlatformSlotLayout> _platformSlotLayouts;
    [SerializeField] CanvasGroup _contentCG;

    [SerializeField] GameObject  _platformLayout, _manaLayout;

    Blast _blast;
    Move _move;
    int _indexMove;

    public void Init(Move move, Blast blast, int index = -1)
    {
        _blast = blast;
        _move = move;
        _indexMove = index;

        _moveNameTxt.text = NakamaData.Instance.GetMoveDataRef(_move.id).Name.GetLocalizedString();

        var moveEffect = _move.effect;


        if (moveEffect != MoveEffect.None)
        {
            string Can = _move.attackType == AttackType.Special ? "" : "Can ";
            _moveDescTxt.text = Can + ResourceObjectHolder.Instance.GetResourceByType((ResourceType)moveEffect).Name.GetLocalizedString();
        }
        _moveDescTxt.gameObject.SetActive(moveEffect != MoveEffect.None);

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

            for (int i = 0; i < _platformSlotLayouts.Count; i++)
            {
                _platformSlotLayouts[i].Init(_move.type);

                _platformSlotLayouts[i].gameObject.SetActive(_move.cost > i);
            }

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
        bool canUseMove = false;

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
                canUseMove = _move.cost <= _blast.Mana;
                _moveCostTxt.color = canUseMove ? Color.white : Color.red;

                if (canUseMove) Unlock();
                else Lock();
                break;
            case AttackType.Special:

                canUseMove = GetAmountPlatformByType() >= _move.cost;

                for (int i = 0; i < _move.cost; i++)
                {
                    if (i < GetAmountPlatformByType())
                    {
                        _platformSlotLayouts[i].SetOn();
                    }
                    else
                    {
                        _platformSlotLayouts[i].SetOff();
                    }
                }

                if (canUseMove) Unlock();
                else Lock();
                break;
        }
    }


    public void UpdateOnClick(UnityAction action)
    {
        _button.onClick.RemoveAllListeners();

        _button.onClick.AddListener(() => UIManager.Instance.MoveSelectorPopup.NewMoveIndex = _indexMove);
        _button.onClick.AddListener(action);
    }

    public void Lock()
    {
        _contentCG.alpha = .5f;

        _button.interactable = false;
    }

    public void Unlock()
    {
        _contentCG.DOFade(1f, .5f);

        _button.interactable = true;
    }

    int GetAmountPlatformByType() => UIManager.Instance.GameView.PlayerHUD.BlastInWorld.PlatformLayout.GetAmountOfType(_move.type);

    int GetMoveDamage()
    {
        Blast defender = WildBattleManager.Instance.WildBlast;

        return NakamaLogic.CalculateDamage(_blast.Level, _blast.Attack, defender.Defense, _move.type, NakamaData.Instance.GetBlastDataById(defender.data_id).type, _move.power, WildBattleManager.Instance.Meteo);
    }
}
