using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ModifierManager : MonoBehaviour
{
    [SerializeField] Transform _modifierTransform;
    [SerializeField] ModifierLayout _modifierLayoutPrefab;

    List<ModifierLayout> _modifiers = new List<ModifierLayout>();

    public void AddModifier(MoveEffect effectToAdd, int amount = 1)
    {
        var oppositeEffect = GetOpposite(effectToAdd);

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

    MoveEffect GetOpposite(MoveEffect effect)
    {
        switch (effect)
        {
            case MoveEffect.AttackBoost: return MoveEffect.AttackReduce;
            case MoveEffect.AttackReduce: return MoveEffect.AttackBoost;
            case MoveEffect.DefenseBoost: return MoveEffect.DefenseReduce;
            case MoveEffect.DefenseReduce: return MoveEffect.DefenseBoost;
            case MoveEffect.SpeedBoost: return MoveEffect.SpeedReduce;
            case MoveEffect.SpeedReduce: return MoveEffect.SpeedBoost;
            default: return MoveEffect.None;
        }
    }

}
