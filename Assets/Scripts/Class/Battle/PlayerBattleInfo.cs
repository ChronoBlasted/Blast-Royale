using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class PlayerBattleInfo
{
    public string Username;
    public BlastOwner OwnerType;
    public PlayerStat OwnerStat;
    public Blast ActiveBlast;
    public List<Blast> Blasts;
    public List<Item> Items;

    public PlayerBattleInfo(string username, BlastOwner ownerType, PlayerStat ownerStat, Blast activeBlast, List<Blast> blasts, List<Item> items)
    {
        Username = username;
        OwnerType = ownerType;
        OwnerStat = ownerStat;
        ActiveBlast = activeBlast;
        Blasts = blasts;
        Items = items;
    }
}
