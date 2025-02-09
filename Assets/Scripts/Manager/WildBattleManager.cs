using BaseTemplate.Behaviours;
using Nakama;
using Nakama.TinyJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class WildBattleManager : MonoSingleton<WildBattleManager>
{
    bool _isMatchWin;

    //Cache
    GameView _gameView;
    EndView _endView;
    NakamaWildBattle _serverBattle;
    NakamaUserAccount _userAccount;
    NakamaData _dataUtils;

    // Data
    Blast _playerBlast;
    List<Blast> _playerSquads = new List<Blast>();
    List<Item> _playerItems = new List<Item>();

    Blast _wildBlast;

    // Logic
    TurnStateData _turnStateData;
    TurnAction _playerAction, _wbAction;

    public List<Blast> PlayerSquads { get => _playerSquads; }
    public List<Item> PlayerItems { get => _playerItems; }
    public Blast PlayerBlast { get => _playerBlast; }

    public void Init()
    {
        _gameView = UIManager.Instance.GameView;
        _endView = UIManager.Instance.EndView;
        _serverBattle = NakamaManager.Instance.NakamaWildBattle;
        _userAccount = NakamaManager.Instance.NakamaUserAccount;
        _dataUtils = NakamaData.Instance;

        GameStateManager.Instance.OnGameStateChanged += HandleStateChange;
    }

    public void StartBattle(StartStateData startData)
    {
        _playerSquads.Clear();
        _playerItems.Clear();

        foreach (var blast in _userAccount.LastBlastCollection.deckBlasts)
        {
            _playerSquads.Add(new Blast(blast.uuid, blast.data_id, blast.exp, blast.iv, blast.activeMoveset, blast.status));
        }

        _playerBlast = _playerSquads[0];

        foreach (var item in _userAccount.LastItemCollection.deckItems)
        {
            _playerItems.Add(new Item(item.data_id, item.amount));
        }

        _wildBlast = new Blast("", startData.id, startData.exp, startData.iv, startData.activeMoveset, startData.status);

        _gameView.PlayerHUD.Init(_playerBlast);
        _gameView.OpponentHUD.Init(_wildBlast);

        _ = _gameView.PlayerHUD.ComeBackBlast(true);
        _ = _gameView.OpponentHUD.ComeBackBlast(true);

        _ = _gameView.PlayerHUD.ThrowBlast();
        _ = _gameView.OpponentHUD.ThrowBlast();

        _gameView.AttackPanel.UpdateAttack(_playerBlast);

        _gameView.BagPanel.UpdateItem(_playerItems);

        UIManager.Instance.ChangeBlastPopup.UpdateData(_playerSquads);

        _gameView.DialogLayout.UpdateText("");

        GameStateManager.Instance.UpdateStateToGame();

        _serverBattle.PlayerReady();
    }

    public void StartNewTurn()
    {
        _playerAction = new TurnAction();
        _wbAction = new TurnAction();

        _gameView.DialogLayout.UpdateText("Your turn !");

        _gameView.ShowNavBar();
        _gameView.ResetTab();
    }

    public void WaitForOpponent()
    {
        _gameView.DisableAttackPanel();
        _gameView.HideNavBar();

        _gameView.DialogLayout.UpdateText("Waiting for opponent...");
    }

    public async void PlayTurn(TurnStateData turnState)
    {
        _turnStateData = turnState;

        UpdateOpponentTurn(_turnStateData);

        _gameView.CloseAllPanel();

        if (_playerAction.TurnType == TurnType.SWAP)
        {
            await _gameView.BlastSwap(_playerBlast, _playerSquads[_playerAction.SelectedBlastIndex]);

            _playerBlast = _playerSquads[_playerAction.SelectedBlastIndex];

            await Task.Delay(TimeSpan.FromMilliseconds(500));
        }

        if (_playerAction.TurnType == TurnType.ITEM)
        {
            ItemData itemData = NakamaData.Instance.GetItemDataById(_playerItems[_playerAction.ItemIndex].data_id);

            switch (itemData.behaviour)
            {
                case ItemBehaviour.HEAL:
                    _playerSquads[_playerAction.SelectedBlastIndex].Hp += itemData.gain_amount;
                    break;
                case ItemBehaviour.MANA:
                    _playerSquads[_playerAction.SelectedBlastIndex].Mana += itemData.gain_amount;
                    break;
                case ItemBehaviour.STATUS:
                    _playerSquads[_playerAction.SelectedBlastIndex].status = itemData.status;
                    break;
                case ItemBehaviour.CATCH:
                    if (turnState.catched)
                    {
                        _gameView.DoEndMatch("You caught the wild " + _dataUtils.GetBlastDataRef(_wildBlast.data_id).Name.GetLocalizedString());
                    }
                    break;
            }

            _playerItems[_playerAction.ItemIndex].amount--;

            await _gameView.BlastUseItem(_playerItems[_playerAction.ItemIndex], _playerSquads[_playerAction.SelectedBlastIndex], _wildBlast, turnState.catched);

            _gameView.BagPanel.UpdateUI(_playerItems);

            UIManager.Instance.ChangeBlastPopup.UpdateData(_playerSquads);

            await Task.Delay(TimeSpan.FromMilliseconds(500));
        }

        if (_playerAction.TurnType == TurnType.ATTACK && _wbAction.TurnType == TurnType.ATTACK)
        {
            bool isPlayerBlastFaster = NakamaLogic.GetFasterBlast(_playerBlast, _wildBlast);

            if (isPlayerBlastFaster) await Attack(turnState, true);
            else await Attack(turnState, false);

            if (await CheckIfKO()) return;


            if (isPlayerBlastFaster == false) await Attack(turnState, true);
            else await Attack(turnState, false);

            if (await CheckIfKO()) return;
        }

        if (_playerAction.TurnType == TurnType.ATTACK && _wbAction.TurnType != TurnType.ATTACK)
        {
            await Attack(turnState, true);

            await Task.Delay(TimeSpan.FromMilliseconds(500));

            if (await CheckIfKO()) return;
        }



        if (_playerAction.TurnType != TurnType.ATTACK && _wbAction.TurnType == TurnType.ATTACK)
        {
            await Attack(turnState, false);

            await Task.Delay(TimeSpan.FromMilliseconds(500));

            if (await CheckIfKO()) return;
        }


        if (_playerAction.TurnType == TurnType.WAIT)
        {
            _playerBlast.Mana = NakamaLogic.Instance.CalculateStaminaRecovery(_playerBlast.MaxMana, _playerBlast.Mana, true);

            await _gameView.BlastWait(true, _playerBlast);

            await Task.Delay(TimeSpan.FromMilliseconds(500));
        }

        if (_wbAction.TurnType == TurnType.WAIT)
        {
            _wildBlast.Mana = NakamaLogic.Instance.CalculateStaminaRecovery(_wildBlast.MaxMana, _wildBlast.Mana, true);

            await _gameView.BlastWait(false, _wildBlast);

            await Task.Delay(TimeSpan.FromMilliseconds(500));
        }

        EndTurn();

        NakamaManager.Instance.NakamaWildBattle.PlayerReady();
    }

    void EndTurn()
    {
        if (NakamaLogic.IsBlastAlive(_playerBlast) && _playerAction.TurnType != TurnType.WAIT) _playerBlast.Mana = NakamaLogic.Instance.CalculateStaminaRecovery(_playerBlast.MaxMana, _playerBlast.Mana, false);
        if (NakamaLogic.IsBlastAlive(_wildBlast) && _wbAction.TurnType != TurnType.WAIT) _wildBlast.Mana = NakamaLogic.Instance.CalculateStaminaRecovery(_wildBlast.MaxMana, _wildBlast.Mana, false);

        _gameView.EndTurn(_playerBlast, _wildBlast);
    }

    async Task<bool> CheckIfKO()
    {
        if (!NakamaLogic.IsBlastAlive(_wildBlast))
        {
            await _gameView.BlastFainted(false, _wildBlast);

            await Task.Delay(TimeSpan.FromMilliseconds(500));

            UIManager.Instance.LevelExpPopup.UpdateData(_playerBlast, _wildBlast);
            UIManager.Instance.LevelExpPopup.OpenPopup();

            _ = NakamaManager.Instance.NakamaUserAccount.GetPlayerBlast();
            _ = NakamaManager.Instance.NakamaUserAccount.GetPlayerBag();


            UIManager.Instance.LevelExpPopup.UpdateClose(GameStateManager.Instance.UpdateStateToEnd);

            return true;
        }
        else if (!NakamaLogic.IsBlastAlive(_playerBlast))
        {
            await _gameView.BlastFainted(true, _playerBlast);

            await Task.Delay(TimeSpan.FromMilliseconds(500));

            if (NakamaLogic.IsAllBlastDead(_playerSquads))
            {
                await _gameView.AllPlayerBlastFainted();

                GameStateManager.Instance.UpdateStateToEnd();
            }
            else
            {
                EndTurn();

                PlayerMustChangeBlast();
            }

            return true;
        }
        else return false;
    }

    async Task Attack(TurnStateData turnState, bool isPlayerAttack)
    {
        Blast attacker = isPlayerAttack ? _playerBlast : _wildBlast;
        Blast defender = isPlayerAttack ? _wildBlast : _playerBlast;
        int moveDamage = isPlayerAttack ? turnState.p_move_damage : turnState.wb_move_damage;
        Status moveStatus = isPlayerAttack ? turnState.p_move_status : turnState.wb_move_status;

        defender.Hp -= moveDamage;
        defender.status -= moveStatus;

        Move move = isPlayerAttack ? _dataUtils.GetMoveById(attacker.activeMoveset[_playerAction.MoveIndex]) : _dataUtils.GetMoveById(attacker.activeMoveset[_wbAction.MoveIndex]);

        if (attacker.Mana >= move.cost)
        {
            attacker.Mana -= move.cost;

            await _gameView.BlastAttack(isPlayerAttack, attacker, defender, move, moveDamage, moveStatus);
        }
        else
        {
            await _gameView.CantAttack(isPlayerAttack, attacker, move);
        }
    }


    private void UpdateOpponentTurn(TurnStateData turnState)
    {
        _wbAction.TurnType = turnState.wb_turn_type;

        if (turnState.wb_turn_type == TurnType.ATTACK)
        {
            _wbAction.MoveIndex = turnState.wb_move_index;
            _wbAction.MoveDamage = turnState.wb_move_damage;
            _wbAction.MoveStatus = turnState.wb_move_status;
        }
    }


    public void MatchEnd(string data)
    {
        bool result;
        if (Boolean.TryParse(data, out result))
        {
            _isMatchWin = result;
            _endView.UpdateEndGame(_isMatchWin);
        }
    }

    #region PlayerAction 
    public async void PlayerAttack(int indexAttack)
    {
        try
        {
            _playerAction.TurnType = TurnType.ATTACK;
            _playerAction.MoveIndex = indexAttack;

            await _serverBattle.PlayerAttack(indexAttack);
        }
        catch (ApiResponseException e)
        {
            Debug.Log(e);
        }
    }

    public async void PlayerUseItem(int indexItem, int indexSelectedBlast = 0)
    {
        try
        {
            _playerAction.TurnType = TurnType.ITEM;

            _playerAction.ItemIndex = indexItem;
            _playerAction.SelectedBlastIndex = indexSelectedBlast;

            await _serverBattle.PlayerUseItem(indexItem, indexSelectedBlast);
        }
        catch (ApiResponseException e)
        {
            Debug.Log(e);
        }
    }

    public void PlayerChangeBlast(int indexSelectedBlast)
    {
        try
        {
            _playerAction.TurnType = TurnType.SWAP;
            _playerAction.SelectedBlastIndex = indexSelectedBlast;

            _serverBattle.PlayerChangeBlast(indexSelectedBlast);
        }
        catch (ApiResponseException e)
        {
            Debug.Log(e);
        }
    }

    public async void PlayerWait()
    {
        try
        {
            _playerAction.TurnType = TurnType.WAIT;

            await _serverBattle.PlayerWait();
        }
        catch (ApiResponseException e)
        {
            Debug.Log(e);
        }
    }

    public void PlayerMustChangeBlast()
    {
        UIManager.Instance.ChangeBlastPopup.OpenPopup();

        List<UnityAction<int>> actions = new List<UnityAction<int>>()
        {
            PlayerChangeBlast,
            SwapBlast,
        };

        UIManager.Instance.ChangeBlastPopup.UpdateAction(actions);

        UIManager.Instance.ChangeBlastPopup.UpdateClose(UIManager.Instance.GameView.ResetTab, false);
    }

    async void SwapBlast(int index)
    {
        await _gameView.ThrowBlast(_playerSquads[index]);

        _playerBlast = _playerSquads[index];

        await Task.Delay(TimeSpan.FromMilliseconds(500));

        _serverBattle.PlayerReady();
    }

    #endregion


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
    }
    void HandleGame()
    {
    }
    void HandleEnd()
    {
    }
    void HandleWait()
    {
    }
}

[Serializable]
public struct TurnAction
{
    public TurnType TurnType;

    public int MoveIndex;
    public int MoveDamage;
    public Status MoveStatus;

    public int ItemIndex;
    public int SelectedBlastIndex;
}
