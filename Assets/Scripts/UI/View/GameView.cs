using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;

public class GameView : View
{
    [SerializeField] MovePanel _attackPanel;
    [SerializeField] BagPanel _bagPanel;

    [SerializeField] GameNavBar _bottomNavBar;

    [SerializeField] HUDLayout _playerHUD, _opponentHUD;
    [SerializeField] DialogLayout _dialogLayout;

    public HUDLayout PlayerHUD { get => _playerHUD; }
    public HUDLayout OpponentHUD { get => _opponentHUD; }
    public DialogLayout DialogLayout { get => _dialogLayout; }

    public MovePanel AttackPanel { get => _attackPanel; }
    public BagPanel BagPanel { get => _bagPanel; }

    Panel _currentPanel;

    WildBattleManager _wildBattleManager;
    NakamaData _dataUtils;

    public override void Init()
    {
        base.Init();

        _currentPanel = null;

        _attackPanel.Init();
        _bagPanel.Init();

        _wildBattleManager = WildBattleManager.Instance;
        _dataUtils = NakamaData.Instance;
    }

    public override void OpenView(bool _instant = false)
    {
        base.OpenView(_instant);

        _bottomNavBar.Init();

        DisableAttackPanel();
        HideNavBar();
    }

    public override void CloseView()
    {
        base.CloseView();

    }

    void ChangePanel(Panel newMiniPanel)
    {
        if (newMiniPanel == _currentPanel) return;

        if (_currentPanel != null)
        {
            _currentPanel.ClosePanel();
        }

        _currentPanel = newMiniPanel;

        _currentPanel.gameObject.SetActive(true);
        _currentPanel.OpenPanel();
    }

    public void CloseCurrentPanel()
    {
        if (_currentPanel != null)
        {
            _currentPanel.ClosePanel();
            _currentPanel = null;
        }
    }

    public void CloseAllPanel()
    {
        _attackPanel.ClosePanel();
        _bagPanel.ClosePanel();

        _currentPanel = null;
    }

    public void DisableAttackPanel()
    {
        _attackPanel.Disable();
    }

    public void HideNavBar()
    {
        _bottomNavBar.Hide();
    }

    public void ShowNavBar()
    {
        _bottomNavBar.Show();
    }

    public void ResetTab()
    {
        _bottomNavBar.Init();
        ChangePanel(_attackPanel);
    }

    #region WildBlastBattle

    public void SetMeteo(Meteo meteo)
    {
        var meteoData = ResourceObjectHolder.Instance.GetResourceByType((ResourceType)meteo);

        DialogLayout.SetMeteo(meteoData.Name.GetLocalizedString());

        //Instantiate(meteoData.Prefab); TODO mettre un fx de meteo
    }

    public void EndTurn(Blast playerBlast, Blast opponentBlast)
    {
        _playerHUD.UpdateManaBar(playerBlast.Mana);
        _opponentHUD.UpdateManaBar(opponentBlast.Mana);
    }

    public async Task BlastWait(bool isPlayer, Blast blast)
    {
        HUDLayout waiterHUD;

        if (isPlayer)
        {
            await _dialogLayout.UpdateTextAsync("You wait and regen mana !");
            waiterHUD = _playerHUD;
        }
        else
        {
            await _dialogLayout.UpdateTextAsync("Wild " + NakamaData.Instance.GetBlastDataRef(blast.data_id).Name.GetLocalizedString() + " wait and regen mana !");

            waiterHUD = _opponentHUD;
        }

        waiterHUD.UpdateManaBar(blast.Mana);
    }

    public async Task ApplyStatusEndTurn(bool isPlayer, Blast blast, Blast otherBlast)
    {
        HUDLayout blastHUD;
        HUDLayout otherHUD;

        blastHUD = isPlayer ? _playerHUD : _opponentHUD;
        otherHUD = isPlayer ? _opponentHUD : _playerHUD;

        string isWild = isPlayer ? "" : "Wild ";

        await _dialogLayout.UpdateTextAsync(isWild + NakamaData.Instance.GetBlastDataRef(blast.data_id).Name.GetLocalizedString() +
            " suffer from " +
            ResourceObjectHolder.Instance.GetResourceByType((ResourceType)blast.status));


        blastHUD.UpdateManaBar(blast.Mana);
        blastHUD.UpdateHpBar(blast.Hp);

        otherHUD.UpdateManaBar(otherBlast.Mana);
        otherHUD.UpdateHpBar(otherBlast.Hp);
    }

    public async Task AllPlayerBlastFainted()
    {
        await _dialogLayout.UpdateTextAsync("All your blast are fainted !");
    }

    public async Task BlastFainted(bool isPlayer, Blast blast)
    {
        HUDLayout waiterHUD;

        await _dialogLayout.UpdateTextAsync(NakamaData.Instance.GetBlastDataRef(blast.data_id).Name.GetLocalizedString() + " fainted !");

        waiterHUD = isPlayer ? _playerHUD : _opponentHUD;

        await waiterHUD.DoFaintedAnim();
    }

    public async Task BlastSwap(Blast currentBlast, Blast newCurrentBlast)
    {
        await ComeBackBlast(currentBlast);

        await ThrowBlast(newCurrentBlast);
    }

