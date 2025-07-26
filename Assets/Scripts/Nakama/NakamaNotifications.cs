using Nakama;
using Nakama.TinyJson;
using PimDeWitte.UnityMainThreadDispatcher;
using System;
using UnityEngine;

public class NakamaNotifications : MonoBehaviour
{
    IClient _client;
    ISession _session;
    ISocket _socket;

    Action<IApiNotification> _notificationHandler;

    public void Init(IClient client, ISession session, ISocket socket)
    {
        _client = client;
        _session = session;
        _socket = socket;

        _notificationHandler = m => UnityMainThreadDispatcher.Instance().Enqueue(() => OnReceivedNotification(m));

        _socket.ReceivedNotification += _notificationHandler;
    }

    private async void OnReceivedNotification(IApiNotification notification)
    {
        Reward reward = new Reward();

        switch (notification.Code)
        {
            case NotificationOpCodes.CURENCY:
                return;
            case NotificationOpCodes.BLAST:
                Blast rewardBlast = notification.Content.FromJson<Blast>();

                reward.blast = rewardBlast;

                NakamaManager.Instance.NakamaUserAccount.AddPlayerBlast(rewardBlast);
                break;
            case NotificationOpCodes.ITEM:
                Item rewardItem = notification.Content.FromJson<Item>();

                reward.item = rewardItem;

                NakamaManager.Instance.NakamaUserAccount.AddOrUpdateItem(rewardItem);
                break;
            default:
                break;
        }

        UIManager.Instance.RewardPopup.OpenPopup();
        UIManager.Instance.RewardPopup.UpdateData(reward);

        Debug.Log("REWARD");
    }
}
