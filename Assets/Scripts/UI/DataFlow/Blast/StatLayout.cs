using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatLayout : MonoBehaviour
{
    [SerializeField] StatType _type;
    [SerializeField] TMP_Text _title;
    [SerializeField] Image _ico;

    public void Start()
    {
        var data = ResourceObjectHolder.Instance.GetResourceByType((ResourceType)_type);
        _title.text = data.Name.GetLocalizedString();
        if (_type != StatType.Type) _ico.sprite = data.Sprite;
    }
}
