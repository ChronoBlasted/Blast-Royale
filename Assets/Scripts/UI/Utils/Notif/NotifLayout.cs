using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotifLayout : MonoBehaviour
{
    [SerializeField] Image _notifImage;
    [SerializeField] TMP_Text _notifText;

    public void Activate(int amountIndex)
    {
        gameObject.SetActive(true);

        if (amountIndex <= 0)
        {
            _notifText.text = "";
        }
        else
        {
            _notifText.text = amountIndex.ToString();
        }
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
