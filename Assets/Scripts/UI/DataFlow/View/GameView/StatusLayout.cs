using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusLayout : MonoBehaviour
{
    [SerializeField] Image _bg;
    [SerializeField] TMP_Text _text, _titleDescTxt, _descTxt;
    [SerializeField] CanvasGroup _cg;
    Status _currentStatus;
    ResourceData _statusData;

    Coroutine _closeCor;

    public void Init(Status newStatus)
    {
        _currentStatus = newStatus;

        _statusData = ResourceObjectHolder.Instance.GetResourceByType((ResourceType)_currentStatus);

        _bg.color = ColorManager.Instance.GetStatusColor(_currentStatus);
        _text.text = newStatus.ToString().Substring(0, 3);
    }

    public void HandleOpenStatusInfo()
    {
        _cg.DOFade(1f, 0.5f);

        _titleDescTxt.text = _statusData.Name.GetLocalizedString();
        _descTxt.text = _statusData.Desc.GetLocalizedString();

        if (_closeCor != null)
        {
            StopCoroutine(_closeCor);
            _closeCor = null;
        }

        _closeCor = StartCoroutine(CloseTooltip());
    }

    IEnumerator CloseTooltip()
    {
        yield return new WaitForSeconds(3f);

        _cg.DOFade(0, .5f);
    }
}
