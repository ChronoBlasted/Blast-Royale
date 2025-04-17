using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadPanel : Panel
{
    [Header("Deck Layout")]
    [SerializeField] NavBar _squadNavBar;
    [SerializeField] SquadLayout _squadDeckLayout;
    [SerializeField] BagLayout _bagDeckLayout;

    [Header("Mid Layout")]
    [SerializeField] SquadMidLayout _squadMidLayout;

    [Header("Stored Blast Layout")]
    [SerializeField] Transform _storedBlastTransform;
    [SerializeField] Transform _storedItemTransform;

    [Header("Stored Item Layout")]
    [SerializeField] BlastLayout _blastLayoutPrefab;
    [SerializeField] ItemLayout _itemLayoutPrefab;

    [Header("Solo Layout")]
    [SerializeField] GameObject _soloBlastGO;
    [SerializeField] BlastLayout _soloBlastLayout;

    [SerializeField] GameObject _soloItemGO;
    [SerializeField] ItemLayout _soloItemLayout;

    bool _isSwapMode = false;
    int _currentIndexStored;
    public bool IsSwapMode { get => _isSwapMode; set => _isSwapMode = value; }
    public int CurrentIndexStored { get => _currentIndexStored; set => _currentIndexStored = value; }
    public Transform StoredBlastTransform { get => _storedBlastTransform; }
    public Transform StoredItemTransform { get => _storedItemTransform; }

    public override void Init()
    {
        base.Init();
    }

    public override void OpenPanel()
    {
        base.OpenPanel();

        UIManager.Instance.MenuView.TopBar.ShowTopBar();

        _squadNavBar.Init();
    }

    public override void ClosePanel()
    {
        base.ClosePanel();

        QuitSoloBlast();
        QuitSoloItem(false);
    }

    public void SwitchToSoloBlast(Blast blast)
    {
        _soloBlastGO.SetActive(true);
        _soloBlastLayout.Init(blast);
        _storedBlastTransform.gameObject.SetActive(false);

        _squadDeckLayout.DoShakeRotate();
    }

    public void QuitSoloBlast(bool activeDefaultTransform = true)
    {
        _soloBlastGO.SetActive(false);
        _storedBlastTransform.gameObject.SetActive(activeDefaultTransform);

        _squadDeckLayout.StopShakeRotate();

        IsSwapMode = false;
    }

    public void SwitchToSoloItem(Item item)
    {
        _soloItemGO.SetActive(true);
        _soloItemLayout.Init(item);
        _storedItemTransform.gameObject.SetActive(false);

        _bagDeckLayout.DoShakeRotate();
    }


    public void QuitSoloItem(bool activeDefaultTransform = true)
    {
        _soloItemGO.SetActive(false);
        _storedItemTransform.gameObject.SetActive(activeDefaultTransform);

        _bagDeckLayout.StopShakeRotate();

        IsSwapMode = false;
    }

    public void UpdateStoredBlast(List<Blast> decks)
    {
        foreach (Transform child in _storedBlastTransform.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < decks.Count; i++)
        {
            var currentBlast = Instantiate(_blastLayoutPrefab, _storedBlastTransform);

            currentBlast.Init(decks[i], i);
        }
    }

    public void UpdateDeckBlast(List<Blast> decks)
    {
        _squadDeckLayout.UpdateDeckBlast(decks);
    }

    public void UpdateStoredItem(List<Item> items)
    {
        foreach (Transform child in _storedItemTransform.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < items.Count; i++)
        {
            var currentItem = Instantiate(_itemLayoutPrefab, _storedItemTransform);

            currentItem.Init(items[i], i);
        }
    }

    public void UpdateDeckItem(List<Item> decks)
    {
        _bagDeckLayout.UpdateDeckItems(decks);
    }

    public void UpdateMiddleTitle(SquadTabType tabType)
    {
        _squadMidLayout.UpdateData(tabType);
    }
}
