using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestLayout : MonoBehaviour
{
    [SerializeField] TMP_Text _questName, _questDesc, _questAmount;
    [SerializeField] Slider _questSlider;
    [SerializeField] Image _questIco, _bg, _checkIco;
    [SerializeField] GameObject _adsLayout, _sliderLayout;

    [SerializeField] Color _activeColor, _finishedColor;
    DailyQuestData data;

    public void Init(DailyQuestData questData)
    {
        data = questData;
        QuestDataRef dataRef = NakamaData.Instance.GetQuestDataRefByIds(data.id);

        _questName.text = dataRef.QuestName.GetLocalizedString();
        _questDesc.text = dataRef.QuestDesc.GetLocalizedString();

        _checkIco.enabled = data.progress >= data.goal;

        _bg.color = data.progress >= data.goal ? _finishedColor : _activeColor;


        _questAmount.text = data.progress + "/" + data.goal;

        _questSlider.maxValue = data.goal;
        _questSlider.value = data.progress;

        _questIco.sprite = dataRef.QuestIco;

        _adsLayout.SetActive(dataRef.QuestIds == QuestIds.watch_ad);
        _sliderLayout.SetActive(dataRef.QuestIds != QuestIds.watch_ad);
    }

    public async void HandleOnWatchAdQuest()
    {
        await NakamaManager.Instance.NakamaQuest.ClaimAdQuest();
    }
}
