using TMPro;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.UI;

public class ModifierLayout : MonoBehaviour
{
    public StatType StatType;

    [SerializeField] Image _ico;
    [SerializeField] TMP_Text _amountTxt;

    int _amount;

    public int Amount { get => _amount;}

    public void Init(StatType newStatType, int amount)
    {
        StatType = newStatType;

        var data = ResourceObjectHolder.Instance.GetResourceByType((ResourceType)StatType);

        _ico.sprite = data.Sprite;
        _amount = 0;

        Add(amount);
    }

    public void Add(int amountToAdd = 1)
    {
        _amount += amountToAdd;
        _amount = Mathf.Clamp(_amount, -3, 3);

        _amountTxt.text = _amount.ToString("+0;-0;0");
    }
}
