using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressionLayout : MonoBehaviour
{
    [SerializeField] List<ProgressionSlotLayout> slots;

    public void Init(int currentIndexProgression)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].Init(currentIndexProgression + i);
        }
    }
}
