using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoveSelectorPopup : Popup
{
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

        var blastData = NakamaData.Instance.GetBlastDataById(blast.data_id);
        var blastMoveset = blast.activeMoveset
            .Select(id => NakamaData.Instance.GetMoveById(id))
            .ToList();

        var availableMoves = new List<(Move move, bool isUnlocked)>();

        foreach (var moveInfo in blastData.movepool)
        {
            var move = NakamaData.Instance.GetMoveById(moveInfo.move_id);
            bool isUnlocked = blast.Level >= moveInfo.levelMin;
            availableMoves.Add((move, isUnlocked));
        }

        _currentMoveToReplace.Init(moveToReplace, null);

        foreach (Transform t in _scrollMoveAvaible)
        {
            Destroy(t.gameObject);
        }

        int index = 0;
        foreach (var (move, isUnlocked) in availableMoves)
        {
            var currentMove = Instantiate(_moveLayoutPrefab, _scrollMoveAvaible);
            currentMove.Init(move, null, index);
            index++;

            if (isUnlocked)
            {
                currentMove.UpdateOnClick(HandleChangeMove);
                currentMove.Unlock();

                if (blastMoveset.Contains(move))
                    currentMove.Lock("Already in use");
            }
            else
            {
                int requiredLevel = blastData.movepool.First(m => m.move_id == move.id).levelMin;
                currentMove.Lock($"Unlock at level {requiredLevel}");
            }
        }

        _noMoveAvailable.enabled = _scrollMoveAvaible.childCount <= 0;
    }


    public void HandleChangeMove()
    {
        NakamaManager.Instance.NakamaUserAccount.SwitchMoveBlast(_uuidBlast, _outMoveIndex, _newMoveIndex);

        ClosePopup();
    }
}
