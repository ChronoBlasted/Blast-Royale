using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PediaView : View
{
    [SerializeField] PediaBlastLayout _pediaBlastLayoutPrefab;
    [SerializeField] Transform _pediaBlastTransform;

    [SerializeField] PediaItemLayout _pediaItemLayoutPrefab;
    [SerializeField] Transform _pediaItemTransform;

    [SerializeField] PediaMoveLayout _moveLayoutPrefab;
    [SerializeField] Transform _moveTransform;

    [SerializeField] NavBar _pediaNavBar;

    public override void Init()
    {
        base.Init();
    }

    public override void OpenView(bool _instant = false)
    {
        _pediaNavBar.Init();

        base.OpenView(_instant);
    }

    public override void CloseView()
    {
        base.CloseView();
    }

    public void Close()
    {
        UIManager.Instance.ChangeView(UIManager.Instance.MenuView);
    }

    public void UpdateBlastPedia(List<BlastData> allBlasts)
    {
        foreach (Transform t in _pediaBlastTransform.transform)
        {
            Destroy(t.gameObject);
        }

        foreach (var blast in allBlasts)
        {
            PediaBlastLayout pediaBlastLayout = Instantiate(_pediaBlastLayoutPrefab, _pediaBlastTransform);
            pediaBlastLayout.Init(blast);
        }
    }

    public void UpdateItemPedia(List<ItemData> allItems)
    {
        foreach (var item in allItems)
        {
            PediaItemLayout pediaBlastLayout = Instantiate(_pediaItemLayoutPrefab, _pediaItemTransform);
            pediaBlastLayout.Init(item);
        }
    }

    public void UpdateMovePedia(List<Move> allItems)
    {
        foreach (var item in allItems)
        {
            PediaMoveLayout pediaBlastLayout = Instantiate(_moveLayoutPrefab, _moveTransform);
            pediaBlastLayout.Init(item);
        }
    }
}
