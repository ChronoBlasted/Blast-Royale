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
    }

    public void AddModifier(MoveEffect effectToAdd, MoveEffect oppositeEffect, int amount = 1)
    {
        var oppositeModifier = _modifiers.FirstOrDefault(m => m.MoveEffect == oppositeEffect);

        if (oppositeModifier != null)
        {
            oppositeModifier.Add(-amount);

            if (oppositeModifier.Amount <= 0)
            {
                _modifiers.Remove(oppositeModifier);
                Destroy(oppositeModifier.gameObject);
            }

            return;
        }

        var existingModifier = _modifiers.FirstOrDefault(m => m.MoveEffect == effectToAdd);
        if (existingModifier != null)
        {
            existingModifier.Add(amount);
            return;
        }

        var newModifier = Instantiate(_modifierLayoutPrefab, _modifierTransform);
        newModifier.Init(effectToAdd, amount);
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
