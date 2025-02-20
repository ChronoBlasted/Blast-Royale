using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoveSelectorPopup : Popup
{
    [SerializeField] Image _bg;
    [SerializeField] MoveLayout _currentMoveToReplace;
    [SerializeField] Transform _scrollMoveAvaible;
    [SerializeField] MoveLayout _moveLayoutPrefab;
    [SerializeField] TMP_Text _noMoveAvailable;

    string _uuidBlast;
    int _outMoveIndex;
    int _newMoveIndex;

    public int NewMoveIndex { get => _newMoveIndex; set => _newMoveIndex = value; }

    public override void ClosePopup()
    {
        base.ClosePopup(false);
    }

    public override void OpenPopup()
    {
        base.OpenPopup(false);
    }

    public void UpdateData(Blast blast, Move moveToReplace, int outMoveIndex)
    {
        _uuidBlast = blast.uuid;
        _outMoveIndex = outMoveIndex;

        _bg.color = ResourceObjectHolder.Instance.GetTypeDataByType(moveToReplace.type).Color;

        List<Move> blastMoveset = blast.activeMoveset.Select(index => NakamaData.Instance.GetMoveById(index)).ToList();
        List<Move> availablesMoves = new List<Move>();

        availablesMoves = NakamaData.Instance.GetBlastDataById(blast.data_id)
            .movepool
            .Where(move => NakamaLogic.CalculateLevelFromExperience(blast.exp) >= move.levelMin)
            .Select(move => NakamaData.Instance.GetMoveById(move.move_id))
            .ToList();

        _currentMoveToReplace.Init(moveToReplace, null);

        foreach (Transform t in _scrollMoveAvaible)
        {
            Destroy(t.gameObject);
        }

        for (int i = 0; i < availablesMoves.Count; i++)
        {
            var currentMove = Instantiate(_moveLayoutPrefab, _scrollMoveAvaible);
            currentMove.Init(availablesMoves[i], null, i);

            currentMove.UpdateOnClick(HandleChangeMove);

            if (blastMoveset.Contains(availablesMoves[i])) currentMove.Lock();
            else currentMove.Unlock();
        }

        _noMoveAvailable.enabled = _scrollMoveAvaible.childCount <= 0;
    }

    public void HandleChangeMove()
    {

        Debug.Log("SWAP");
        NakamaManager.Instance.NakamaUserAccount.SwitchMoveBlast(_uuidBlast, _outMoveIndex, _newMoveIndex);
    }
}
