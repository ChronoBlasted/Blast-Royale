using System.Collections.Generic;
using System;
using UnityEngine;
using BaseTemplate.Behaviours;
using UnityEngine.Events;

public class NotificationManager : MonoSingleton<NotificationManager>
{
    public UnityAction<NotifFlags> OnNotifUpdate;

    Dictionary<NotifFlags, List<NotifLayout>> _allNotifs = new();

    public void Register(NotifLayout badge, NotifFlags category)
    {
        if (!_allNotifs.ContainsKey(category))
            _allNotifs[category] = new List<NotifLayout>();

        if (!_allNotifs[category].Contains(badge))
            _allNotifs[category].Add(badge);

        OnNotifUpdate?.Invoke(category);

    }

    public void Unregister(NotifLayout badge, NotifFlags category)
    {
        if (!_allNotifs.ContainsKey(category))
        {
            return;
        }

        _allNotifs[category].Remove(badge);

        OnNotifUpdate?.Invoke(category);
    }


    public int GetAmountOfNotifByFlags(NotifFlags flags)
    {
        return _allNotifs.ContainsKey(flags) ? _allNotifs[flags].Count : 0;
    }
}
