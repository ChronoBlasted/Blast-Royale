using BaseTemplate.Behaviours;
using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public enum TravelEffect
{
    EXPLODE,
    LINEAR
}

public enum TextStyle
{
    H1,
    H2,
    H3,
    H4,
    H5,
    Normal,
    C1,
    C2,
    C3,
}

public class UIManager : MonoSingleton<UIManager>
{
    public Canvas MainCanvas;
    public RectTransform RectTransformPool;

    [Header("Views")]
    public MenuView MenuView;
    public GameView GameView;
    public EndViewPvE EndViewPve;
    public OpeningView OpeningView;
    public PediaView PediaView;
    public DailyRewardView DailyRewardView;
    public AreaLeaderboardView AreaLeaderboardView;
    public RegularLeaderboardView RegularLeaderboardView;
    public FriendView FriendView;
    public SettingView SettingView;
    public LoadingBattleView LoadingBattleView;
    public AllAreaView AllAreaView;
    public ErrorView ErrorView;
    public BlackShadeView BlackShadeView;

    [Header("Popups")]
    public ChangeBlastPopup ChangeBlastPopup;
    public BlastInfoPopup BlastInfoPopup;
    public MoveSelectorPopup MoveSelectorPopup;
    public ItemInfoPopup ItemInfoPopup;
    public ProfilePopup ProfilePopup;
    public LanguagePopup LanguagePopup;
    public RewardPopup RewardPopup;
    public LevelExpPopup LevelExpPopup;
    public LinkLogPopup LinkLogPopup;
    public ConfirmPopup ConfirmPopup;
    public WildBattleOfferPopup WildBattleOfferPopup;
    public QuestPopup QuestPopup;

    View _currentView;

    public void Init()
    {
        GameStateManager.Instance.OnGameStateChanged += HandleStateChange;

        InitView();

        ChangeView(OpeningView);
    }

    public void InitView()
    {
        MenuView.Init();
        GameView.Init();
        EndViewPve.Init();
        OpeningView.Init();
        SettingView.Init();
        OpeningView.Init();
        RegularLeaderboardView.Init();
        DailyRewardView.Init();
        PediaView.Init();
        FriendView.Init();
        LoadingBattleView.Init();
        AllAreaView.Init();
        ErrorView.Init();

        ChangeBlastPopup.Init();
        BlastInfoPopup.Init();
        MoveSelectorPopup.Init();
        ItemInfoPopup.Init();
        ProfilePopup.Init();
        LanguagePopup.Init();
        RewardPopup.Init();
        LevelExpPopup.Init();
        ConfirmPopup.Init();
        LinkLogPopup.Init();
        WildBattleOfferPopup.Init();

        BlackShadeView.HideBlackShade();
    }

    #region View

    public void ChangeView(View newPanel, bool _instant = false)
    {
        if (newPanel == _currentView) return;

        if (_currentView != null)
        {
            CloseView(_currentView);
        }

        _currentView = newPanel;

        _currentView.gameObject.SetActive(true);

        if (_instant) _currentView.OpenView(_instant);
        else _currentView.OpenView();

    }

    public void ChangeView(View newPanel)
    {
        if (newPanel == _currentView) return;

        if (_currentView != null)
        {
            CloseView(_currentView);
        }

        _currentView = newPanel;

        _currentView.gameObject.SetActive(true);
        _currentView.OpenView();
    }

    void CloseView(View newPanel)
    {
        newPanel.CloseView();
    }

    #endregion

    #region GameState

    void HandleStateChange(GameState newState)
    {
        switch (newState)
        {
            case GameState.MENU:
                HandleMenu();
                break;
            case GameState.GAME:
                HandleGame();
                break;
            case GameState.END:
                HandleEnd();
                break;
            case GameState.WAIT:
                HandleWait();
                break;
            default:
                break;
        }
    }

    void HandleMenu()
    {
        ChangeView(MenuView);
    }
    void HandleGame()
    {
        ChangeView(GameView);
    }
    void HandleEnd()
    {
        ChangeView(EndViewPve);
    }
    void HandleWait()
    {
    }


    #endregion

    public void DOTravelImage(TravelEffect travelEffect, Vector2 sizeOfImage, Sprite spriteOfImage, Transform startTransform, Transform endTransform, System.Action<bool> OnSpriteTravelStartOrFinish, float duration = 1, int amountToSpawn = 20)
    {
        StartCoroutine(DOTravelImageCor(travelEffect, sizeOfImage, spriteOfImage, startTransform, endTransform, OnSpriteTravelStartOrFinish, duration, amountToSpawn));
    }

