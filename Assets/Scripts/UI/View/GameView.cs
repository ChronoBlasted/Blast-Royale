using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;

public class GameView : View
{
    [SerializeField] MoveMiniPanel _attackPanel;
    [SerializeField] BagMiniPanel _bagPanel;
    [SerializeField] SquadMiniPanel _squadPanel;

    [SerializeField] GameNavBar _bottomNavBar;
    [SerializeField] CanvasGroup _headerCG;
    [SerializeField] CanvasGroup _runBtnCG;
    [SerializeField] CanvasGroup _waitBtnCG;

    [SerializeField] HUDLayout _playerHUD, _opponentHUD;
    [SerializeField] DialogLayout _dialogLayout;

    public HUDLayout PlayerHUD { get => _playerHUD; }
    public HUDLayout OpponentHUD { get => _opponentHUD; }
    public DialogLayout DialogLayout { get => _dialogLayout; }

    public MoveMiniPanel AttackPanel { get => _attackPanel; }
    public BagMiniPanel BagPanel { get => _bagPanel; }
    public SquadMiniPanel SquadPanel { get => _squadPanel; }

    Panel _currentPanel;

    WildBattleManager _wildBattleManager;
    NakamaData _dataUtils;

    Meteo _currentMeteo;

    public override void Init()
    {
        base.Init();

        _attackPanel.Init();
        _bagPanel.Init();
        _squadPanel.Init();

        _wildBattleManager = WildBattleManager.Instance;
        _dataUtils = NakamaData.Instance;
    }

    public override void OpenView(bool _instant = false)
    {
        base.OpenView(_instant);

        _currentPanel = null;

        _bottomNavBar.Init();

        DisablePanels(true);
        HideNavBar(true);

        ShowHUD();

        DialogLayout.Hide();

        StartCoroutine(PlayerHUD.BlastInWorld.SetPos());
        StartCoroutine(OpponentHUD.BlastInWorld.SetPos());
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
        _squadPanel.ClosePanel();

        _currentPanel = null;
    }

    public void DisablePanels(bool instant = false)
    {
        _attackPanel.Disable(instant);
        _bagPanel.Disable(instant);
        _squadPanel.Disable(instant);
    }

    public void HideNavBar(bool instant = false)
    {
        _bottomNavBar.Hide(instant);

        _runBtnCG.DOFade(0, .2f);
        _runBtnCG.interactable = false;
        _runBtnCG.blocksRaycasts = false;


        _waitBtnCG.DOFade(0, .2f);
        _waitBtnCG.interactable = false;
        _waitBtnCG.blocksRaycasts = false;
    }

    public void ShowNavBar()
    {
        _bottomNavBar.Show();

        _runBtnCG.DOFade(1, .2f);
        _runBtnCG.interactable = true;
        _runBtnCG.blocksRaycasts = true;

        _waitBtnCG.DOFade(1, .2f);
        _waitBtnCG.interactable = true;
        _waitBtnCG.blocksRaycasts = true;
    }


    public void HideHUD()
    {
        _playerHUD.Hide();
        _opponentHUD.Hide();

        _dialogLayout.Hide();

        _headerCG.DOFade(0, .2f);
        _headerCG.interactable = false;
        _headerCG.blocksRaycasts = false;
    }

    public void ShowHUD(bool shouldShowOpponentHUD = true)
    {
        if (shouldShowOpponentHUD)
        {
            _opponentHUD.Show();
        }
        else
        {
            _opponentHUD.Hide();
        }

        _playerHUD.Show();

        _headerCG.DOFade(1, .2f);
        _headerCG.interactable = true;
        _headerCG.blocksRaycasts = true;
    }


    public void ResetTab()
    {
        _bottomNavBar.Init();
        ChangePanel(_attackPanel);
    }

    #region WildBlastBattle

