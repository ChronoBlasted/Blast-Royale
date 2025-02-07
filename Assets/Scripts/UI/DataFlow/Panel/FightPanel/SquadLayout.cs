using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadLayout : MonoBehaviour
{
    [SerializeField] List<BlastLayout> _decksBlastLayout;

    Sequence _shakeSeq;

    public void UpdateDeckBlast(List<Blast> decks)
    {
        for (int i = 0; i < _decksBlastLayout.Count; i++)
        {
            if (i < decks.Count) _decksBlastLayout[i].Init(decks[i], i);
            else _decksBlastLayout[i].gameObject.SetActive(false);
        }
    }

    public void DoShakeRotate()
    {
        if (_shakeSeq.IsActive())
        {
            _shakeSeq.Kill(true);
            _shakeSeq = null;
        }

        _shakeSeq = DOTween.Sequence();

        foreach (BlastLayout blast in _decksBlastLayout)
        {
            _shakeSeq.Join(blast.gameObject.transform.DOShakeRotation(.4f, new Vector3(0, 0, 3), 18, 90, false, ShakeRandomnessMode.Harmonic));
        }

        _shakeSeq.SetLoops(-1, LoopType.Restart);
        _shakeSeq.Play();
    }

    public void StopShakeRotate()
    {
        if (_shakeSeq.IsActive())
        {
            _shakeSeq.Kill(true);
            _shakeSeq = null;
        }

        foreach (BlastLayout blast in _decksBlastLayout)
        {
            blast.gameObject.transform.rotation = Quaternion.identity;
        }
    }
}
