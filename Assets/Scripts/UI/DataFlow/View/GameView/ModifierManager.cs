using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ModifierManager : MonoBehaviour
{
    [SerializeField] Transform _modifierTransform;
    [SerializeField] ModifierLayout _modifierLayoutPrefab;

    List<ModifierLayout> _modifiers = new List<ModifierLayout>();

    public void Init()
    {
        foreach (Transform t in _modifierTransform)
        {
            Destroy(t.gameObject);
        }

        _modifiers.Clear();
    }

    public void AddModifier(StatType statType, int amount = 1)
    {
        var existingModifier = _modifiers.FirstOrDefault(m => m.StatType == statType);
        if (existingModifier != null)
        {
            existingModifier.Add(amount);

            if (existingModifier.Amount == 0)
            {
                _modifiers.Remove(existingModifier);
                Destroy(existingModifier.gameObject);
            }
            return;
        }

        var newModifier = Instantiate(_modifierLayoutPrefab, _modifierTransform);
        newModifier.Init(statType, amount);
        _modifiers.Add(newModifier);
    }

    public MoveEffect GetMoveEffectFromStat(StatType stat, int amount)
    {
        bool isBoost = amount > 0;

        switch (stat)
        {
            case StatType.Attack: return isBoost ? MoveEffect.AttackBoost : MoveEffect.AttackReduce;
            case StatType.Defense: return isBoost ? MoveEffect.DefenseBoost : MoveEffect.DefenseReduce;
            case StatType.Speed: return isBoost ? MoveEffect.SpeedBoost : MoveEffect.SpeedReduce;

            default: return MoveEffect.None;
        }
    }

}
