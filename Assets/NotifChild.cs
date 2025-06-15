using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotifChild : MonoBehaviour
{
    [SerializeField] NotifParent _notifParent;
    [SerializeField] NotifLayout _notifLayout;

    public void SetParent(NotifParent parent)
    {
        _notifParent = parent;
    }

    public void Init()
    {
        _notifParent.AddChild(this);

        _notifLayout.Init(-1);
    }

    public void Remove()
    {
        _notifLayout.Remove();

        _notifParent.RemoveChild(this);
    }
}
