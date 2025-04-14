using TMPro;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.UI;

public class ModifierLayout : MonoBehaviour
{
    public MoveEffect MoveEffect;

    [SerializeField] Image _ico;
    [SerializeField] TMP_Text _amountTxt;

    int _amount;

    public int Amount { get => _amount;}

    public void Init(MoveEffect newEffect, int amount)
    {
        MoveEffect = newEffect;

        var data = ResourceObjectHolder.Instance.GetResourceByType((ResourceType)MoveEffect);

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
