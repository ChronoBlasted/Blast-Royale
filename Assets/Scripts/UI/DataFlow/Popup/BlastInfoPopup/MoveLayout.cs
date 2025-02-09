using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MoveLayout : MonoBehaviour
{
    [SerializeField] TMP_Text _moveNameTxt, _moveDescTxt, _movePowerTxt, _moveCostTxt;
    [SerializeField] Image _moveBorder;
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

        _moveNameTxt.text = _move.name;
        _moveDescTxt.text = _move.desc;
        _movePowerTxt.text = _move.power.ToString();
        _moveCostTxt.text = _move.cost.ToString();

        _moveCostTxt.color = Color.white;

        if (_blast != null) UpdateUI();

        _moveBorder.color = ColorManager.Instance.GetTypeColor(move.type);
    }

    public void UpdateUI()
    {
        if (_move.cost > _blast.Mana) _moveCostTxt.color = Color.red;
        else _moveCostTxt.color = Color.white;
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
