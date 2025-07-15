using BaseTemplate.Behaviours;
using Nakama;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class PvEBattleManager : MonoSingleton<PvEBattleManager>
{
    bool _isMatchWin;

    // Cache
    GameView _gameView;
    EndView _endView;
    NakamaPvEBattle _serverBattle;
    NakamaUserAccount _userAccount;
    NakamaData _dataUtils;

    // Data
    int _indexProgression;
    int _blastDefeated;
    int _blastCatched;
    int _bossEncounter;
    int _shinyEncounter;

    int _coinGenerated;
    int _gemGenerated;

    public bool BonusAds = false;

    List<Blast> _playerSquads = new List<Blast>();
    List<Item> _playerItems = new List<Item>();
    PlayerBattleInfo _playerMeInfo;
    PlayerBattleInfo _playerWildInfo;
    Blast _nextWildBlast;
    List<Offer> _currentOffers;

    Meteo _meteo;

    // Logic
    TurnStateData _turnStateData;
    TurnAction _playerAction, _wbAction;

    public List<Offer> PvEBattleReward;
    public List<Blast> PlayerSquads => _playerSquads;
    public List<Item> PlayerItems => _playerItems;
    public Blast WildBlast => _playerWildInfo.ActiveBlast;
    public Meteo Meteo => _meteo;

    public TurnAction PlayerAction { get => _playerAction; }
    public TurnAction WbAction { get => _wbAction; }
    public Blast PlayerBlast { get => _playerMeInfo.ActiveBlast; }
    public int BlastDefeated { get => _blastDefeated; }
    public int BlastCatched { get => _blastCatched; }
    public int ShinyEncounter { get => _shinyEncounter; }
    public int BossEncounter { get => _bossEncounter; }
    public int IndexProgression { get => _indexProgression; }

    public void Init()
    {
        _gameView = UIManager.Instance.GameView;
        _endView = UIManager.Instance.EndView;
        _serverBattle = NakamaManager.Instance.NakamaPvEBattle;
        _userAccount = NakamaManager.Instance.NakamaUserAccount;
        _dataUtils = NakamaData.Instance;
    }

    public async void StartBattle(StartStateData startData)
    {
        _playerSquads.Clear();
        _playerItems.Clear();

        foreach (var blast in _userAccount.LastBlastCollection.deckBlasts)
            _playerSquads.Add(new Blast(blast.uuid, blast.data_id, blast.exp, blast.iv, blast.activeMoveset, blast.boss, blast.shiny));

        foreach (var item in _userAccount.LastItemCollection.deckItems)
            _playerItems.Add(new Item(item.data_id, item.amount));

        _playerMeInfo = new PlayerBattleInfo(_userAccount.Username, BlastOwner.Me, _playerSquads[0], _playerSquads, _playerItems);

        SetNewWildBlast(startData.newBlastData);

        _playerWildInfo = new PlayerBattleInfo("", BlastOwner.Wild, _nextWildBlast, new List<Blast> { _nextWildBlast }, null);

        _gameView.PlayerHUD.BlastInWorld.PlatformLayout.Init();
        _gameView.OpponentHUD.BlastInWorld.PlatformLayout.Init();

        _indexProgression = 1;
        _blastDefeated = 0;
        _bossEncounter = 0;
        _shinyEncounter = 0;
        _blastCatched = 0;
        _coinGenerated = 0;
        _gemGenerated = 0;

        _meteo = NakamaLogic.GetEnumFromIndex<Meteo>((int)startData.meteo);
        _gameView.SetMeteo(_meteo);

        PvEBattleReward.Clear();

        _gameView.UpdateStateProgressLayout(false);
        _gameView.SetProgression(_indexProgression);
        _gameView.ExpProgressionLayout.SetSprite(_dataUtils.GetBlastDataRef(_playerMeInfo.ActiveBlast.data_id).Sprite);

        _gameView.DialogLayout.UpdateText("");
        _gameView.DialogLayout.Hide();

        _gameView.PlayerHUD.Init(_playerMeInfo.ActiveBlast);

        _gameView.AttackPanel.UpdateAttack(_playerMeInfo.ActiveBlast);

        _gameView.BagPanel.HandleOnPvPBattle(false);
        _gameView.BagPanel.UpdateItem(_playerItems);

        _gameView.SquadPanel.UpdateBlasts(_playerSquads);
        UIManager.Instance.ChangeBlastPopup.UpdateData(_playerSquads);

        _ = _gameView.PlayerHUD.ComeBackBlast(true);
        _ = _gameView.OpponentHUD.ComeBackBlast(true);

        GameStateManager.Instance.UpdateStateToGame();

        UIManager.Instance.GameView.ShowSpawnBlast();

        await FrameWaiter.WaitForEndOfFrameAsync();

        CameraManager.Instance.SetCameraZoom(10);

        CameraManager.Instance.ResetCamera(1f);

        await ShowWildBlast();

        await _gameView.PlayerHUD.ThrowBlast();

        _serverBattle.PlayerReady();
    }

    public void SetNewWildBlast(NewBlastData newBlastData)
    {
        var _wildBlast = new Blast("", newBlastData.id, newBlastData.exp, newBlastData.iv, newBlastData.activeMoveset, newBlastData.boss, newBlastData.shiny);

        _nextWildBlast = _wildBlast;
    }

    public async Task ShowWildBlast()
    {
        _playerWildInfo = new PlayerBattleInfo("", BlastOwner.Wild, _nextWildBlast, new List<Blast> { _nextWildBlast }, null);

        _gameView.OpponentHUD.Init(_nextWildBlast);

        _ = _gameView.OpponentHUD.ComeBackBlast(true);
        await _gameView.OpponentHUD.ThrowBlast();
    }

    public void StartNewTurn()
    {
        _playerAction = new TurnAction();
        _wbAction = new TurnAction();
        _gameView.DialogLayout.Hide();
        _gameView.ShowNavBar();
        _gameView.ResetTab();
    }

    public void WaitForOpponent()
    {
        _gameView.DisablePanels();
        _gameView.HideNavBar();
    }

    public async void PlayTurn(TurnStateData turnState)
    {
        _turnStateData = turnState;

        UpdateOpponentTurn(_turnStateData);

        _gameView.CloseAllPanel();

        bool isPlayerFirst = NakamaLogic.CompareActionPriorities(_playerAction.TurnType, _wbAction.TurnType);

        if (_playerAction.TurnType == TurnType.Attack && _wbAction.TurnType == TurnType.Attack)
        {
            var playerMove = _dataUtils.GetMoveById(PlayerBlast.activeMoveset[_playerAction.MoveIndex]);
            var wildMove = _dataUtils.GetMoveById(WildBlast.activeMoveset[_wbAction.MoveIndex]);

            isPlayerFirst = playerMove.priority > wildMove.priority ||
                            (playerMove.priority == wildMove.priority && NakamaLogic.GetFasterBlast(PlayerBlast, WildBlast));
        }

        bool battleStop = false;

        if (isPlayerFirst)
        {
            var gameLogicContextPlayer = new GameLogicContext(
                attacker: _playerMeInfo.ActiveBlast,
                defender: _playerWildInfo.ActiveBlast,
                players: new List<PlayerBattleInfo> { _playerMeInfo, _playerWildInfo },
                moveIndex: _playerAction.MoveIndex,
                moveDamage: _turnStateData.p1MoveDamage,
                moveEffects: _turnStateData.p1MoveEffects,
                itemIndex: _playerAction.ItemIndex,
                selectedBlastIndex: _playerAction.SelectedBlastIndex,
                isCatched: _turnStateData.catched
            );

            var playerHandler = TurnHandlerFactory.CreateHandler(_playerAction.TurnType, gameLogicContextPlayer, _gameView, _dataUtils);
            battleStop = await playerHandler.HandleTurn();
            if (battleStop)
            {
                await StopTurnHandler();
                return;
            }

            var gameLogicContextWild = new GameLogicContext(
                attacker: _playerWildInfo.ActiveBlast,
                defender: _playerMeInfo.ActiveBlast,
                players: new List<PlayerBattleInfo> { _playerMeInfo, _playerWildInfo },
                moveIndex: _turnStateData.p2MoveIndex,
                moveDamage: _turnStateData.p2MoveDamage,
                moveEffects: _turnStateData.p2MoveEffects,
                itemIndex: _wbAction.ItemIndex,
                selectedBlastIndex: _wbAction.SelectedBlastIndex,
                isCatched: false
            );

            var wildHandler = TurnHandlerFactory.CreateHandler(_wbAction.TurnType, gameLogicContextWild, _gameView, _dataUtils);
            battleStop = await wildHandler.HandleTurn();
            if (battleStop)
            {
                await StopTurnHandler();
                return;
            }
        }
        else
        {
            var gameLogicContextWild = new GameLogicContext(
                attacker: _playerWildInfo.ActiveBlast,
                defender: _playerMeInfo.ActiveBlast,
                players: new List<PlayerBattleInfo> { _playerMeInfo, _playerWildInfo },
                moveIndex: _turnStateData.p2MoveIndex,
                moveDamage: _turnStateData.p2MoveDamage,
                moveEffects: _turnStateData.p2MoveEffects,
                itemIndex: _wbAction.ItemIndex,
                selectedBlastIndex: _wbAction.SelectedBlastIndex,
                isCatched: false
            );

            var wildHandler = TurnHandlerFactory.CreateHandler(_wbAction.TurnType, gameLogicContextWild, _gameView, _dataUtils);
            battleStop = await wildHandler.HandleTurn();
            if (battleStop)
            {
                await StopTurnHandler();
                return;
            }

            var gameLogicContextPlayer = new GameLogicContext(
                attacker: _playerMeInfo.ActiveBlast,
                defender: _playerWildInfo.ActiveBlast,
                players: new List<PlayerBattleInfo> { _playerMeInfo, _playerWildInfo },
                moveIndex: _playerAction.MoveIndex,
                moveDamage: _turnStateData.p1MoveDamage,
                moveEffects: _turnStateData.p1MoveEffects,
                itemIndex: _playerAction.ItemIndex,
                selectedBlastIndex: _playerAction.SelectedBlastIndex,
                isCatched: _turnStateData.catched
            );

            var playerHandler = TurnHandlerFactory.CreateHandler(_playerAction.TurnType, gameLogicContextPlayer, _gameView, _dataUtils);
            battleStop = await playerHandler.HandleTurn();
            if (battleStop)
            {
                await StopTurnHandler();
                return;
            }
        }

        if (_playerMeInfo.ActiveBlast.status != Status.None)
        {
            var statusHandler = TurnHandlerFactory.CreateHandler(TurnType.Status,
                new GameLogicContext(
                    attacker: _playerMeInfo.ActiveBlast,
                    defender: _playerWildInfo.ActiveBlast,
                    players: new List<PlayerBattleInfo> { _playerMeInfo, _playerWildInfo },
                    moveIndex: -1,
                    moveDamage: 0,
                    moveEffects: null,
                    itemIndex: -1,
                    selectedBlastIndex: -1,
                    isCatched: false
                ),
                _gameView, _dataUtils
            );
            battleStop = await statusHandler.HandleTurn();
            if (battleStop)
            {
                await StopTurnHandler();
                return;
            }
        }

        if (_playerWildInfo.ActiveBlast.status != Status.None)
        {
            var statusHandler = TurnHandlerFactory.CreateHandler(TurnType.Status,
                new GameLogicContext(
                    attacker: _playerWildInfo.ActiveBlast,
                    defender: _playerMeInfo.ActiveBlast,
                    players: new List<PlayerBattleInfo> { _playerMeInfo, _playerWildInfo },
                    moveIndex: -1,
                    moveDamage: 0,
                    moveEffects: null,
                    itemIndex: -1,
                    selectedBlastIndex: -1,
                    isCatched: false
                ),
                _gameView, _dataUtils
            );
            battleStop = await statusHandler.HandleTurn();
            if (battleStop)
            {
                await StopTurnHandler();
                return;
            }
        }

        await StopTurnHandler();
    }

    public async Task StopTurnHandler()
    {
        if (NakamaLogic.IsBlastAlive(_playerWildInfo.ActiveBlast) == false || _turnStateData.catched)
        {
            _indexProgression++;

            if (_turnStateData.catched)
            {
                Offer newBlast = new Offer
                {
                    type = OfferType.Blast,
                    blast = _playerWildInfo.ActiveBlast,
                };

                PvEBattleReward.Add(newBlast);

                _blastCatched++;
            }
            else
            {
                _blastDefeated++;

                if (_playerWildInfo.ActiveBlast.boss) _bossEncounter++;
                if (_playerWildInfo.ActiveBlast.shiny) _shinyEncounter++;
            }

            _coinGenerated += 200;

            await Task.Delay(500);

            _gameView.AddProgress();

            await Task.Delay(2000);

            if (_indexProgression % 5 == 0 && _indexProgression % 10 != 0)
            {
                ShowOffers();
            }
            else
            {
                await ShowWildBlast();
            }
        }
        else if (NakamaLogic.IsAllBlastFainted(_playerMeInfo.Blasts))
        {
            PlayerLeave(false);
        }

        EndTurn();

        bool isOfferStep = _indexProgression % 5 == 0 && _indexProgression % 10 != 0;

        if (isOfferStep == false && NakamaLogic.IsBlastAlive(_playerMeInfo.ActiveBlast))
        {
            NakamaManager.Instance.NakamaPvEBattle.PlayerReady();
        }
    }


    public void SetNewOffers(List<Offer> newOffers)
    {
        _currentOffers = newOffers;

        UIManager.Instance.WildBattleOfferPopup.Init(newOffers);
    }

    public void ShowOffers()
    {
        UIManager.Instance.WildBattleOfferPopup.OpenPopup(true, false);
    }

    public void EndTurn()
    {
        if (NakamaLogic.IsBlastAlive(_playerMeInfo.ActiveBlast)) _playerMeInfo.ActiveBlast.Mana = NakamaLogic.Instance.CalculateStaminaRecovery(_playerMeInfo.ActiveBlast.MaxMana, _playerMeInfo.ActiveBlast.Mana, false);
        if (NakamaLogic.IsBlastAlive(_playerWildInfo.ActiveBlast)) _playerWildInfo.ActiveBlast.Mana = NakamaLogic.Instance.CalculateStaminaRecovery(_playerWildInfo.ActiveBlast.MaxMana, _playerWildInfo.ActiveBlast.Mana, false);

        _gameView.EndTurn(_playerMeInfo.ActiveBlast, _playerWildInfo.ActiveBlast);
    }

    private void UpdateOpponentTurn(TurnStateData turnState)
    {
        _wbAction.TurnType = turnState.p2TurnType;

        if (_wbAction.TurnType == TurnType.Attack)
        {
            _wbAction.MoveIndex = turnState.p2MoveIndex;
            _wbAction.MoveDamage = turnState.p2MoveDamage;
            _wbAction.MoveEffects = turnState.p2MoveEffects;
        }
    }

    public void MatchEnd(string data)
    {
        if (bool.TryParse(data, out var result))
        {
            _isMatchWin = result;
            _endView.UpdateEndGame(_isMatchWin);
        }
    }

    #region TurnAction Handlers

    public async void PlayerAttack(int indexAttack)
    {
        try
        {
            SetPlayerActionAttack(indexAttack);
            await _serverBattle.PlayerAttack(indexAttack);
        }
        catch (ApiResponseException e) { Debug.LogError(e); }
    }

    public async void PlayerUseItem(int indexItem, int indexSelectedBlast = 0)
    {
        try
        {
            SetPlayerActionItem(indexItem, indexSelectedBlast);
            await _serverBattle.PlayerUseItem(indexItem, indexSelectedBlast);
        }
        catch (ApiResponseException e) { Debug.LogError(e); }
    }

    public void PlayerChangeBlast(int indexSelectedBlast)
    {
        try
        {
            SetPlayerActionSwap(indexSelectedBlast);
            _serverBattle.PlayerChangeBlast(indexSelectedBlast);
        }
        catch (ApiResponseException e) { Debug.LogError(e); }
    }

    public async void PlayerChooseOffer(int indexOffer)
    {
        try
        {
            await _serverBattle.PlayerChooseOffer(indexOffer);

            switch (_currentOffers[indexOffer].type)
            {
                case OfferType.Coin:
                    _coinGenerated += _currentOffers[indexOffer].coinsAmount;
                    break;
                case OfferType.Gem:
                    _gemGenerated += _currentOffers[indexOffer].gemsAmount;
                    break;
                case OfferType.Blast:
                case OfferType.Item:
                    PvEBattleReward.Add(_currentOffers[indexOffer]);
                    break;
                default:
                    break;
            }

            _coinGenerated += 200;

            _indexProgression++;

            await Task.Delay(500);

            _gameView.AddProgress();

            await Task.Delay(2000);

            await ShowWildBlast();

            NakamaManager.Instance.NakamaPvEBattle.PlayerReady();
        }
        catch (ApiResponseException e) { Debug.LogError(e); }
    }

    public async void PlayerWait()
    {
        try
        {
            SetPlayerActionWait();
            await _serverBattle.PlayerWait();
        }
        catch (ApiResponseException e) { Debug.LogError(e); }
    }

    public void PlayerLeave(bool leaveMatch)
    {
        if (leaveMatch) _serverBattle.LeaveMatch();

        if (_coinGenerated > 0)
        {
            if (BonusAds)
            {
                Offer coinBonus = new Offer();

                coinBonus.type = OfferType.Coin;
                coinBonus.coinsAmount = _coinGenerated / 2;
                coinBonus.isBonus = true;

                PvEBattleReward.Insert(0, coinBonus);
            }

            Offer coinReward = new Offer();

            coinReward.type = OfferType.Coin;
            coinReward.coinsAmount = _coinGenerated;

            PvEBattleReward.Insert(0, coinReward);
        }

        if (_gemGenerated > 0)
        {
            if (BonusAds)
            {
                Offer gemBonus = new Offer();

                gemBonus.type = OfferType.Gem;
                gemBonus.coinsAmount = _gemGenerated / 2;
                gemBonus.isBonus = true;

                PvEBattleReward.Insert(0, gemBonus);
            }

            Offer gemReward = new Offer();

            gemReward.type = OfferType.Gem;
            gemReward.gemsAmount = _gemGenerated;

            PvEBattleReward.Insert(0, gemReward);
        }

        UIManager.Instance.ChangeView(UIManager.Instance.EndView);
    }

    private void SetPlayerActionAttack(int moveIndex)
    {
        _playerAction.TurnType = TurnType.Attack;
        _playerAction.MoveIndex = moveIndex;
        _playerAction.MoveDamage = 0;
        _playerAction.MoveEffects = null;
    }

    private void SetPlayerActionItem(int itemIndex, int selectedBlastIndex)
    {
        _playerAction.TurnType = TurnType.Item;
        _playerAction.ItemIndex = itemIndex;
        _playerAction.SelectedBlastIndex = selectedBlastIndex;
    }

    private void SetPlayerActionSwap(int selectedBlastIndex)
    {
        _playerAction.TurnType = TurnType.Swap;
        _playerAction.SelectedBlastIndex = selectedBlastIndex;
    }

    private void SetPlayerActionWait()
    {
        _playerAction.TurnType = TurnType.Wait;
    }

    public void PlayerMustChangeBlast()
    {
        UIManager.Instance.ChangeBlastPopup.OpenPopup();

        List<UnityAction<int>> actions = new List<UnityAction<int>>()
        {
            PlayerChangeBlast,
            SwapBlast,
        };

        UIManager.Instance.ChangeBlastPopup.UpdateAction(actions, CHANGE_REASON.SWAP);
        UIManager.Instance.ChangeBlastPopup.UpdateClose(UIManager.Instance.GameView.ResetTab, false);
    }

    async void SwapBlast(int index)
    {
        await _gameView.ThrowBlast(_playerSquads[index]);
        _playerMeInfo.ActiveBlast = _playerSquads[index];
        await Task.Delay(TimeSpan.FromMilliseconds(500));
        _serverBattle.PlayerReady();
    }

    #endregion
}

[Serializable]
public struct TurnAction
{
    public TurnType TurnType;
    public int MoveIndex;
    public int MoveDamage;
    public List<MoveEffectData> MoveEffects;
    public int ItemIndex;
    public int SelectedBlastIndex;
}



