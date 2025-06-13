using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestLayout : MonoBehaviour
{
    [SerializeField] TMP_Text _questName, _questDesc, _questAmount;
    [SerializeField] Slider _questSlider;
    [SerializeField] Image _questIco, _bg, _checkIco;
    [SerializeField] GameObject _sliderLayout;
    [SerializeField] RewardedAdsButton _adsLayout;

    [SerializeField] Color _activeColor, _finishedColor;
    DailyQuestData data;

    bool _questComplete;

    public bool QuestComplete { get => _questComplete; }

    public void Init(DailyQuestData questData)
    {
        data = questData;
        QuestDataRef dataRef = NakamaData.Instance.GetQuestDataRefByIds(data.id);


        Debug.Log(data.progress);
        _questName.text = dataRef.QuestName.GetLocalizedString(data.goal - data.progress);
        _questDesc.text = dataRef.QuestDesc.GetLocalizedString(data.goal - data.progress);

        _questComplete = data.progress >= data.goal;
        _checkIco.enabled = _questComplete;

        _bg.color = _questComplete ? _finishedColor : _activeColor;

        _questAmount.text = data.progress + "/" + data.goal;

        _questSlider.maxValue = data.goal;
        _questSlider.value = data.progress;

        _questIco.sprite = dataRef.QuestIco;

        if (_questComplete)
        {
            _sliderLayout.SetActive(true);
            _adsLayout.gameObject.SetActive(false);
        }
        else if (dataRef.QuestIds == QuestIds.watch_ad)
        {
            _sliderLayout.SetActive(false);
            _adsLayout.gameObject.SetActive(true);
            _adsLayout.Init();
        }
        else
        {
            _sliderLayout.SetActive(true);
            _adsLayout.gameObject.SetActive(false);
        }
    }

    public async void HandleOnWatchAdQuest()
    {
        await NakamaManager.Instance.NakamaQuest.ClaimAdQuest();

        _adsLayout.Disable();
    }
}
