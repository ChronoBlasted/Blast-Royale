using System;
using System.Buffers.Text;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Blast
{
    public string uuid;
    public int data_id;
    public int exp;
    public int iv;
    public bool boss;
    public bool shiny;

    int hp;
    int mana;

    public Status status;
    public List<int> activeMoveset;
    public List<ModifierBlastStruct> modifiers = new List<ModifierBlastStruct>();

    public int MaxHp { get => CalculateBlastHp(NakamaData.Instance.GetBlastDataById(data_id).hp, iv, Level); }
    public int MaxMana { get => CalculateBlastMana(NakamaData.Instance.GetBlastDataById(data_id).mana, iv, Level); }
    public int Hp { get => hp; set => hp = Math.Clamp(value, 0, MaxHp); }
    public int Mana { get => mana; set => mana = Math.Clamp(value, 0, MaxMana); }
    public float Attack => CalculateBlastStat(NakamaData.Instance.GetBlastDataById(data_id).attack, iv, Level) * GetModifierMultiplier(StatType.Attack);
    public float Defense => CalculateBlastStat(NakamaData.Instance.GetBlastDataById(data_id).defense, iv, Level) * GetModifierMultiplier(StatType.Defense);
    public float Speed => CalculateBlastStat(NakamaData.Instance.GetBlastDataById(data_id).speed, iv, Level) * GetModifierMultiplier(StatType.Speed);
    public int Level { get => NakamaLogic.CalculateLevelFromExperience(exp); }

    public Blast() { }

    public Blast(string uuid, int data, int exp, int iv, List<int> moveset, bool boss, bool shiny)
    {
        this.uuid = uuid;
        data_id = data;
        this.exp = exp;
        this.iv = iv;
        hp = CalculateBlastHp(NakamaData.Instance.GetBlastDataById(data).hp, iv, Level);
        mana = CalculateBlastMana(NakamaData.Instance.GetBlastDataById(data).mana, iv, Level);
        status = Status.None;
        activeMoveset = moveset;
        this.boss = boss;
        this.shiny = shiny;
    }

    public int CalculateBlastStat(int baseStat, int iv, int level)
    {
        float result = ((baseStat + iv) * level) / 100 + 5;
        return Mathf.FloorToInt(result);
    }

    public int CalculateBlastHp(int baseHp, int iv, int level)
    {
        float result = ((baseHp + iv) * level) / 100 + level + 10;
        return Mathf.FloorToInt(result);
    }

    public int CalculateBlastMana(float baseMana, float iv, float level)
    {
        float mana = ((baseMana + iv) * level) / 100f + 10f;
        return Mathf.FloorToInt(mana);
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

            if (mod == null) return 1f;

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

    public void ApplyModifier(MoveEffect effect, int amount = 1)
    {

        switch (effect)
        {
            case MoveEffect.AttackBoost:
                UpdateModifier(StatType.Attack, amount);
                break;
            case MoveEffect.DefenseBoost:
                UpdateModifier(StatType.Defense, amount);
                break;
            case MoveEffect.SpeedBoost:
                UpdateModifier(StatType.Speed, amount);
                break;

            case MoveEffect.AttackReduce:
                UpdateModifier(StatType.Attack, -amount);
                break;
            case MoveEffect.DefenseReduce:
                UpdateModifier(StatType.Defense, -amount);
                break;
            case MoveEffect.SpeedReduce:
                UpdateModifier(StatType.Speed, -amount);
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