    public void SetMeteo(Meteo startDataMeteo)
    {
        var meteo = NakamaLogic.GetEnumFromIndex<Meteo>((int)startDataMeteo);

        var meteoData = ResourceObjectHolder.Instance.GetResourceByType((ResourceType)meteo);

        _currentMeteo = meteo;

        DialogLayout.SetMeteo(meteoData.Name.GetLocalizedString());

        EnvironmentManager.Instance.SetMeteo(meteo);
    }

    public void EndTurn(Blast playerBlast, Blast opponentBlast)
    {
        _playerHUD.UpdateManaBar(playerBlast.Mana);
        _opponentHUD.UpdateManaBar(opponentBlast.Mana);
    }

    public async Task BlastWait(bool isPlayer, Blast blast)
    {
        HUDLayout blastHUD;

        blastHUD = isPlayer ? _playerHUD : _opponentHUD;

        Instantiate(ResourceObjectHolder.Instance.GetResourceByType(ResourceType.Wait).Prefab, blastHUD.BlastInWorld.transform);

        // TODO Mettre en valeur la recovery de mana

        blastHUD.UpdateManaBar(blast.Mana);

        await Task.Delay(TimeSpan.FromMilliseconds(500));
    }

    public async Task ApplyStatusEndTurn(bool isPlayer, Blast blast, Blast otherBlast)
    {
        HUDLayout blastHUD;
        HUDLayout otherHUD;

        blastHUD = isPlayer ? _playerHUD : _opponentHUD;
        otherHUD = isPlayer ? _opponentHUD : _playerHUD;

        string isWild = isPlayer ? "" : "Wild ";

        ResourceData resourceData = ResourceObjectHolder.Instance.GetResourceByType((ResourceType)blast.status);

        Instantiate(resourceData.Prefab, blastHUD.BlastInWorld.BlastRender.transform);

        blastHUD.BlastInWorld.DoTakeDamageRender();

        // TODO FAire un rappel du status / mettre en evidence

        blastHUD.UpdateManaBar(blast.Mana);
        blastHUD.UpdateHpBar(blast.Hp);

        otherHUD.UpdateManaBar(otherBlast.Mana);
        otherHUD.UpdateHpBar(otherBlast.Hp);


        await Task.Delay(TimeSpan.FromMilliseconds(500));
    }

    public async Task AllPlayerBlastFainted()
    {
        _dialogLayout.Show();
        await _dialogLayout.UpdateTextAsync("All your blast are fainted !");
        _dialogLayout.Hide();
    }

    public async Task BlastFainted(bool isPlayer, Blast blast)
    {
        HUDLayout waiterHUD;

        // TODO Amplifie KO

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
        ItemDataRef itemDataRef = NakamaData.Instance.GetItemDataRef(item.data_id);
        BlastDataRef blastDataRef = NakamaData.Instance.GetBlastDataRef(selectedBlast.data_id);


        switch (itemData.behaviour)
        {
            case ItemBehaviour.HEAL:
                _dialogLayout.Show();

                await _dialogLayout.UpdateTextAsync("You use " + itemDataRef.Name.GetLocalizedString() + " on " + blastDataRef.Name.GetLocalizedString());

                _playerHUD.UpdateHpBar(selectedBlast.Hp);
                break;
            case ItemBehaviour.MANA:
                _dialogLayout.Show();

                await _dialogLayout.UpdateTextAsync("You use " + itemDataRef.Name.GetLocalizedString() + " on " + blastDataRef.Name.GetLocalizedString());

                _playerHUD.UpdateManaBar(selectedBlast.Mana);
                break;
            case ItemBehaviour.STATUS:
                _dialogLayout.Show();

                await _dialogLayout.UpdateTextAsync("You use " + itemDataRef.Name.GetLocalizedString() + " on " + blastDataRef.Name.GetLocalizedString());

                _playerHUD.SetStatus(selectedBlast.status);
                break;
            case ItemBehaviour.CATCH:
                if (isCaptured)
                {
                    await _opponentHUD.BlastInWorld.DoCatchBlastTrap(4, itemDataRef.Prefab);
                    break;
                }
                else
                {
                    await _opponentHUD.BlastInWorld.DoCatchBlastTrap(UnityEngine.Random.Range(0, 3), itemDataRef.Prefab);
                    break;
                }
        }

        _dialogLayout.Hide();
    }

