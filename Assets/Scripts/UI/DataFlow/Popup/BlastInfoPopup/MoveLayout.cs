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

        _moveNameTxt.text = NakamaData.Instance.GetMoveDataRef(move.id).Name.GetLocalizedString();

        var moveEffect = move.effect;

        if (move.effect != MoveEffect.None)
        {
            _moveDescTxt.gameObject.SetActive(true);
            string Can = move.platform_cost > 0 ? "" : "Can ";
            _moveDescTxt.text = Can + ResourceObjectHolder.Instance.GetResourceByType((ResourceType)moveEffect).Name.GetLocalizedString();
        }
        else _moveDescTxt.gameObject.SetActive(false);

        _moveIco.sprite = ResourceObjectHolder.Instance.GetTypeDataByType(move.type).Sprite;

        SetAttackCostData();

        if (_blast != null) UpdateUI();
        else _moveCostTxt.color = Color.white;

        _moveBorder.color = ResourceObjectHolder.Instance.GetTypeDataByType(move.type).Color;
    }

    void SetAttackCostData()
    {
        if (_move.platform_cost > 0)
        {
            _costIco.sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.PlatformCost).Sprite;
            _moveCostTxt.text = _move.platform_cost.ToString();
        }
        else
        {
            _costIco.sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.Mana).Sprite;
            _moveCostTxt.text = _move.cost.ToString();
        }

        if (_move.power > 0)
        {
            _damageIco.sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.AttackDamage).Sprite;
            _movePowerTxt.text = _move.power.ToString();
        }
        else
        {
            _damageIco.sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.AttackStatus).Sprite;

            _movePowerTxt.text = "";
        }
    }

    public void UpdateUI()
    {
        if (_move.platform_cost > 0)
        {
            if (_move.platform_cost > UIManager.Instance.GameView.PlayerHUD.BlastInWorld.PlatformLayout.GetAmountOfType(_move.type))
            {
                _moveCostTxt.color = Color.red;
                Lock();
            }
            else
            {
                _moveCostTxt.color = Color.white;
                Unlock();
            }
        }
        else
        {
            if (_move.cost > _blast.Mana)
            {
                _moveCostTxt.color = Color.red;
                Lock();
            }
            else
            {
                _moveCostTxt.color = Color.white;
                Unlock();
            }
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
