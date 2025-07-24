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

    protected StartStateData _startData;
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
        _startData = startData;

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

        foreach (var blast in startData.newBlastSquad)
            _opponentSquads.Add(new Blast(blast.uuid, blast.data_id, blast.exp, blast.iv, blast.activeMoveset, blast.boss, blast.shiny));

        _nextOpponentBlast = _opponentSquads[0];
        _playerOpponentInfo = new PlayerBattleInfo("", BlastOwner.Opponent, _opponentSquads[0], _opponentSquads, null);

        _meteo = NakamaLogic.GetEnumFromIndex<Meteo>((int)startData.meteo);
        _gameView.SetMeteo(_meteo);

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

    public void SetOpponent(NewBlastData newBlastData)
    {
        var opponentBlast = new Blast("", newBlastData.id, newBlastData.exp, newBlastData.iv, newBlastData.activeMoveset, newBlastData.boss, newBlastData.shiny);

        _nextOpponentBlast = opponentBlast;
    }

    public virtual async void StartBattleAnim()
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
        ResetAction();
        _gameView.DialogLayout.Hide();
        _gameView.ShowNavBar();
        _gameView.ResetTab();

        _gameView.StartTimer(_startData.turnDelay / 1000);
    }

    public async void StartNewMustSwapTurn()
    {
        _gameView.StartTimer(_startData.turnDelay / 1000);

        if (NakamaLogic.IsBlastAlive(_playerOpponentInfo.ActiveBlast) == false && _playerOpponentInfo.OwnerType == BlastOwner.Opponent)
        {
            await _gameView.DoShowMessage("Waiting for ennemy swap");
        }

        ResetAction();
    }

    public void ResetAction()
    {
        _playerAction = new TurnAction();
        _opponentAction = new TurnAction();
    }

    public void WaitForOpponent()
    {
        _gameView.DisablePanels();
        _gameView.HideNavBar();
    }

    public virtual async void PlayTurn(TurnStateData turnState)
    {
        _turnStateData = turnState;

        _playerAction = UpdateActionTurn(_turnStateData.p1TurnData);
        _opponentAction = UpdateActionTurn(_turnStateData.p2TurnData);

        _gameView.CloseAllPanel();
        _gameView.StopTimer();

        bool isPlayerFirst = NakamaLogic.CompareActionPriorities(_playerAction.TurnType, _opponentAction.TurnType);

        if (_playerAction.TurnType == TurnType.Attack && _opponentAction.TurnType == TurnType.Attack)
        {
            var playerMove = _dataUtils.GetMoveById(PlayerBlast.activeMoveset[_playerAction.MoveIndex]);
            var wildMove = _dataUtils.GetMoveById(OpponentBlast.activeMoveset[_opponentAction.MoveIndex]);

            isPlayerFirst = playerMove.priority > wildMove.priority ||
                            (playerMove.priority == wildMove.priority &&
                             NakamaLogic.GetFasterBlast(PlayerBlast, OpponentBlast));
        }

        var firstAction = isPlayerFirst ? _playerAction : _opponentAction;
        var secondAction = isPlayerFirst ? _opponentAction : _playerAction;
        var firstInfo = isPlayerFirst ? _playerMeInfo : _playerOpponentInfo;
        var secondInfo = isPlayerFirst ? _playerOpponentInfo : _playerMeInfo;

        if (await HandleSingleTurn(firstAction, firstInfo, secondInfo, isPlayerFirst))
        {
            await StopTurnHandler();
            return;
        }
        if (await HandleSingleTurn(secondAction, secondInfo, firstInfo, !isPlayerFirst))
        {
            await StopTurnHandler();
            return;
        }

        if (await HandleStatusIfNeeded(_playerMeInfo, _playerOpponentInfo)) return;
        if (await HandleStatusIfNeeded(_playerOpponentInfo, _playerMeInfo)) return;

        await StopTurnHandler();
    }

    public virtual async void PlayMustSwapTurn(TurnStateData turnState)
    {
        _turnStateData = turnState;

        _playerAction = UpdateActionTurn(_turnStateData.p1TurnData);
        _opponentAction = UpdateActionTurn(_turnStateData.p2TurnData);

        _gameView.CloseAllPanel();
        _gameView.StopTimer();

        bool isPlayerFirst = NakamaLogic.CompareActionPriorities(_playerAction.TurnType, _opponentAction.TurnType);

        var firstAction = isPlayerFirst ? _playerAction : _opponentAction;
        var secondAction = isPlayerFirst ? _opponentAction : _playerAction;
        var firstInfo = isPlayerFirst ? _playerMeInfo : _playerOpponentInfo;
        var secondInfo = isPlayerFirst ? _playerOpponentInfo : _playerMeInfo;

        await HandleSingleTurn(firstAction, firstInfo, secondInfo, isPlayerFirst);
        await HandleSingleTurn(secondAction, secondInfo, firstInfo, !isPlayerFirst);

        _serverBattle.PlayerReady();
    }

    private async Task<bool> HandleSingleTurn(TurnAction action, PlayerBattleInfo attackerInfo, PlayerBattleInfo defenderInfo, bool isPlayer)
    {
        var context = new GameLogicContext(
            attacker: attackerInfo.ActiveBlast,
            defender: defenderInfo.ActiveBlast,
            players: new List<PlayerBattleInfo> { _playerMeInfo, _playerOpponentInfo },
            moveIndex: action.MoveIndex,
            moveDamage: action.MoveDamage,
            moveEffects: action.MoveEffects,
            itemIndex: action.ItemIndex,
            selectedBlastIndex: action.SelectedBlastIndex,
            isCatched: isPlayer ? _turnStateData.catched : false
        );

        var handler = TurnHandlerFactory.CreateHandler(action.TurnType, context, _gameView, _dataUtils);
        return await handler.HandleTurn();
    }

    private async Task<bool> HandleStatusIfNeeded(PlayerBattleInfo statusOwner, PlayerBattleInfo target)
    {
        if (statusOwner.ActiveBlast.status == Status.None) return false;

        var statusContext = new GameLogicContext(
            attacker: statusOwner.ActiveBlast,
            defender: target.ActiveBlast,
            players: new List<PlayerBattleInfo> { _playerMeInfo, _playerOpponentInfo },
            moveIndex: -1,
            moveDamage: 0,
            moveEffects: null,
            itemIndex: -1,
            selectedBlastIndex: -1,
            isCatched: false
        );

        var statusHandler = TurnHandlerFactory.CreateHandler(TurnType.Status, statusContext, _gameView, _dataUtils);
        return await statusHandler.HandleTurn();
    }


    private TurnAction UpdateActionTurn(PlayerTurnData turnData)
    {
        TurnAction turnAction = new TurnAction();

        turnAction.TurnType = turnData.type;

        if (turnAction.TurnType == TurnType.Attack)
        {
            turnAction.MoveIndex = turnData.index;
            turnAction.MoveDamage = turnData.moveDamage;
            turnAction.MoveEffects = turnData.moveEffects;
        }
        else if (turnAction.TurnType == TurnType.Swap)
        {
            turnAction.SelectedBlastIndex = turnData.index;
        }

        return turnAction;
    }

    public virtual async Task StopTurnHandler()
    {
        if (NakamaLogic.IsBlastAlive(_playerOpponentInfo.ActiveBlast) == false && _playerOpponentInfo.OwnerType == BlastOwner.Wild)
        {
            await ShowOpponentBlast();
        }
        else if (NakamaLogic.IsAllBlastFainted(_playerMeInfo.Blasts))
        {
            await PlayerLeave(false);
        }

        EndTurn();

        _serverBattle.PlayerReady();
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

    public virtual async void PlayerAttack(int indexAttack)
    {
        try
        {
            SetPlayerActionAttack(indexAttack);
            await _serverBattle.PlayerAttack(indexAttack);
        }
        catch (ApiResponseException e) { Debug.LogError(e); }
    }

    public virtual async void PlayerUseItem(int indexItem, int indexSelectedBlast = 0)
    {
        try
        {
            SetPlayerActionItem(indexItem, indexSelectedBlast);
            await _serverBattle.PlayerUseItem(indexItem, indexSelectedBlast);
        }
        catch (ApiResponseException e) { Debug.LogError(e); }
    }

    public virtual void PlayerChangeBlast(int indexSelectedBlast)
    {
        try
        {
            SetPlayerActionSwap(indexSelectedBlast);
            _serverBattle.PlayerChangeBlast(indexSelectedBlast);
        }
        catch (ApiResponseException e) { Debug.LogError(e); }
    }

    public virtual async void PlayerWait()
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
        };

        UIManager.Instance.ChangeBlastPopup.UpdateAction(actions, CHANGE_REASON.SWAP);
        UIManager.Instance.ChangeBlastPopup.UpdateClose(UIManager.Instance.GameView.ResetTab, false);
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



