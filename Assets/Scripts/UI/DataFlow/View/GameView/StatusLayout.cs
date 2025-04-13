using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusLayout : MonoBehaviour
{
    [SerializeField] Image _bg;
    [SerializeField] TMP_Text _text;

    public void Init(Status newStatus)
    {
        _bg.color = ColorManager.Instance.GetStatusColor(newStatus);
        _text.text = newStatus.ToString().Substring(0, 3);
    }
}