    IEnumerator DOTravelImageCor(TravelEffect travelEffect, Vector2 sizeOfImage, Sprite spriteOfImage, Transform startTransform, Transform endTransform, System.Action<bool> OnSpriteTravelStartOrFinish, float duration, int amountToSpawn)
    {
        int tempAmount = 0;

        for (int i = 0; i < amountToSpawn; i++)
        {
            Vector3 worldPosition = endTransform.transform.position;
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

            Vector3 startWorldPos = startTransform.transform.position;
            Vector3 startScreenPos = Camera.main.WorldToScreenPoint(startWorldPos);

            Image ObjectToMove = PoolManager.Instance[ResourceType.BaseImg].Get().GetComponent<Image>();

            ObjectToMove.rectTransform.localScale = Vector3.one;
            ObjectToMove.rectTransform.sizeDelta = sizeOfImage;
            ObjectToMove.sprite = spriteOfImage;
            ObjectToMove.color = new Color(1, 1, 1, 0);

            if (OnSpriteTravelStartOrFinish != null) OnSpriteTravelStartOrFinish.Invoke(true);

            Tween GoToTween = null;
            Tween ExplodeTween = null;

            if (startTransform.GetComponent<RectTransform>() != null)
            {
                ObjectToMove.rectTransform.position = startWorldPos;

                ExplodeTween = ObjectToMove.rectTransform.DOMove(new Vector3(Random.Range(startTransform.transform.position.x - .25f
                        , startTransform.transform.position.x + .25f), Random.Range(startTransform.transform.position.y - .25f
                        , startTransform.transform.position.y + .25f), Random.Range(startTransform.transform.position.z - .25f
                        , startTransform.transform.position.z + .25f)), .5f);
            }
            else
            {
                ObjectToMove.rectTransform.position = startScreenPos;

                ExplodeTween = ObjectToMove.rectTransform.DOMove(
                    new Vector3(Random.Range(startScreenPos.x - 75, startScreenPos.x + 75),
                                Random.Range(startScreenPos.y - 75, startScreenPos.y + 75)), .5f);
            }


            if (endTransform.GetComponent<RectTransform>() != null)
            {
                GoToTween = ObjectToMove.rectTransform.DOMove(endTransform.transform.position, duration);
            }
            else
            {
                GoToTween = ObjectToMove.rectTransform.DOLocalMove(startScreenPos, duration);
            }

            switch (travelEffect)
            {
                case TravelEffect.EXPLODE:

                    DOTween.Sequence()
                        .Join(ObjectToMove.DOFade(1, .2f).SetEase(Ease.OutQuart))
                        .Join(ExplodeTween.SetEase(Ease.OutQuart))

                        .AppendInterval(.2f)

                        .Append(GoToTween.SetEase(Ease.InOutQuad))
                        .Join(ObjectToMove.DOFade(0, .2f).SetDelay(duration - .2f).SetEase(Ease.OutQuart))

                        .OnComplete(() =>
                        {
                            if (OnSpriteTravelStartOrFinish != null) OnSpriteTravelStartOrFinish.Invoke(false);

                            PoolManager.Instance[ResourceType.BaseImg].Release(ObjectToMove.gameObject);

                            tempAmount++;
                        });

                    break;


                case TravelEffect.LINEAR:

                    DOTween.Sequence()
                           .Join(GoToTween.SetEase(Ease.Linear))
                           .Join(ObjectToMove.DOFade(1, .1f).SetEase(Ease.OutQuart))

                           .Join(ObjectToMove.DOFade(0, .2f).SetDelay(duration - .2f).SetEase(Ease.OutQuart))

                           .OnComplete(() =>
                           {
                               if (OnSpriteTravelStartOrFinish != null) OnSpriteTravelStartOrFinish.Invoke(false);

                               PoolManager.Instance[ResourceType.BaseImg].Release(ObjectToMove.gameObject);

                               tempAmount++;
                           });
                    break;
            }
            yield return new WaitForSeconds(Random.Range(.05f, .1f));
        }
    }

    public static void ResetScroll(ScrollRect scrollRect)
    {
        scrollRect.normalizedPosition = new Vector2(scrollRect.normalizedPosition.x, 1);
    }

    public static void ScrollToItemY(ScrollRect scrollRect, Transform content, int indexToScroll)
    {
        float step = 1f / (content.childCount - 1);

        scrollRect.normalizedPosition = new Vector2(scrollRect.normalizedPosition.x, step * indexToScroll);
    }

    public static void ScrollToItemX(ScrollRect scrollRect, Transform content, int indexToScroll)
    {
        float step = 1f / (content.childCount - 1);

        scrollRect.normalizedPosition = new Vector2(step * indexToScroll, scrollRect.normalizedPosition.y);
    }

    public void DoSmoothTextInt(TMP_Text textToUpdate, int baseInt, int destinationInt, string prefix = "", float duration = 1f, Ease ease = Ease.OutSine)
    {
        if (destinationInt == baseInt + 1)
        {
            textToUpdate.text = prefix + GetFormattedInt(destinationInt);

            return;
        }

        DOVirtual.Int(baseInt, destinationInt, duration, x =>
        {
            textToUpdate.text = prefix + GetFormattedInt(x);
        }).SetEase(ease);
    }

    public static string GetTradByKey(string key)
    {
        var stringResult = LocalizationSettings.StringDatabase.GetLocalizedString("TranslationStringTable", key);
        return stringResult;
    }

    public static string GetFormattedInt(int amount)
    {
        return amount.ToString("#,0");
    }
}
