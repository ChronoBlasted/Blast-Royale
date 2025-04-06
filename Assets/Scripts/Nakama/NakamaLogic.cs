using BaseTemplate.Behaviours;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NakamaLogic : MonoSingleton<NakamaLogic>
{
    public static bool GetFasterBlast(Blast blast1, Blast blast2)
    {
        if (blast1.Speed > blast2.Speed) return true;
        else return false;
    }

    public static int CalculateDamage(int attackerLevel, int attackerAttack, int defenderDefense, TYPE attackerType, TYPE defenderType, int movePower)
    {
        float damage = ((2 * attackerLevel / 5 + 2) * movePower * GetTypeMultiplier(attackerType, defenderType) * (attackerAttack / defenderDefense) / 50) + 1;
        return Mathf.FloorToInt(damage);
    }

    public static bool IsBlastAlive(Blast blast)
    {
        return blast.Hp > 0;
    }

    public static bool IsAllBlastDead(List<Blast> allPlayerBlasts)
    {
        return allPlayerBlasts.All(blast => blast.Hp == 0);
    }

    public int CalculateStaminaRecovery(int maxStamina, int currentStamina, bool useWait = false)
    {
        int normalRecovery = Mathf.FloorToInt(maxStamina * 0.2f);

        int waitRecovery = Mathf.FloorToInt(maxStamina * 0.5f);

        int recoveredStamina = currentStamina + (useWait ? waitRecovery : normalRecovery);

        if (recoveredStamina > maxStamina)
        {
            recoveredStamina = maxStamina;
        }

        return recoveredStamina;
    }

    public static int CalculateExpGain(int expYield, int yourLevel, int enemyLevel)
    {
        int expGain = Mathf.FloorToInt(((expYield * enemyLevel / 7f) * ((2f * enemyLevel + 10) / (enemyLevel + yourLevel + 10f)) + 1));

        return expGain;
    }

    public static (Blast, Blast) ApplyStatusEffectAtEndOfTurn(Blast blast, Blast otherBlast)
    {
        switch (blast.status)
        {
            case Status.Burn:
                blast.Hp = Mathf.Max(0, blast.Hp - Mathf.FloorToInt(blast.MaxHp / 8f));
                break;

            case Status.Seeded:
                int healAmount = Mathf.FloorToInt(blast.MaxHp / 16f);

                blast.Hp = Mathf.Max(0, blast.Hp - healAmount);
                otherBlast.Hp = Mathf.Min(otherBlast.MaxHp, otherBlast.Hp + healAmount);
                break;

            default:
                break;
        }

        return (blast, otherBlast);
    }

    public static void ApplyEffect(Blast blast, Move move)
    {
        if (move.effect == null)
            return;

        float modifier;

        switch (move.effect)
        {
            case MoveEffect.Burn:
                blast.status = Status.Burn;
                break;

            case MoveEffect.Seeded:
                blast.status = Status.Seeded;
                break;

            case MoveEffect.Wet:
                blast.status = Status.Wet;
                break;

            case MoveEffect.ManaExplosion:
                int manaDmg = Mathf.FloorToInt(blast.MaxMana / 2f);
                blast.Hp = Mathf.Max(0, blast.Hp - manaDmg);
                blast.Mana = Mathf.FloorToInt(blast.Mana / 2f);
                break;

            case MoveEffect.HpExplosion:
                int hpCost = Mathf.FloorToInt(blast.MaxHp / 3f);
                blast.Hp = Mathf.Max(0, blast.Hp - hpCost);
                break;

            case MoveEffect.ManaRestore:
                blast.Mana += move.power;
                break;

            case MoveEffect.HpRestore:
                blast.Hp += move.power;
                break;

            case MoveEffect.AttackBoost:
                modifier = Mathf.FloorToInt(blast.Attack * 1.5f);
                blast.AttackModifer = Mathf.Min(modifier, 500);
                break;

            case MoveEffect.DefenseBoost:
                modifier = Mathf.FloorToInt(blast.Defense * 1.5f);
                blast.DefenseModifier = Mathf.Min(modifier, 500);
                break;

            case MoveEffect.SpeedBoost:
                modifier = Mathf.FloorToInt(blast.Speed * 1.5f);
                blast.SpeedModifier = Mathf.Min(modifier, 500);
                break;

            case MoveEffect.AttackReduce:
                modifier = Mathf.FloorToInt(blast.Attack * 0.75f);
                blast.AttackModifer = Mathf.Max(1, modifier);
                break;

            case MoveEffect.DefenseReduce:
                modifier = Mathf.FloorToInt(blast.Defense * 0.75f);
                blast.DefenseModifier = Mathf.Max(1, modifier);
                break;

            case MoveEffect.SpeedReduce:
                modifier = Mathf.FloorToInt(blast.Speed * 0.75f);
                blast.SpeedModifier = Mathf.Max(1, modifier);
                break;

            case MoveEffect.Cleanse:
                blast.status = Status.None;
                break;
        }
    }

    public static int CalculateLevelFromExperience(int experience)
    {
        if (experience < 0)
        {
            throw new System.ArgumentOutOfRangeException("L'expérience totale ne peut pas être négative.");
        }

        int niveau = 1;
        int experienceNiveau = 0;

        for (int i = 1; i <= 100; i++)
        {
            experienceNiveau = Mathf.FloorToInt((Mathf.Pow(i, 3) * 100) / 2);
            if (experience < experienceNiveau)
            {
                break;
            }
            niveau = i;
        }

        return niveau;
    }

    public static int CalculateExperienceFromLevel(int level)
    {
        if (level < 1 || level > 100)
        {
            throw new System.ArgumentOutOfRangeException("Le niveau doit être compris entre 1 et 100.");
        }

        int experienceNiveau = 0;

        for (int i = 1; i <= level; i++)
        {
            experienceNiveau = Mathf.FloorToInt((Mathf.Pow(i, 3) * 100) / 2); // Exemple de formule croissante
        }

        return experienceNiveau;
    }

    public static float GetTypeMultiplier(TYPE moveType, TYPE defenderType)
    {
        switch (moveType)
        {
            case TYPE.FIRE:
                switch (defenderType)
                {
                    case TYPE.GRASS:
                        return 2;
                    case TYPE.WATER:
                        return 0.5f;
                    default:
                        return 1;
                }

            case TYPE.WATER:
                switch (defenderType)
                {
                    case TYPE.FIRE:
                        return 2;
                    case TYPE.GRASS:
                        return 0.5f;
                    default:
                        return 1;
                }

            case TYPE.GRASS:
                switch (defenderType)
                {
                    case TYPE.WATER:
                        return 2;
                    case TYPE.FIRE:
                        return 0.5f;
                    default:
                        return 1;
                }

            case TYPE.NORMAL:
                switch (defenderType)
                {
                    case TYPE.LIGHT:
                        return 0.5f;
                    case TYPE.DARK:
                        return 0.5f;
                    default:
                        return 1;
                }

            case TYPE.GROUND:
                switch (defenderType)
                {
                    case TYPE.ELECTRIC:
                        return 2;
                    case TYPE.FLY:
                        return 0;
                    default:
                        return 1;
                }

            case TYPE.FLY:
                switch (defenderType)
                {
                    case TYPE.ELECTRIC:
                        return 0;
                    case TYPE.GROUND:
                        return 2;
                    default:
                        return 1;
                }

            case TYPE.ELECTRIC:
                switch (defenderType)
                {
                    case TYPE.GROUND:
                        return 0;
                    case TYPE.FLY:
                        return 2;
                    default:
                        return 1;
                }

            case TYPE.LIGHT:
                switch (defenderType)
                {
                    case TYPE.DARK:
                        return 2;
                    case TYPE.NORMAL:
                        return 2;
                    case TYPE.LIGHT:
                        return 0.5f;
                    default:
                        return 1;
                }

            case TYPE.DARK:
                switch (defenderType)
                {
                    case TYPE.LIGHT:
                        return 2;
                    case TYPE.NORMAL:
                        return 2;
                    case TYPE.DARK:
                        return 0.5f;
                    default:
                        return 1;
                }

            default:
                return 1;
        }
    }

    public static TEnum GetEnumFromIndex<TEnum>(int index) where TEnum : Enum
    {
        TEnum[] values = (TEnum[])Enum.GetValues(typeof(TEnum));

        if (index < 0 || index >= values.Length)
            throw new ArgumentOutOfRangeException(nameof(index), "Index must be within the valid range.");

        return values[index];
    }
}
