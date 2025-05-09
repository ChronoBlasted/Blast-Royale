using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBattleInfo
{
    public string Username;
    public BlastOwner OwnerType;
    public Blast ActiveBlast;
    public List<Blast> Blasts;
    public List<Item> Items;

    public PlayerBattleInfo(string username, BlastOwner ownerType, Blast activeBlast, List<Blast> blasts, List<Item> items)
    {
        Username = username;
        OwnerType = ownerType;
        ActiveBlast = activeBlast;
        Blasts = blasts;
        Items = items;
    }
}
