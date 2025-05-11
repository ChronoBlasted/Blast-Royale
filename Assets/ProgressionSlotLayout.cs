using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressionSlotLayout : MonoBehaviour
{
    [SerializeField] Image _ico;
    [SerializeField] TMP_Text _progressionTxt;

    public void Init(int indexProgression)
    {
        _progressionTxt.text = indexProgression.ToString();
    }
}
