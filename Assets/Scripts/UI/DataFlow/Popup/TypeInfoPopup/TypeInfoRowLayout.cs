using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypeInfoRowLayout : MonoBehaviour
{
    [SerializeField] Image _currentTypeIco;
    [SerializeField] Transform _weakerTypeTransform, _strongerTypeTransform;
    [SerializeField] Image _icoTypePrefab;

    Type _currentType;
    TypeData _data;

    public void Init(Type newType)
    {
        _currentType = newType;
        _data = ResourceObjectHolder.Instance.GetTypeDataByType(_currentType);

        _currentTypeIco.sprite = _data.Sprite;

        foreach (Type weakerType in _data.TypeWeaker)
        {
            var currentType = Instantiate(_icoTypePrefab, _weakerTypeTransform);
            currentType.sprite = ResourceObjectHolder.Instance.GetTypeDataByType(weakerType).Sprite;
        }

        foreach (Type strongerType in _data.TypeStronger)
        {
            var currentType = Instantiate(_icoTypePrefab, _strongerTypeTransform);
            currentType.sprite = ResourceObjectHolder.Instance.GetTypeDataByType(strongerType).Sprite;
        }
    }
}
