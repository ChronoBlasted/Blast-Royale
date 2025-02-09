using DG.Tweening;
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

    int hp;
    int maxHp;
    int mana;
    int maxMana;
    int attack;
    int defense;
    int speed;

    public Status status;
    public List<int> activeMoveset;

    public int MaxHp { get => CalculateBlastHp(NakamaData.Instance.GetBlastDataById(data_id).hp, iv, NakamaLogic.CalculateLevelFromExperience(exp)); }
    public int MaxMana { get => CalculateBlastMana(NakamaData.Instance.GetBlastDataById(data_id).mana, iv, NakamaLogic.CalculateLevelFromExperience(exp)); }
    public int Hp { get => hp; set => hp = Math.Clamp(value, 0, MaxHp); }
    public int Mana { get => mana; set => mana = Math.Clamp(value, 0, MaxMana); }
    public int Attack { get => CalculateBlastStat(NakamaData.Instance.GetBlastDataById(data_id).attack, NakamaLogic.CalculateLevelFromExperience(exp), iv); }
    public int Defense { get => CalculateBlastStat(NakamaData.Instance.GetBlastDataById(data_id).defense, NakamaLogic.CalculateLevelFromExperience(exp), iv); }
    public int Speed { get => CalculateBlastStat(NakamaData.Instance.GetBlastDataById(data_id).speed, NakamaLogic.CalculateLevelFromExperience(exp), iv); }

    public Blast(string uuid, int data, int exp, int iv, List<int> moveset, Status status = Status.NONE)
    {
        this.uuid = uuid;
        this.data_id = data;
        this.exp = exp;
        this.iv = iv;
        hp = CalculateBlastHp(NakamaData.Instance.GetBlastDataById(data).hp, iv, NakamaLogic.CalculateLevelFromExperience(exp));
        mana = CalculateBlastMana(NakamaData.Instance.GetBlastDataById(data).mana, iv, NakamaLogic.CalculateLevelFromExperience(exp));
        this.status = status;
        this.activeMoveset = moveset;
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
}