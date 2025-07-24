using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProfileLayout : MonoBehaviour
{
    [SerializeField] TMP_Text _playerName,_playerTrophy;

    public void UpdateUsername(string newName)
    {
        _playerName.text = newName;
    }

    public void UpdateTrophy(string newTrophy)
    {
        _playerTrophy.text = newTrophy;
    }

    public void HandleOnProfileClick()
    {
        UIManager.Instance.ProfilePopup.OpenPopup();
    }
}
