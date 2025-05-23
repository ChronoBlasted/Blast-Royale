using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MoveMiniPanel : Panel
{
    [SerializeField] List<MoveLayout> moveInBatleLayouts;

    NakamaData _dataUtils;

    Blast _blast;
    public override void Init()
    {
        base.Init();

        _dataUtils = NakamaData.Instance;
    }

    public override void OpenPanel()
    {
        base.OpenPanel();

        for (int i = 0; i < moveInBatleLayouts.Count; i++)
        {
            if (i < _blast.activeMoveset.Count) moveInBatleLayouts[i].UpdateUI();
        }
    }

    public override void ClosePanel()
    {
        base.ClosePanel();
    }

    public void UpdateAttack(Blast blastAttacks)
    {
        _blast = blastAttacks;

        for (int i = 0; i < moveInBatleLayouts.Count; i++)
        {
            moveInBatleLayouts[i].gameObject.SetActive(i < _blast.activeMoveset.Count);
            if (i < _blast.activeMoveset.Count) moveInBatleLayouts[i].Init(_dataUtils.GetMoveById(_blast.activeMoveset[i]), _blast, i);
        }
    }
}
