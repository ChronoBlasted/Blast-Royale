using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadMiniPanel : Panel
{
    [SerializeField] List<BlastMiniSquadLayout> _blastMiniSquadLayouts;


    public override void OpenPanel()
    {
        base.OpenPanel();

        for (int i = 0; i < _blastMiniSquadLayouts.Count; i++)
        {
            _blastMiniSquadLayouts[i].UpdateUI();
        }
    }

    public override void ClosePanel()
    {
        base.ClosePanel();
    }

    public void UpdateBlasts(List<Blast> blasts)
    {
        for (int i = 0; i < _blastMiniSquadLayouts.Count; i++)
        {
            _blastMiniSquadLayouts[i].Init(blasts[i]);
        }
    }
}
