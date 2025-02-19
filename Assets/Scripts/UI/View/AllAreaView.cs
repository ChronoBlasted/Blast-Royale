using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AllAreaView : View
{
    [SerializeField] AreaLayout _areaLayoutPrefab;
    [SerializeField] Transform _areaContent;
    [SerializeField] Transform _areaTransform;
    [SerializeField] ScrollRect _scrollRect;

    List<AreaData> _allArea = new List<AreaData>();

    public override void Init()
    {
        base.Init();
    }

    public override void OpenView(bool _instant = false)
    {
        base.OpenView(_instant);

        UpdateScrollRect();
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
        _allArea.Clear();

        foreach (AreaData areaData in allArea)
        {
            AreaLayout currentAreaLayout = Instantiate(_areaLayoutPrefab, _areaTransform);
            currentAreaLayout.Init(areaData);

            _allArea.Add(areaData);
        }
    }

    public void UpdateScrollRect()
    {
        for (int i = 0; i < _allArea.Count; i++)
        {
            if (NakamaManager.Instance.NakamaUserAccount.LastWalletData[Currency.trophies.ToString()] >= _allArea[i].trophyRequired)
            {
                UIManager.ScrollToItem(_scrollRect, _areaContent, i);
                return;
            }
        }
    }
}
