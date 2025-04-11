using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Blast
{
    public string uuid;
    public int data_id;
    public int exp;
    public int iv;

    int hp;
    int mana;

    public Status status;
    public List<int> activeMoveset;
    public List<ModifierBlastStruct> modifiers = new List<ModifierBlastStruct>();

    public int MaxHp { get => CalculateBlastHp(NakamaData.Instance.GetBlastDataById(data_id).hp, iv, Level); }
    public int MaxMana { get => CalculateBlastMana(NakamaData.Instance.GetBlastDataById(data_id).mana, iv, Level); }
    public int Hp { get => hp; set => hp = Math.Clamp(value, 0, MaxHp); }
    public int Mana { get => mana; set => mana = Math.Clamp(value, 0, MaxMana); }
    public int Attack => Mathf.FloorToInt(CalculateBlastStat(Mathf.FloorToInt(NakamaData.Instance.GetBlastDataById(data_id).attack), Level, iv) * GetModifierMultiplier(StatType.Attack));
    public int Defense => Mathf.FloorToInt(CalculateBlastStat(Mathf.FloorToInt(NakamaData.Instance.GetBlastDataById(data_id).defense), Level, iv) * GetModifierMultiplier(StatType.Defense));
    public int Speed => Mathf.FloorToInt(CalculateBlastStat(Mathf.FloorToInt(NakamaData.Instance.GetBlastDataById(data_id).speed), Level, iv) * GetModifierMultiplier(StatType.Speed));
    public int Level { get => NakamaLogic.CalculateLevelFromExperience(exp); }

    public Blast(string uuid, int data, int exp, int iv, List<int> moveset)
    {
        this.uuid = uuid;
        data_id = data;
        this.exp = exp;
        this.iv = iv;
        hp = CalculateBlastHp(NakamaData.Instance.GetBlastDataById(data).hp, iv, Level);
        mana = CalculateBlastMana(NakamaData.Instance.GetBlastDataById(data).mana, iv, Level);
        status = Status.None;
        activeMoveset = moveset;
    }

    int CalculateBlastStat(int baseStat, int iv, int level)
    {
        return Mathf.FloorToInt(((2 * baseStat + iv) * level) / 100 + 5);
    }

    int CalculateBlastHp(int baseHp, int iv, int level)
    {
        return ((2 * baseHp + iv) * level) / 100 + level + 10;
    }

    int CalculateBlastMana(int baseMana, int iv, int level)
    {
        return Mathf.FloorToInt(((baseMana + iv) * (level / 100f) + level / 2) + 10);
    }

    public int GetRatioExp()
    {
        return exp - NakamaLogic.CalculateExperienceFromLevel(Level);
    }

    public int GetRatioExpNextLevel()
    {
        return NakamaLogic.CalculateExperienceFromLevel(Level + 1) - NakamaLogic.CalculateExperienceFromLevel(Level);
    }

    float GetModifierMultiplier(StatType stat)
    {
        if (modifiers == null) return 1f;

        if (modifiers.Count > 0)
        {
            var mod = modifiers.Find(m => m.stats == stat);

            int amount = mod.amount;
            if (amount > 0)
            {
                if (amount == 1) return 1.5f;
                if (amount == 2) return 2f;
                return 3f;
            }
            else if (amount < 0)
            {
                if (amount == -1) return 0.8f;
                if (amount == -2) return 0.6f;
                return 0.2f;
            }
        }

        return 1f;
    }

    public void ApplyModifier(MoveEffect effect)
    {
        switch (effect)
        {
            case MoveEffect.AttackBoost:
                UpdateModifier(StatType.Attack, 1);
                break;
            case MoveEffect.DefenseBoost:
                UpdateModifier(StatType.Defense, 1);
                break;
            case MoveEffect.SpeedBoost:
                UpdateModifier(StatType.Speed, 1);
                break;

            case MoveEffect.AttackReduce:
                UpdateModifier(StatType.Attack, -1);
                break;
            case MoveEffect.DefenseReduce:
                UpdateModifier(StatType.Defense, -1);
                break;
            case MoveEffect.SpeedReduce:
                UpdateModifier(StatType.Speed, -1);
                break;
        }
    }

    private void UpdateModifier(StatType stat, int amount)
    {

        var existingModifier = modifiers.Find(m => m.stats == stat);

        if (existingModifier != null)
        {
            existingModifier.amount += amount;

            if (existingModifier.amount == 0)
            {
                modifiers.Remove(existingModifier);
            }
        }
        else
        {
            modifiers.Add(new ModifierBlastStruct { stats = stat, amount = amount });
        }
    }
}