    public async Task ComeBackBlast(Blast currentBlast)
    {
        await _dialogLayout.UpdateTextAsync(_dataUtils.GetBlastDataRef(currentBlast.data_id).Name.GetLocalizedString() + " come back !");

        await _playerHUD.ComeBackBlast();
    }

    public async Task ThrowBlast(Blast newBlast)
    {
        PlayerHUD.Init(newBlast);
        AttackPanel.UpdateAttack(newBlast);

        await _dialogLayout.UpdateTextAsync(_dataUtils.GetBlastDataRef(newBlast.data_id).Name.GetLocalizedString() + " go !");

        await _playerHUD.ThrowBlast();
    }


    public async Task BlastUseItem(Item item, Blast selectedBlast = null, Blast wildBlast = null, bool isCaptured = false)
    {
        ItemData itemData = NakamaData.Instance.GetItemDataById(item.data_id);

        switch (itemData.behaviour)
        {
            case ItemBehaviour.HEAL:
                await _dialogLayout.UpdateTextAsync("You use " + NakamaData.Instance.GetItemDataRef(item.data_id).Name.GetLocalizedString() + " on " + _dataUtils.GetBlastDataRef(selectedBlast.data_id).Name.GetLocalizedString());

                _playerHUD.UpdateHpBar(selectedBlast.Hp);
                break;
            case ItemBehaviour.MANA:
                await _dialogLayout.UpdateTextAsync("You use " + NakamaData.Instance.GetItemDataRef(item.data_id).Name.GetLocalizedString() + " on " + _dataUtils.GetBlastDataRef(selectedBlast.data_id).Name.GetLocalizedString());

                _playerHUD.UpdateManaBar(selectedBlast.Mana);
                break;
            case ItemBehaviour.STATUS:
                break;
            case ItemBehaviour.CATCH:

                await _dialogLayout.UpdateTextAsync("You use " + NakamaData.Instance.GetItemDataRef(item.data_id).Name.GetLocalizedString() + " on " + _dataUtils.GetBlastDataRef(wildBlast.data_id).Name.GetLocalizedString());

                if (isCaptured)
                {
                    // DoBlastTrapAnim

                    break;
                }
                else
                {
                    // DoBlastTrapAnim

                    await _dialogLayout.UpdateTextAsync("You didn't caught the wild " + _dataUtils.GetBlastDataRef(wildBlast.data_id).Name.GetLocalizedString() + " !");
                    break;
                }
        }
    }

    public async Task BlastAttack(bool isPlayer, Blast attacker, Blast defender, Move move, int damage, MoveEffect moveEffect)
    {
        HUDLayout attackerHUD;
        HUDLayout defenderHUD;

        float effective = NakamaLogic.GetTypeMultiplier(NakamaData.Instance.GetBlastDataById(attacker.data_id).type, NakamaData.Instance.GetBlastDataById(defender.data_id).type);

        var isWild = isPlayer ? "Wild " : "";

        await _dialogLayout.UpdateTextAsync(
            isWild +
            _dataUtils.GetBlastDataRef(attacker.data_id).Name.GetLocalizedString() +
            " do " +
            NakamaData.Instance.GetMoveDataRef(move.id).Name.GetLocalizedString() +
            " !");

        attackerHUD = isPlayer ? _playerHUD : _opponentHUD;
        defenderHUD = isPlayer ? _opponentHUD : _playerHUD;

        attackerHUD.UpdateManaBar(attacker.Mana);

        await attackerHUD.DoAttackAnimAsync(defenderHUD, defender, effective);

        if (effective == 2)
        {
            await _dialogLayout.UpdateTextAsync("It's super affective !");
        }
        else if (effective == .5f)
        {
            await _dialogLayout.UpdateTextAsync("It's not super affective !");
        }

        if (moveEffect != MoveEffect.None)
        {
            string dialogText;
            (dialogText, _) = NakamaLogic.ApplyEffect(defender, move);
            await _dialogLayout.UpdateTextAsync(dialogText);
        }

        await Task.Delay(TimeSpan.FromMilliseconds(500));
    }

    public async Task CantAttack(bool isPlayer, Blast blast, Move move)
    {
        HUDLayout attackerHUD;

        attackerHUD = isPlayer ? _playerHUD : _opponentHUD;

        attackerHUD.gameObject.transform.DOShakePosition(.25f, new Vector3(100f, 0, 0));

        var isWild = isPlayer ? "Wild " : "";

        await _dialogLayout.UpdateTextAsync(
            isWild +
            NakamaData.Instance.GetBlastDataRef(blast.data_id).Name.GetLocalizedString()
            + " don't have enough mana to do "
            + NakamaData.Instance.GetMoveDataRef(move.id).Name.GetLocalizedString());
    }

    public async Task MultiplierAttack(bool isPlayer, Blast blast, Move move)
    {
        var isWild = isPlayer ? "Wild " : "";
        await _dialogLayout.UpdateTextAsync(isWild + NakamaData.Instance.GetBlastDataRef(blast.data_id).Name.GetLocalizedString() + " don't have enough mana to do " + NakamaData.Instance.GetItemDataRef(move.id).Name.GetLocalizedString());
    }

    public async void DoEndMatch(string textToShow)
    {
        await _dialogLayout.UpdateTextAsync(textToShow);

        await Task.Delay(TimeSpan.FromMilliseconds(1000));

        GameStateManager.Instance.UpdateStateToEnd();

    }
    #endregion
}
