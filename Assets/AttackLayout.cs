using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AttackLayout : MonoBehaviour
{
    [SerializeField] CanvasGroup _cg;
    [SerializeField] Image _bg;
    [SerializeField] TMP_Text _attackName;

    public void Show(string newAttackName, Type attackType)
    {
        _cg.DOFade(1f, .5f);
        _bg.color = ResourceObjectHolder.Instance.GetTypeDataByType(attackType).Color;

        _attackName.text = newAttackName;
    }

    public void Hide()
    {
        _cg.DOFade(0f, .5f);
    }
}
