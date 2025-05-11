using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeInfoPopup : Popup
{
    [SerializeField] TypeInfoRowLayout _typeInfoRowPrefab;
    [SerializeField] Transform _typeInfoRowTransform;

    public override void Init()
    {
        base.Init();

        foreach (Type type in Enum.GetValues(typeof(Type)))
        {
            var row = Instantiate(_typeInfoRowPrefab, _typeInfoRowTransform);
            row.Init(type);
        }
    }
}
