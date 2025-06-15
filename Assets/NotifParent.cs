using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotifParent : MonoBehaviour
{
    [SerializeField] NotifParent _nestedParentLayout;
    [SerializeField] NotifLayout _notifLayout;

    public List<NotifChild> _childs = new List<NotifChild>();

    public void AddChild(NotifChild child)
    {
        if (!_childs.Contains(child))
        {
            _childs.Add(child);
        }

        if (_childs.Count > 0)
        {
            _notifLayout.Init(_childs.Count);
        }

        if (_nestedParentLayout != null) _nestedParentLayout.AddChild(child);

    }

    public void RemoveChild(NotifChild child)
    {
        if (_childs.Contains(child))
        {
            _childs.Remove(child);
        }

        if (_childs.Count == 0)
        {
            _notifLayout.Remove();
        }
        else
        {
            _notifLayout.Init(_childs.Count);
        }

        if (_nestedParentLayout != null) _nestedParentLayout.RemoveChild(child);
    }
}
