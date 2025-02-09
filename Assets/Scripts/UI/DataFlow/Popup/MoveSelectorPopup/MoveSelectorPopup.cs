using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MoveSelectorPopup : Popup
{
    [SerializeField] Image _bg;
    [SerializeField] MoveLayout _currentMoveToReplace;
    [SerializeField] Transform _scrollMoveAvaible;
    [SerializeField] MoveLayout _moveLayoutPrefab;

    string _uuidBlast;
    int _outMoveIndex;
    int _newMoveIndex;

    public int NewMoveIndex { get => _newMoveIndex; set => _newMoveIndex = value; }

    public override void ClosePopup()
    {
        base.ClosePopup();
    }

    public override void OpenPopup()
    {
        base.OpenPopup();
    }

    public void UpdateData(Blast blast, Move moveToReplace, int outMoveIndex)
    {
        _uuidBlast = blast.uuid;
        _outMoveIndex = outMoveIndex;

        _bg.color = ColorManager.Instance.GetTypeColor(moveToReplace.type);

        List<Move> blastMoveset = blast.activeMoveset.Select(index => DataUtils.Instance.GetMoveById(index)).ToList();
        List<Move> availablesMoves = new List<Move>();

        availablesMoves = DataUtils.Instance.GetBlastDataById(blast.data_id)
            .movepool
            .Where(move => NakamaLogic.CalculateLevelFromExperience(blast.exp) >= move.levelMin)
            .Select(move => DataUtils.Instance.GetMoveById(move.move_id))
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
    }

    public void HandleChangeMove()
    {
        NakamaManager.Instance.NakamaUserAccount.SwitchMoveBlast(_uuidBlast, _outMoveIndex, _newMoveIndex);
    }
}
