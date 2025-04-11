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
    public static Blast ApplyEffectToBlast(Blast blast, Move move)
    {
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
                blast.ApplyModifier(MoveEffect.AttackBoost);
                break;

            case MoveEffect.DefenseBoost:
                blast.ApplyModifier(MoveEffect.DefenseBoost);
                break;

            case MoveEffect.SpeedBoost:
                blast.ApplyModifier(MoveEffect.SpeedBoost);
                break;

            case MoveEffect.AttackReduce:
                blast.ApplyModifier(MoveEffect.AttackReduce);
                break;

            case MoveEffect.DefenseReduce:
                blast.ApplyModifier(MoveEffect.DefenseReduce);
                break;

            case MoveEffect.SpeedReduce:
                blast.ApplyModifier(MoveEffect.SpeedReduce);
                break;

            case MoveEffect.Cleanse:
                blast.status = Status.None;
                break;
        }

        return blast;
    }

    public static string GetEffectMessage(MoveEffect move)
    {
        string message;
        switch (move)
        {
            case MoveEffect.Burn:
                message = "is now burned";
                break;

            case MoveEffect.Seeded:
                message = "is now seeded";
                break;

            case MoveEffect.Wet:
                message = "is now wet";
                break;

            case MoveEffect.ManaExplosion:
                message = "suffer from a mana explosion";
                break;

            case MoveEffect.HpExplosion:
                message = "suffer from an HP explosion";
                break;

            case MoveEffect.ManaRestore:
                message = "mana has been restored";
                break;

            case MoveEffect.HpRestore:
                message = "HP has been restored";
                break;

            case MoveEffect.AttackBoost:
                message = "attack is boosted";
                break;

            case MoveEffect.DefenseBoost:
                message = "defense is boosted";
                break;

            case MoveEffect.SpeedBoost:
                message = "speed is boosted";
                break;

            case MoveEffect.AttackReduce:
                message = "attack is reduced";
                break;

            case MoveEffect.DefenseReduce:
                message = "defense is reduced";
                break;

            case MoveEffect.SpeedReduce:
                message = "speed is reduced";
                break;

            case MoveEffect.Cleanse:
                message = "All status are cleansed";
                break;

            default:
                message = "No effect applied";
                break;
        }

        return message;
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
