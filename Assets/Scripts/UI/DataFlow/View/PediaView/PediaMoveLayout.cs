using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PediaMoveLayout : MonoBehaviour
{
    [SerializeField] MoveLayout _moveLayout;

    Move _data;

    public void Init(Move move)
    {
        _data = move;

        _moveLayout.Init(move, null);
        _moveLayout.MoveDescTxt.text = NakamaData.Instance.GetMoveDataRef(move.id).Desc.GetLocalizedString();
    }
}
