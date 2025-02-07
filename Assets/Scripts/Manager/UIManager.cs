using BaseTemplate.Behaviours;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class UIManager : MonoSingleton<UIManager>
{
    [field: SerializeField] public Canvas MainCanvas { get; protected set; }
    [field: SerializeField] public MenuView MenuView { get; protected set; }
    [field: SerializeField] public GameView GameView { get; protected set; }
    [field: SerializeField] public EndView EndView { get; protected set; }
    [field: SerializeField] public OpeningView OpeningView { get; protected set; }
    [field: SerializeField] public PediaView PediaView { get; protected set; }
    [field: SerializeField] public FriendView FriendView { get; protected set; }
    [field: SerializeField] public AllAreaView AllAreaView { get; protected set; }
    [field: SerializeField] public LeaderboardView LeaderboardView { get; protected set; }
    [field: SerializeField] public LoadingBattleView LoadingBattleView { get; protected set; }
    [field: SerializeField] public DailyRewardView DailyRewardView { get; protected set; }
    [field: SerializeField] public BlastInfoPopup BlastInfoPopup { get; protected set; }
    [field: SerializeField] public MoveSelectorPopup MoveSelectorPopup { get; protected set; }
    [field: SerializeField] public ConfirmPopup ConfirmPopup { get; protected set; }

    [Header("Black Shade Ref")]
    [SerializeField] Image _blackShadeImg;
    [field: SerializeField] public Button BlackShadeButton { get; protected set; }

    [Header("Popup Ref")]
    // Empty

    View _currentView;
    Tweener _blackShadeTweener;

    public void Init()
    {
        GameManager.Instance.OnGameStateChanged += HandleStateChange;

        InitView();

        ChangeView(OpeningView);
    }

    public void InitView()
    {
        MenuView.Init();
        GameView.Init();
        EndView.Init();
        OpeningView.Init();

        HideBlackShade();
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
        ChangeView(EndView);
    }
    void HandleWait()
    {
    }


    #endregion

    public void ShowBlackShade(UnityAction _onClickAction)
    {
        if (_blackShadeTweener.IsActive()) _blackShadeTweener.Kill();

        _blackShadeTweener = _blackShadeImg.DOFade(.5f, .1f);

        _blackShadeImg.raycastTarget = true;

        BlackShadeButton.onClick.AddListener(_onClickAction);
    }

    public void HideBlackShade(bool _instant = true)
    {
        if (_blackShadeTweener.IsActive()) _blackShadeTweener.Kill();

        if (_instant) _blackShadeTweener = _blackShadeImg.DOFade(0f, 0);
        else _blackShadeTweener = _blackShadeImg.DOFade(0f, .1f);

        _blackShadeImg.raycastTarget = false;

        BlackShadeButton.onClick.RemoveAllListeners();
    }

    public static void ScrollToItem(ScrollRect scrollRect, Transform content, int indexToScroll)
    {
        float step = 1f / (content.childCount - 1);

        scrollRect.normalizedPosition = new Vector2(scrollRect.normalizedPosition.x, step * indexToScroll);
    }

    public static string GetTradByKey(string key)
    {
        var stringResult = LocalizationSettings.StringDatabase.GetLocalizedString("TranslationStringTable", key);
        return stringResult;
    }

    public static string GetFormattedInt(float amount)
    {
        return amount.ToString("#,0");
    }
}
