using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MoveLayout : MonoBehaviour
{
    [SerializeField] TMP_Text _moveNameTxt, _moveDescTxt, _movePowerTxt, _moveCostTxt;
    [SerializeField] Image _moveBorder, _moveIco, _damageIco, _costIco;
    [SerializeField] CanvasGroup _cg;
    [SerializeField] Button _button;

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
            _moveDescTxt.gameObject.SetActive(true);
            string Can = _move.attackType == AttackType.Special ? "" : "Can ";
            _moveDescTxt.text = Can + ResourceObjectHolder.Instance.GetResourceByType((ResourceType)moveEffect).Name.GetLocalizedString();
        }
        else _moveDescTxt.gameObject.SetActive(false);

        _moveIco.sprite = ResourceObjectHolder.Instance.GetTypeDataByType(_move.type).Sprite;

        SetAttackCostData();

        if (_blast != null) UpdateUI();
        else _moveCostTxt.color = Color.white;

        _moveBorder.color = ResourceObjectHolder.Instance.GetTypeDataByType(_move.type).Color;
    }

    void SetAttackCostData()
    {
        switch (_move.attackType)
        {
            case AttackType.None:
                break;
            case AttackType.Normal:
                _costIco.sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.Mana).Sprite;
                _moveCostTxt.text = _move.cost.ToString();

                _damageIco.sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.AttackDamage).Sprite;
                _movePowerTxt.text = _move.power.ToString();
                break;
            case AttackType.Status:
                _costIco.sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.Mana).Sprite;
                _moveCostTxt.text = _move.cost.ToString();

                _damageIco.sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.AttackStatus).Sprite;
                _movePowerTxt.text = "";
                break;
            case AttackType.Special:
                _costIco.sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.PlatformCost).Sprite;
                _moveCostTxt.text = _move.cost.ToString();

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
            case AttackType.Status:
                canUseMove = _move.cost <= _blast.Mana;
                break;

            case AttackType.Special:
                canUseMove = _move.cost <= UIManager.Instance.GameView.PlayerHUD.BlastInWorld.PlatformLayout.GetAmountOfType(_move.type);
                break;
        }

        _moveCostTxt.color = canUseMove ? Color.white : Color.red;

        if (canUseMove)
        {
            Unlock();
        }
        else
        {
            Lock();
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
        _cg.interactable = false;

        _cg.alpha = .5f;
    }

    public void Unlock()
    {
        _cg.interactable = true;

        _cg.alpha = 1;
    }
}
