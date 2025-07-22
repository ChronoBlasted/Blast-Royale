using Nakama;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class BattleBase : MonoBehaviour
{
    protected bool _isMatchWin;

    protected GameView _gameView;
    protected EndViewPvE _endView;
    protected NakamaBattleBase _serverBattle;
    protected NakamaUserAccount _userAccount;
    protected NakamaData _dataUtils;

    public bool BonusAds = false;
    public List<Offer> BattleReward;

    private List<Blast> _playerSquads = new List<Blast>();
    protected List<Blast> _opponentSquads = new List<Blast>();

    protected List<Item> _playerItems = new List<Item>();

    protected PlayerBattleInfo _playerMeInfo;
    protected PlayerBattleInfo _playerOpponentInfo;

    private Meteo _meteo;
    protected Blast _nextOpponentBlast;

    protected TurnStateData _turnStateData;
    protected TurnAction _playerAction, _opponentAction;

    public Blast OpponentBlast => _playerOpponentInfo.ActiveBlast;

    public Blast PlayerBlast => _playerMeInfo.ActiveBlast;

    public Meteo Meteo { get => _meteo; }
    public List<Blast> PlayerSquads { get => _playerSquads; }

    public void Init()
    {
        _gameView = UIManager.Instance.GameView;
        _endView = UIManager.Instance.EndViewPve;
        _userAccount = NakamaManager.Instance.NakamaUserAccount;
        _dataUtils = NakamaData.Instance;
    }

    public virtual void StartBattle(StartStateData startData)
    {
        _playerSquads.Clear();
        _playerItems.Clear();
        _opponentSquads.Clear();
        BattleReward.Clear();

        foreach (var blast in _userAccount.LastBlastCollection.deckBlasts)
            _playerSquads.Add(new Blast(blast.uuid, blast.data_id, blast.exp, blast.iv, blast.activeMoveset, blast.boss, blast.shiny));

        foreach (var item in _userAccount.LastItemCollection.deckItems)
            _playerItems.Add(new Item(item.data_id, item.amount));

        _playerMeInfo = new PlayerBattleInfo(_userAccount.Username, BlastOwner.Me, _playerSquads[0], _playerSquads, _playerItems);

        _gameView.PlayerHUD.BlastInWorld.PlatformLayout.Init();
        _gameView.OpponentHUD.BlastInWorld.PlatformLayout.Init();

        SetOpponent(startData.newBlastData);
        _playerOpponentInfo = new PlayerBattleInfo("", BlastOwner.Opponent, _nextOpponentBlast, new List<Blast> { _nextOpponentBlast }, null);

        _meteo = NakamaLogic.GetEnumFromIndex<Meteo>((int)startData.meteo);
        _gameView.SetMeteo(_meteo);

        _gameView.UpdateStateProgressLayout(true);

        _gameView.DialogLayout.UpdateText("");
        _gameView.DialogLayout.Hide();

        _gameView.PlayerHUD.Init(_playerMeInfo.ActiveBlast);

        _gameView.AttackPanel.UpdateAttack(_playerMeInfo.ActiveBlast);
        _gameView.SquadPanel.UpdateBlasts(_playerSquads);
        _gameView.BagPanel.UpdateItem(_playerItems);

        UIManager.Instance.ChangeBlastPopup.UpdateData(_playerSquads);

        _ = _gameView.PlayerHUD.ComeBackBlast(true);
        _ = _gameView.OpponentHUD.ComeBackBlast(true);

        GameStateManager.Instance.UpdateStateToGame();

        StartBattleAnim();
    }

    public  void SetOpponent(NewBlastData newBlastData)
    {
        var opponentBlast = new Blast("", newBlastData.id, newBlastData.exp, newBlastData.iv, newBlastData.activeMoveset, newBlastData.boss, newBlastData.shiny);

        _nextOpponentBlast = opponentBlast;
    }


    public async void StartBattleAnim()
    {
        UIManager.Instance.GameView.ShowSpawnBlast();

        await FrameWaiter.WaitForEndOfFrameAsync();

        CameraManager.Instance.SetCameraZoom(10);

        CameraManager.Instance.ResetCamera(1f);

        await ShowOpponentBlast();

        await _gameView.PlayerHUD.ThrowBlast();

        _serverBattle.PlayerReady();
    }



    public virtual async Task ShowOpponentBlast()
    {
        _gameView.OpponentHUD.Init(_nextOpponentBlast);

        _ = _gameView.OpponentHUD.ComeBackBlast(true);

        await _gameView.OpponentHUD.ThrowBlast();
    }

    public void StartNewTurn()
    {
        _playerAction = new TurnAction();
        _opponentAction = new TurnAction();
        _gameView.DialogLayout.Hide();
        _gameView.ShowNavBar();
        _gameView.ResetTab();
    }

    public void WaitForOpponent()
    {
        _gameView.DisablePanels();
        _gameView.HideNavBar();
    }

    public virtual async void PlayTurn(TurnStateData turnState)
    {
        _turnStateData = turnState;

        _playerAction.MoveDamage = turnState.p1TurnData.moveDamage;
        _playerAction.MoveEffects = turnState.p1TurnData.moveEffects;

        _opponentAction = UpdateActionTurn(_turnStateData.p2TurnData);

        _gameView.CloseAllPanel();

        bool isPlayerFirst = NakamaLogic.CompareActionPriorities(_playerAction.TurnType, _opponentAction.TurnType);

        if (_playerAction.TurnType == TurnType.Attack && _opponentAction.TurnType == TurnType.Attack)
        {
            var playerMove = _dataUtils.GetMoveById(PlayerBlast.activeMoveset[_playerAction.MoveIndex]);
            var wildMove = _dataUtils.GetMoveById(OpponentBlast.activeMoveset[_opponentAction.MoveIndex]);

            isPlayerFirst = playerMove.priority > wildMove.priority ||
                            (playerMove.priority == wildMove.priority && NakamaLogic.GetFasterBlast(PlayerBlast, OpponentBlast));
        }

        bool battleStop = false;

        if (isPlayerFirst)
        {
            var gameLogicContextPlayer = new GameLogicContext(
                attacker: _playerMeInfo.ActiveBlast,
                defender: _playerOpponentInfo.ActiveBlast,
                players: new List<PlayerBattleInfo> { _playerMeInfo, _playerOpponentInfo },
                moveIndex: _playerAction.MoveIndex,
                moveDamage: _playerAction.MoveDamage,
                moveEffects: _playerAction.MoveEffects,
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
                attacker: _playerOpponentInfo.ActiveBlast,
                defender: _playerMeInfo.ActiveBlast,
                players: new List<PlayerBattleInfo> { _playerMeInfo, _playerOpponentInfo },
                moveIndex: _opponentAction.MoveIndex,
                moveDamage: _opponentAction.MoveDamage,
                moveEffects: _opponentAction.MoveEffects,
                itemIndex: _opponentAction.ItemIndex,
                selectedBlastIndex: _opponentAction.SelectedBlastIndex,
                isCatched: false
            );


            var wildHandler = TurnHandlerFactory.CreateHandler(_opponentAction.TurnType, gameLogicContextWild, _gameView, _dataUtils);
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
                attacker: _playerOpponentInfo.ActiveBlast,
                defender: _playerMeInfo.ActiveBlast,
                players: new List<PlayerBattleInfo> { _playerMeInfo, _playerOpponentInfo },
                moveIndex: _opponentAction.MoveIndex,
                moveDamage: _opponentAction.MoveDamage,
                moveEffects: _opponentAction.MoveEffects,
                itemIndex: _opponentAction.ItemIndex,
                selectedBlastIndex: _opponentAction.SelectedBlastIndex,
                isCatched: false
            );

            var wildHandler = TurnHandlerFactory.CreateHandler(_opponentAction.TurnType, gameLogicContextWild, _gameView, _dataUtils);
            battleStop = await wildHandler.HandleTurn();
            if (battleStop)
            {
                await StopTurnHandler();
                return;
            }

            var gameLogicContextPlayer = new GameLogicContext(
                attacker: _playerMeInfo.ActiveBlast,
                defender: _playerOpponentInfo.ActiveBlast,
                players: new List<PlayerBattleInfo> { _playerMeInfo, _playerOpponentInfo },
                moveIndex: _playerAction.MoveIndex,
                moveDamage: _playerAction.MoveDamage,
                moveEffects: _playerAction.MoveEffects,
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
                    defender: _playerOpponentInfo.ActiveBlast,
                    players: new List<PlayerBattleInfo> { _playerMeInfo, _playerOpponentInfo },
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

        if (_playerOpponentInfo.ActiveBlast.status != Status.None)
        {
            var statusHandler = TurnHandlerFactory.CreateHandler(TurnType.Status,
                new GameLogicContext(
                    attacker: _playerOpponentInfo.ActiveBlast,
                    defender: _playerMeInfo.ActiveBlast,
                    players: new List<PlayerBattleInfo> { _playerMeInfo, _playerOpponentInfo },
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

    private TurnAction UpdateActionTurn(PlayerTurnData turnData)
    {
        TurnAction turnAction = new TurnAction();

        turnAction.TurnType = turnData.type;

        if (turnAction.TurnType == TurnType.Attack)
        {
            turnAction.MoveIndex = turnData.moveIndex;
            turnAction.MoveDamage = turnData.moveDamage;
            turnAction.MoveEffects = turnData.moveEffects;
        }

        return turnAction;
    }

    public virtual async Task StopTurnHandler()
    {
        if (NakamaLogic.IsBlastAlive(_playerOpponentInfo.ActiveBlast) == false)
        {
            await ShowOpponentBlast();
        }
        else if (NakamaLogic.IsAllBlastFainted(_playerMeInfo.Blasts))
        {
            await PlayerLeave(false);
        }

        EndTurn();

        if (NakamaLogic.IsBlastAlive(_playerMeInfo.ActiveBlast))
        {
            _serverBattle.PlayerReady();
        }
    }

    public void EndTurn()
    {
        if (NakamaLogic.IsBlastAlive(_playerMeInfo.ActiveBlast)) _playerMeInfo.ActiveBlast.Mana = NakamaLogic.Instance.CalculateStaminaRecovery(_playerMeInfo.ActiveBlast.MaxMana, _playerMeInfo.ActiveBlast.Mana, false);
        if (NakamaLogic.IsBlastAlive(_playerOpponentInfo.ActiveBlast)) _playerOpponentInfo.ActiveBlast.Mana = NakamaLogic.Instance.CalculateStaminaRecovery(_playerOpponentInfo.ActiveBlast.MaxMana, _playerOpponentInfo.ActiveBlast.Mana, false);

        _gameView.EndTurn(_playerMeInfo.ActiveBlast, _playerOpponentInfo.ActiveBlast);
    }

    public virtual async Task PlayerLeave(bool leaveMatch)
    {
        if (leaveMatch) await _serverBattle.LeaveMatch();

        UIManager.Instance.ChangeView(UIManager.Instance.EndViewPve);
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

    public async void PlayerWait()
    {
        try
        {
            SetPlayerActionWait();
            await _serverBattle.PlayerWait();
        }
        catch (ApiResponseException e) { Debug.LogError(e); }
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



