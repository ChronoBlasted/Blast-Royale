using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    List<PediaBlastLayout> _pediaBlastLayouts = new List<PediaBlastLayout>();

    public override void Init()
    {
        base.Init();

        ClearChilds();
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
        _pediaBlastLayouts.Clear();

        foreach (var blast in allBlasts)
        {
            PediaBlastLayout pediaBlastLayout = Instantiate(_pediaBlastLayoutPrefab, _pediaBlastTransform);
            pediaBlastLayout.Init(blast);

            _pediaBlastLayouts.Add(pediaBlastLayout);
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

    public void UpdateMovePedia(List<Move> allMoves)
    {
        foreach (var item in allMoves)
        {
            PediaMoveLayout pediaBlastLayout = Instantiate(_moveLayoutPrefab, _moveTransform);
            pediaBlastLayout.Init(item);
        }
    }

    private void OnApplicationQuit()
    {
        ClearChilds();
    }

    private void ClearChilds()
    {
        foreach (Transform t in _pediaBlastTransform.transform)
        {
            Destroy(t.gameObject);
        }

        foreach (Transform t in _pediaItemTransform.transform)
        {
            Destroy(t.gameObject);
        }

        foreach (Transform t in _moveTransform.transform)
        {
            Destroy(t.gameObject);
        }
    }
}