    public async Task BlastAttack(bool isPlayer, Blast attacker, Blast defender, Move move, int damage, MoveEffect moveEffect)
    {
        HUDLayout attackerHUD = null;
        HUDLayout defenderHUD = null;
        MoveDataRef moveDataRef = NakamaData.Instance.GetMoveDataRef(move.id);
        float effective = NakamaLogic.GetTypeMultiplier(move.type, NakamaData.Instance.GetBlastDataById(defender.data_id).type);
        attackerHUD = isPlayer ? _playerHUD : _opponentHUD;

        HideHUD();

        attackerHUD.AttackLayout.Show(moveDataRef.Name.GetLocalizedString(), move.type);
        CameraManager.Instance.SetCameraPosition(new Vector3(attackerHUD.BlastInWorld.transform.position.x, attackerHUD.BlastInWorld.transform.position.y / 2, attackerHUD.BlastInWorld.transform.position.z / 2));
        CameraManager.Instance.SetCameraZoom(6);
        float shakeIntensity = .5f;
        if (damage > 50) shakeIntensity = 1f;
        else if (damage > 100) shakeIntensity = 2f;
        else if (damage > 200) shakeIntensity = 4f;
        CameraManager.Instance.DoShakeCamera(shakeIntensity, .125f, 1f);
        EnvironmentManager.Instance.SetDarkBackground(true);

        await Task.Delay(TimeSpan.FromMilliseconds(1000));

        attackerHUD.AttackLayout.Hide();
        CameraManager.Instance.Reset();
        EnvironmentManager.Instance.SetDarkBackground(false);

        ShowHUD();

        switch (move.target)
        {
            case Target.Opponent:
                defenderHUD = isPlayer ? _opponentHUD : _playerHUD;
                break;
            case Target.Self:
                defenderHUD = isPlayer ? _playerHUD : _opponentHUD;
                break;
        }

        switch (move.attackType)
        {
            case AttackType.None:
                break;
            case AttackType.Normal:
            case AttackType.Status:
                attackerHUD.UpdateManaBar(attacker.Mana);
                attackerHUD.BlastInWorld.PlatformLayout.AddEnergy(move.type);
                if (NakamaLogic.IsWeatherBoosted(_currentMeteo, move.type)) attackerHUD.BlastInWorld.PlatformLayout.AddEnergy(move.type);
                break;
            case AttackType.Special:
                attackerHUD.BlastInWorld.PlatformLayout.RemoveEnergyByType(move.type, move.cost);
                break;
        }

        await attackerHUD.DoAttackAnimAsync(defenderHUD, defender, move, damage, effective);

        //if (effective == 2) await _dialogLayout.UpdateTextAsync("It's super effective !");
        //else if (effective == .5f) await _dialogLayout.UpdateTextAsync("It's not super effective !");

        // TODO Mettre FX super efficace / pas super efficace

        if (moveEffect != MoveEffect.None)
        {
            string dialogText;
            dialogText = NakamaLogic.GetEffectMessage(moveEffect);

            defenderHUD.SetStatus(defender.status);

            var isStatusMove = move.attackType == AttackType.Status;
            defenderHUD.AddModifier(moveEffect, isStatusMove ? move.power : 1);

            Instantiate(ResourceObjectHolder.Instance.GetResourceByType((ResourceType)moveEffect).Prefab, defenderHUD.BlastInWorld.BlastRender.transform);

            defenderHUD.BlastInWorld.DoTakeDamageRender();

            await Task.Delay(TimeSpan.FromMilliseconds(500));
        }

        await Task.Delay(TimeSpan.FromMilliseconds(500));
    }

    public async void DoEndMatch(string textToShow)
    {
        await _dialogLayout.UpdateTextAsync(textToShow);

        await Task.Delay(TimeSpan.FromMilliseconds(1000));

        GameStateManager.Instance.UpdateStateToEnd();

    }
    #endregion
}
