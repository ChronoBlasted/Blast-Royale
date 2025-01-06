using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlastLayout : MonoBehaviour
{
    [SerializeField] TMP_Text _blastNameTxt, _blastLevelTxt;
    [SerializeField] Image _blastImg, _blastBorder;

    Blast _blast;
    int _index;

    public void Init(Blast blast, int index = -1)
    {

    }
}
