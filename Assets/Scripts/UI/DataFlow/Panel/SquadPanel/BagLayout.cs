using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagLayout : MonoBehaviour
{
    [SerializeField] List<ItemLayout> _decksItemLayout;

    Sequence _shakeSeq;

    public void UpdateDeckItems(List<Item> decks)
    {
        for (int i = 0; i < _decksItemLayout.Count; i++)
        {
            if (i < decks.Count) _decksItemLayout[i].Init(decks[i], i);
            else _decksItemLayout[i].gameObject.SetActive(false);
        }
    }

    public void DoShakeRotate(Item newItem)
    {
        if (_shakeSeq.IsActive())
        {
            _shakeSeq.Kill(true);
            _shakeSeq = null;
        }

        _shakeSeq = DOTween.Sequence();

        foreach (ItemLayout item in _decksItemLayout)
        {
            if (newItem == item.Item) item.gameObject.SetActive(false);
            _shakeSeq.Join(item.gameObject.transform.DOShakeRotation(.4f, new Vector3(0, 0, 3), 18, 90, false, ShakeRandomnessMode.Harmonic));
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

        foreach (ItemLayout item in _decksItemLayout)
        {
            item.gameObject.SetActive(true);
            item.gameObject.transform.rotation = Quaternion.identity;
        }
    }
}
