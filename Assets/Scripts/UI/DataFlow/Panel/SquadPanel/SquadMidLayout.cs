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

                foreach (var blast in DataUtils.Instance.BlastCollection.deckBlasts)
                {
                    var id = uniqueBlast.IndexOf(DataUtils.Instance.GetBlastDataById(blast.data_id).id);
                    if (id == -1) uniqueBlast.Add(DataUtils.Instance.GetBlastDataById(blast.data_id).id);
                }

                foreach (var blast in DataUtils.Instance.BlastCollection.storedBlasts)
                {
                    var id = uniqueBlast.IndexOf(DataUtils.Instance.GetBlastDataById(blast.data_id).id);
                    if (id == -1) uniqueBlast.Add(DataUtils.Instance.GetBlastDataById(blast.data_id).id);
                }

                _totalPedia.text = uniqueBlast.Count + "/" + DataUtils.Instance.BlastPedia.Count;
                _totalAmount.text = DataUtils.Instance.BlastCollection.storedBlasts.Count.ToString();
                _titleTxt.text = _storedBlastTrad.GetLocalizedString();
                _currentIco.sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.Blast).sprite;
                _pediaIco.sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.BlastPedia).sprite;
                break;
            case SquadTabType.ITEM:

                _totalPedia.text = DataUtils.Instance.ItemCollection.storedItems.Count + DataUtils.Instance.ItemCollection.deckItems.Count + "/" + DataUtils.Instance.ItemPedia.Count;
                _totalAmount.text = DataUtils.Instance.ItemCollection.storedItems.Count.ToString();
                _titleTxt.text = _storedItemsTrad.GetLocalizedString();
                _currentIco.sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.Item).sprite;
                _pediaIco.sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.ItemPedia).sprite;
                break;
        }
    }
}
