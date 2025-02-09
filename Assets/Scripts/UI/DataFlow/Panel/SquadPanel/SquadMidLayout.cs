using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class SquadMidLayout : MonoBehaviour
{
    [SerializeField] TMP_Text _totalAmount, _titleTxt, _totalPedia;
    [SerializeField] Image _currentIco, _pediaIco;

    [SerializeField] LocalizedString _storedBlastTrad, _storedItemsTrad;
    public void UpdateData(SquadTabType type)
    {
        switch (type)
        {
            case SquadTabType.BLAST:

                List<int> uniqueBlast = new List<int>();

                foreach (var blast in NakamaData.Instance.BlastCollection.deckBlasts)
                {
                    var id = uniqueBlast.IndexOf(NakamaData.Instance.GetBlastDataById(blast.data_id).id);
                    if (id == -1) uniqueBlast.Add(NakamaData.Instance.GetBlastDataById(blast.data_id).id);
                }

                foreach (var blast in NakamaData.Instance.BlastCollection.storedBlasts)
                {
                    var id = uniqueBlast.IndexOf(NakamaData.Instance.GetBlastDataById(blast.data_id).id);
                    if (id == -1) uniqueBlast.Add(NakamaData.Instance.GetBlastDataById(blast.data_id).id);
                }

                _totalPedia.text = uniqueBlast.Count + "/" + NakamaData.Instance.BlastPedia.Count;
                _totalAmount.text = NakamaData.Instance.BlastCollection.storedBlasts.Count.ToString();
                _titleTxt.text = _storedBlastTrad.GetLocalizedString();
                _currentIco.sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.Blast).Sprite;
                _pediaIco.sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.BlastPedia).Sprite;
                break;
            case SquadTabType.ITEM:

                _totalPedia.text = NakamaData.Instance.ItemCollection.storedItems.Count + NakamaData.Instance.ItemCollection.deckItems.Count + "/" + NakamaData.Instance.ItemPedia.Count;
                _totalAmount.text = NakamaData.Instance.ItemCollection.storedItems.Count.ToString();
                _titleTxt.text = _storedItemsTrad.GetLocalizedString();
                _currentIco.sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.Item).Sprite;
                _pediaIco.sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.ItemPedia).Sprite;
                break;
        }
    }
}
