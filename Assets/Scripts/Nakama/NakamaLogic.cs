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

    public static int CalculateDamage(
        int attackerLevel,
        float attackerAttack,
        float defenderDefense,
        Type attackType,
        Type defenderType,
        float movePower,
        Meteo meteo
    )
    {
        float weatherModifier = CalculateWeatherModifier(meteo, attackType);

        float typeMultiplier = GetTypeMultiplier(attackType, defenderType);

        float baseDamage = (
            (2f * attackerLevel / 5f + 2f)   
            * movePower                      
            * (attackerAttack / defenderDefense) 
        ) / 50f;                           

        float damage = baseDamage * typeMultiplier * weatherModifier;

        return Mathf.FloorToInt(damage);
    }


    public static bool IsBlastAlive(Blast blast)
    {
        return blast.Hp > 0;
    }

    public static bool IsAllBlastFainted(List<Blast> allPlayerBlasts)
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

    public static bool IsWeatherBoosted(Meteo weather, Type moveType)
    {
        switch (weather)
        {
            case Meteo.Sun:
                return moveType == Type.Fire;

            case Meteo.Rain:
                return moveType == Type.Water;

            case Meteo.Leaves:
                return moveType == Type.Grass;

            default:
                return false;
        }
    }

    public static float CalculateWeatherModifier(Meteo weather, Type moveType)
    {
        float modifier = 1.0f;

        switch (weather)
        {
            case Meteo.Sun:
                if (moveType == Type.Fire)
                    modifier = 1.5f;
                break;

            case Meteo.Rain:
                if (moveType == Type.Water)
                    modifier = 1.5f;
                break;

            case Meteo.Leaves:
                if (moveType == Type.Grass)
                    modifier = 1.5f;
                break;

            case Meteo.None:
            default:
                break;
        }

        return modifier;
    }


    public static void ApplyStatusEffectAtEndOfTurn(Blast blast, Blast otherBlast)
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
    }

    public static Blast ApplyEffectToBlast(Blast blast, Move move, MoveEffectData moveEffectData)
    {
        var isStatusMove = move.attackType == AttackType.Status;

        switch (moveEffectData.effect)
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
                blast.ApplyModifier(MoveEffect.AttackBoost, isStatusMove ? moveEffectData.effectModifier : 1);
                break;

            case MoveEffect.DefenseBoost:
                blast.ApplyModifier(MoveEffect.DefenseBoost, isStatusMove ? moveEffectData.effectModifier : 1);
                break;

            case MoveEffect.SpeedBoost:
                blast.ApplyModifier(MoveEffect.SpeedBoost, isStatusMove ? moveEffectData.effectModifier : 1);
                break;

            case MoveEffect.AttackReduce:
                blast.ApplyModifier(MoveEffect.AttackReduce, isStatusMove ? moveEffectData.effectModifier : 1);
                break;

            case MoveEffect.DefenseReduce:
                blast.ApplyModifier(MoveEffect.DefenseReduce, isStatusMove ? moveEffectData.effectModifier : 1);
                break;

            case MoveEffect.SpeedReduce:
                blast.ApplyModifier(MoveEffect.SpeedReduce, isStatusMove ? moveEffectData.effectModifier : 1);
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

        return Mathf.FloorToInt(Mathf.Pow(experience, 1f / 3f));
    }

    public static int CalculateExperienceFromLevel(int level)
    {
        if (level < 1 || level > 100)
        {
            throw new System.ArgumentOutOfRangeException("Le niveau doit être compris entre 1 et 100.");
        }

        return Mathf.FloorToInt(Mathf.Pow(level, 3));
    }

    public static Blast GetBlastByUUID(string uuid, BlastCollection allBlasts)
    {
        Blast selectedBlast = null;

        selectedBlast = allBlasts.deckBlasts.FirstOrDefault(blast => blast.uuid == uuid);
        if (selectedBlast == null) selectedBlast = allBlasts.storedBlasts.FirstOrDefault(blast => blast.uuid == uuid);

        return selectedBlast;
    }

    public static float GetTypeMultiplier(Type moveType, Type defenderType)
    {
        switch (moveType)
        {
            case Type.Fire:
                switch (defenderType)
                {
                    case Type.Grass:
                        return 2;
                    case Type.Water:
                        return 0.5f;
                    default:
                        return 1;
                }

            case Type.Water:
                switch (defenderType)
                {
                    case Type.Fire:
                        return 2;
                    case Type.Grass:
                        return 0.5f;
                    default:
                        return 1;
                }

            case Type.Grass:
                switch (defenderType)
                {
                    case Type.Water:
                        return 2;
                    case Type.Fire:
                        return 0.5f;
                    default:
                        return 1;
                }

            case Type.Normal:
                switch (defenderType)
                {
                    case Type.Light:
                        return 0.5f;
                    case Type.Dark:
                        return 0.5f;
                    default:
                        return 1;
                }

            case Type.Ground:
                switch (defenderType)
                {
                    case Type.Electric:
                        return 2;
                    case Type.Fly:
                        return 0;
                    default:
                        return 1;
                }

            case Type.Fly:
                switch (defenderType)
                {
                    case Type.Electric:
                        return 0;
                    case Type.Ground:
                        return 2;
                    default:
                        return 1;
                }

            case Type.Electric:
                switch (defenderType)
                {
                    case Type.Ground:
                        return 0;
                    case Type.Fly:
                        return 2;
                    default:
                        return 1;
                }

            case Type.Light:
                switch (defenderType)
                {
                    case Type.Dark:
                        return 2;
                    case Type.Normal:
                        return 2;
                    case Type.Light:
                        return 0.5f;
                    default:
                        return 1;
                }

            case Type.Dark:
                switch (defenderType)
                {
                    case Type.Light:
                        return 2;
                    case Type.Normal:
                        return 2;
                    case Type.Dark:
                        return 0.5f;
                    default:
                        return 1;
                }

            default:
                return 1;
        }
    }

    public static int GetAmountExpBall(BlastData blastData)
    {
        switch (blastData.rarity)
        {
            case Rarity.COMMON:
                return 2;
            case Rarity.UNCOMMON:
                return 3;
            case Rarity.RARE:
                return 5;
            case Rarity.EPIC:
                return 7;
            case Rarity.LEGENDARY:
                return 10;
            case Rarity.ULTIMATE:
                return 10;
            case Rarity.UNIQUE:
                return 5;
            default:
                return 0;
        }
    }

    public static TEnum GetEnumFromIndex<TEnum>(int index) where TEnum : Enum
    {
        TEnum[] values = (TEnum[])Enum.GetValues(typeof(TEnum));

        if (index < 0 || index >= values.Length)
            throw new ArgumentOutOfRangeException(nameof(index), "Index must be within the valid range.");

        return values[index];
    }

    public PlayerBattleInfo GetBlastOwner(Blast blast, List<PlayerBattleInfo> players)
    {
        foreach (var player in players)
        {
            if (player.Blasts.Contains(blast))
            {
                return player;
            }
        }

        return null;
    }
}
