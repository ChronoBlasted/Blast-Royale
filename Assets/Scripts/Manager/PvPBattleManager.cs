using BaseTemplate.Behaviours;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PvPBattleManager : MonoSingleton<PvPBattleManager>
{
    bool _isMatchWin;

    GameView _gameView;
    EndView _endView;
    NakamaPvPBattle _serverBattle;
    NakamaUserAccount _userAccount;
    NakamaData _dataUtils;

    public bool BonusAds = false;
    public List<Offer> PvPBattleReward;


    List<Blast> _playerSquads = new List<Blast>();
    PlayerBattleInfo _playerMeInfo;

    List<Blast> _opponentSquads = new List<Blast>();
    PlayerBattleInfo _playerOpponentInfo;

    Meteo _meteo;
    Blast _nextOpponentBlast;

    // Logic
    TurnStateData _turnStateData;
    TurnAction _playerAction, _opponentAction;


    public List<Blast> PlayerSquads => _playerSquads;
    public List<Blast> OpponentSquads => _opponentSquads;

    public Meteo Meteo => _meteo;

    public TurnAction PlayerAction { get => _playerAction; }
    public TurnAction OpponentAction { get => _opponentAction; }

    public Blast PlayerBlast { get => _playerMeInfo.ActiveBlast; }
    public Blast OpponentBlast { get => _playerOpponentInfo.ActiveBlast; }

    public void Init()
    {
        _gameView = UIManager.Instance.GameView;
        _endView = UIManager.Instance.EndView;
        _serverBattle = NakamaManager.Instance.NakamaPvPBattle;
        _userAccount = NakamaManager.Instance.NakamaUserAccount;
        _dataUtils = NakamaData.Instance;
    }

    public async void StartBattle(StartStateData startData)
    {
        _playerSquads.Clear();
        _opponentSquads.Clear();

        foreach (var blast in _userAccount.LastBlastCollection.deckBlasts)
            _playerSquads.Add(new Blast(blast.uuid, blast.data_id, blast.exp, blast.iv, blast.activeMoveset, blast.boss, blast.shiny));


        _playerMeInfo = new PlayerBattleInfo(_userAccount.Username, BlastOwner.Me, _playerSquads[0], _playerSquads, null);

        var newBlastData = startData.newBlastData;

        SetNewOpponentBlast(startData.newBlastData);
        _playerOpponentInfo = new PlayerBattleInfo("", BlastOwner.Opponent, _nextOpponentBlast, new List<Blast> { _nextOpponentBlast, null, null }, null);

        _gameView.PlayerHUD.BlastInWorld.PlatformLayout.Init();
        _gameView.OpponentHUD.BlastInWorld.PlatformLayout.Init();

        _meteo = NakamaLogic.GetEnumFromIndex<Meteo>((int)startData.meteo);
        _gameView.SetMeteo(_meteo);

        _gameView.UpdateStateProgressLayout(true);

        _gameView.DialogLayout.UpdateText("");
        _gameView.DialogLayout.Hide();

        _gameView.PlayerHUD.Init(_playerMeInfo.ActiveBlast);

        _gameView.AttackPanel.UpdateAttack(_playerMeInfo.ActiveBlast);
        _gameView.BagPanel.HandleOnPvPBattle(true);
        _gameView.SquadPanel.UpdateBlasts(_playerSquads);

        UIManager.Instance.ChangeBlastPopup.UpdateData(_playerSquads);

        _ = _gameView.PlayerHUD.ComeBackBlast(true);
        _ = _gameView.OpponentHUD.ComeBackBlast(true);

        GameStateManager.Instance.UpdateStateToGame();

        UIManager.Instance.GameView.ShowSpawnBlast();

        await FrameWaiter.WaitForEndOfFrameAsync();

        CameraManager.Instance.SetCameraZoom(10);

        CameraManager.Instance.ResetCamera(1f);

        await ShowOpponentBlast();

        await _gameView.PlayerHUD.ThrowBlast();

        _serverBattle.PlayerReady();
    }

    public void SetNewOpponentBlast(NewBlastData newBlastData)
    {
        var _wildBlast = new Blast("", newBlastData.id, newBlastData.exp, newBlastData.iv, newBlastData.activeMoveset, newBlastData.boss, newBlastData.shiny);

        _nextOpponentBlast = _wildBlast;
    }

    public async Task ShowOpponentBlast()
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

    public async void PlayTurn(TurnStateData turnState)
    {
        _turnStateData = turnState;

        UpdateOpponentTurn(_turnStateData);

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
                attacker: _playerOpponentInfo.ActiveBlast,
                defender: _playerMeInfo.ActiveBlast,
                players: new List<PlayerBattleInfo> { _playerMeInfo, _playerOpponentInfo },
                moveIndex: _turnStateData.p2MoveIndex,
                moveDamage: _turnStateData.p2MoveDamage,
                moveEffects: _turnStateData.p2MoveEffects,
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
                moveIndex: _turnStateData.p2MoveIndex,
                moveDamage: _turnStateData.p2MoveDamage,
                moveEffects: _turnStateData.p2MoveEffects,
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

    private void UpdateOpponentTurn(TurnStateData turnState)
    {
        _opponentAction.TurnType = turnState.p2TurnType;

        if (_opponentAction.TurnType == TurnType.Attack)
        {
            _opponentAction.MoveIndex = turnState.p2MoveIndex;
            _opponentAction.MoveDamage = turnState.p2MoveDamage;
            _opponentAction.MoveEffects = turnState.p2MoveEffects;
        }
    }

    public async Task StopTurnHandler()
    {
        if (NakamaLogic.IsBlastAlive(_playerOpponentInfo.ActiveBlast) == false)
        {
            await ShowOpponentBlast();
        }
        else if (NakamaLogic.IsAllBlastFainted(_playerMeInfo.Blasts))
        {
            PlayerLeave(false);
        }

        EndTurn();

        if (NakamaLogic.IsBlastAlive(_playerMeInfo.ActiveBlast))
        {
            NakamaManager.Instance.NakamaPvEBattle.PlayerReady();
        }
    }

    public void EndTurn()
    {
        if (NakamaLogic.IsBlastAlive(_playerMeInfo.ActiveBlast)) _playerMeInfo.ActiveBlast.Mana = NakamaLogic.Instance.CalculateStaminaRecovery(_playerMeInfo.ActiveBlast.MaxMana, _playerMeInfo.ActiveBlast.Mana, false);
        if (NakamaLogic.IsBlastAlive(_playerOpponentInfo.ActiveBlast)) _playerOpponentInfo.ActiveBlast.Mana = NakamaLogic.Instance.CalculateStaminaRecovery(_playerOpponentInfo.ActiveBlast.MaxMana, _playerOpponentInfo.ActiveBlast.Mana, false);

        _gameView.EndTurn(_playerMeInfo.ActiveBlast, _playerOpponentInfo.ActiveBlast);
    }

    public void PlayerLeave(bool leaveMatch)
    {
        if (leaveMatch) _serverBattle.LeaveMatch();

        UIManager.Instance.ChangeView(UIManager.Instance.EndView);
    }

    public void MatchEnd(string data)
    {
        if (bool.TryParse(data, out var result))
        {
            _isMatchWin = result;
            _endView.UpdateEndGame(_isMatchWin);
        }
    }
}
