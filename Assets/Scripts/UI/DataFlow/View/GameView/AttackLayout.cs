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
    [SerializeField] bool isInverted;

    public void Show(string newAttackName, Type attackType)
    {
        _cg.DOFade(1f, .5f);
        _bg.color = ResourceObjectHolder.Instance.GetTypeDataByType(attackType).Color;

        _attackName.text = newAttackName;

        transform.localPosition = new Vector3(isInverted ? -256 : 256, transform.localPosition.y, transform.localPosition.z);
        transform.DOLocalMoveX(isInverted ? 128 : -128, 1f).SetEase(Ease.OutSine);
    }

    public void Hide()
    {
        _cg.DOFade(0f, .5f);
    }
}
