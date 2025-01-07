using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NakamaUserAccount : MonoBehaviour
{
    public void Init()
    {

    }
}

[Serializable]
public class BlastCollection
{
    public List<Blast> deckBlasts;
    public List<Blast> storedBlasts;
}

[Serializable]
public class ItemCollection
{
    public List<Item> deckItems;
    public List<Item> storedItems;
}

public class SwapDeckRequest
{
    public int inIndex;
    public int outIndex;
}

public class SwapMoveRequest
{
    public string uuidBlast;
    public int outMoveIndex;
    public int newMoveIndex;
}

public class Metadata
{
    public bool battle_pass;
    public int area;
    public int win;
    public int loose;
    public int blast_captured;
    public int blast_defeated;
}
