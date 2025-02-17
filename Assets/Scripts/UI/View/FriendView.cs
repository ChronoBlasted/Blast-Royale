using Nakama;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FriendView : View
{
    [SerializeField] Transform _friendRequestTransform, _friendTransform, _friendWaitingTransform;
    [SerializeField] GameObject _headerFriendRequest;
    [SerializeField] FriendLayout _friendLayout;
    [SerializeField] TMP_Text _userName;

    public override void Init()
    {
        base.Init();
    }

    public override void OpenView(bool _instant = false)
    {
        base.OpenView(_instant);
    }

    public override void CloseView()
    {
        base.CloseView();
    }

    public void Close()
    {
        UIManager.Instance.ChangeView(UIManager.Instance.MenuView);
    }

    public void UpdateFriendList(IApiFriendList friendList)
    {
        _headerFriendRequest.SetActive(false);

        _friendRequestTransform.gameObject.SetActive(false);
        _friendTransform.gameObject.SetActive(false);
        _friendWaitingTransform.gameObject.SetActive(false);

        foreach (var friend in friendList.Friends)
        {
            Transform tempT = transform;

            switch (friend.State)
            {
                case 0: // Friend mutual
                    _friendTransform.gameObject.SetActive(true);

                    tempT = _friendTransform;
                    break;
                case 1: // friend request sent
                    _friendWaitingTransform.gameObject.SetActive(true);

                    tempT = _friendWaitingTransform;
                    break;
                case 2:// friend request received
                    _friendRequestTransform.gameObject.SetActive(true);
                    _headerFriendRequest.SetActive(true);

                    tempT = _friendRequestTransform;
                    break;
                case 3: // Ban friend
                    break;
            }

            FriendLayout currentFriend = Instantiate(_friendLayout, tempT);
            currentFriend.Init(friend);
        }
    }

    public void UpdateUsername(string username)
    {
        _userName.text = "Your username : " + username;
    }

    public void HandleOnAddFriendButton()
    {
        UIManager.Instance.ChangeView(UIManager.Instance.FriendView);
    }
}
