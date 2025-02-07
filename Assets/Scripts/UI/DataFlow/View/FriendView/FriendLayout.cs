using Nakama;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FriendLayout : MonoBehaviour
{
    IApiFriend _friend;

    [SerializeField] TMP_Text _friendNameTxt;

    [SerializeField] Button _fightButton, _acceptButton, _declineButton;
    [SerializeField] Image _waitingIco;

    public void Init(IApiFriend friend)
    {
        _friend = friend;

        _fightButton.gameObject.SetActive(false);
        _acceptButton.gameObject.SetActive(false);
        _declineButton.gameObject.SetActive(false);
        _waitingIco.gameObject.SetActive(false);

        _friendNameTxt.text = _friend.User.Username;

        switch (_friend.State)
        {
            case 0: // Friend mutual
                _fightButton.gameObject.SetActive(true);
                break;
            case 1: // friend request sent
                _declineButton.gameObject.SetActive(true);
                _waitingIco.gameObject.SetActive(true);
                break;
            case 2:// friend request received
                _acceptButton.gameObject.SetActive(true);
                _declineButton.gameObject.SetActive(true);
                break;
            case 3: // Ban friend
                break;
        }
    }

    public void UpdateStatus()
    {

    }

    public void HandleOnOpenInfo()
    {
        // TODO Get info and displayed it
    }

    public void HandleOnAcceptRequest()
    {
        NakamaManager.Instance.NakamaFriends.AcceptFriend(_friend.User.Username);
    }

    public void HandleOnDeclineRequest()
    {
        NakamaManager.Instance.NakamaFriends.DeleteFriend(_friend.User.Username);
    }

    public void HandleOnBattleRequest()
    {
        // TODO Send match request to other
    }
}
