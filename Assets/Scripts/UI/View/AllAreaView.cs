using DG.Tweening;
using Nakama;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AllAreaView : View
{
    [SerializeField] ScrollRect _scrollRect;
    [SerializeField] Transform _areaContent;
    [SerializeField] AreaLayout _areaLayoutPrefab;
    [SerializeField] SnapToItem _snapToItem;

    [SerializeField] ScrollStepIndicator _scrollStepIndicator;
    [SerializeField] Transform _visualIndicatorTransform;
    [SerializeField] CanvasGroup _arrowCG;

    List<AreaLayout> _allAreaLayout = new List<AreaLayout>();
    List<ScrollStepIndicator> _visualIndicators = new List<ScrollStepIndicator>();

    int _lastAreaIndex = 0;
    int _lastVisualIndicatorIndex = 0;
    ScrollStepIndicator _selectedStepIndicator;

    Coroutine _arrowCor;

    public override void Init()
    {
        base.Init();
    }

    public override void OpenView(bool _instant = false)
    {
        base.OpenView(_instant);

        UpdateScrollRect();
        UpdateVisualIndicator();
    }

    public override void CloseView()
    {
        base.CloseView();
    }

    public void Close()
    {
        UIManager.Instance.ChangeView(UIManager.Instance.MenuView);
    }

    public void UpdateAllArea(List<AreaData> allArea)
    {
        _allAreaLayout.Clear();
        _visualIndicators.Clear();

        foreach (Transform t in _areaContent)
        {
            Destroy(t.gameObject);
        }

        foreach (Transform t in _visualIndicatorTransform)
        {
            Destroy(t.gameObject);
        }

        for (int i = 0; i < allArea.Count; i++)
        {
            AreaLayout currentAreaLayout = Instantiate(_areaLayoutPrefab, _areaContent);
            currentAreaLayout.Init(allArea[i]);

            _allAreaLayout.Add(currentAreaLayout);

            _visualIndicators.Add(Instantiate(_scrollStepIndicator, _visualIndicatorTransform));
        }
    }

    public void UpdateScrollRect()
    {
        UIManager.ScrollToItemX(_scrollRect, _areaContent, _lastAreaIndex);
    }

    public void UpdateVisualIndicator()
    {
        _arrowCG.alpha = 0;

        _visualIndicators[_lastVisualIndicatorIndex].SetUnActive();

        _lastVisualIndicatorIndex = _snapToItem.CurrentIndex;

        _visualIndicators[_lastVisualIndicatorIndex].SetActive();

        if (_arrowCor != null)
        {
            StopCoroutine(_arrowCor);
            _arrowCor = null;
        }

        _arrowCor = StartCoroutine(ShowArrow());
    }

    IEnumerator ShowArrow()
    {
        yield return new WaitForSeconds(.5f);

        _arrowCG.DOFade(1, .5f);
    }

    public async void HandleOnSelectArea(int index)
    {
        try
        {
            await NakamaManager.Instance.NakamaArea.SelectArea(_allAreaLayout[index].Data.id);

            SelectArea(index);
        }
        catch (ApiResponseException e) { Debug.LogError(e); }
    }

    private void SelectArea(int index)
    {
        _allAreaLayout[_lastAreaIndex].Unselect();
        if (_selectedStepIndicator != null) _selectedStepIndicator.SetUnSelected();

        _lastAreaIndex = index;
        _selectedStepIndicator = _visualIndicators[_lastAreaIndex];

        _allAreaLayout[_lastAreaIndex].Select();
        _selectedStepIndicator.SetSelected();

        AreaDataRef dataRef = NakamaData.Instance.GetAreaDataRef(_allAreaLayout[_lastAreaIndex].Data.id);

        UIManager.Instance.MenuView.FightPanel.AreaLayoutFightPanel.UpdateArea(dataRef.Sprite);
    }

    public void SetArea(int areaID)
    {
        for (int i = 0; i < _allAreaLayout.Count; i++)
        {
            if (_allAreaLayout[i].Data.id == areaID)
            {
                SelectArea(i);
                return;
            }
        }
    }


    public void HandleOnOpenAreaLeaderboard()
    {
        UIManager.Instance.AreaLeaderboardView.CurrentIndex = _snapToItem.CurrentIndex;

        UIManager.Instance.ChangeView(UIManager.Instance.AreaLeaderboardView);
    }
}
