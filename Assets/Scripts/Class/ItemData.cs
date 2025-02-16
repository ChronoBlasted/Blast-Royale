using System;

[Serializable]
public class Item
{
    public int data_id;
    public int amount;

    public Item(int data_id, int amount)
    {
        this.data_id = data_id;
        this.amount = amount;
    }
}

public class ItemData
{
    public int id;
    public ItemBehaviour behaviour;
    public int gain_amount;
    public Status status;
    public float catchRate;
}