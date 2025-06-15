using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotifParent : MonoBehaviour
{
    [SerializeField] NotifFlags _flagTracked;
    [SerializeField] NotifLayout _notifLayout;

    private void Awake()
    {
        NotificationManager.Instance.OnNotifUpdate += OnNotifUpdate;
        OnNotifUpdate(_flagTracked);
    }

    void OnNotifUpdate(NotifFlags category)
    {
        if (_flagTracked.HasFlag(category))
        {
            var amount = 0;

            foreach (NotifFlags flag in System.Enum.GetValues(typeof(NotifFlags)))
            {
                if (flag == NotifFlags.None)
                    continue;

                if ((_flagTracked & flag) == flag)
                {
                    amount += NotificationManager.Instance.GetAmountOfNotifByFlags(flag);
                }
            }

            if (amount <= 0)
            {
                _notifLayout.Deactivate();
            }
            else
            {
                _notifLayout.Activate(amount);
            }
        }
    }
}
