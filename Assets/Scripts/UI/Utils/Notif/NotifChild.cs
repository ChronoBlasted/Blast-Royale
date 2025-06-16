using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotifChild : MonoBehaviour
{
    [SerializeField] NotifFlags _notifType;
    [SerializeField] NotifLayout _notifLayout;

    public void Register()
    {
        NotificationManager.Instance.Register(_notifLayout, _notifType);

        _notifLayout.Activate(0);
    }

    public void Unregister()
    {
        _notifLayout.Deactivate();

        NotificationManager.Instance.Unregister(_notifLayout, _notifType);
    }
}
