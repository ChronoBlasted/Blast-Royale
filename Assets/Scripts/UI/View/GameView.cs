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
    [SerializeField] ProgressionLayout _progressionLayout;
    [SerializeField] ExpProgressionLayout _expProgressionLayout;

    public HUDLayout PlayerHUD { get => _playerHUD; }
    public HUDLayout OpponentHUD { get => _opponentHUD; }
    public DialogLayout DialogLayout { get => _dialogLayout; }

    public MoveMiniPanel AttackPanel { get => _attackPanel; }
    public BagMiniPanel BagPanel { get => _bagPanel; }
    public SquadMiniPanel SquadPanel { get => _squadPanel; }
    public ExpProgressionLayout ExpProgressionLayout { get => _expProgressionLayout; }

    Panel _currentPanel;

    WildBattleManager _wildBattleManager;
    NakamaData _dataUtils;

    Meteo _currentMeteo;

    #region Setup
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
    }

    public void ShowSpawnBlast()
    {
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

        _runBtnCG.DOFade(0, instant ? 0 : .2f);
        _runBtnCG.interactable = false;
        _runBtnCG.blocksRaycasts = false;


        _waitBtnCG.DOFade(0, instant ? 0 : .2f);
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

    #endregion
    #region WildBlastBattle

    public void SetMeteo(Meteo startDataMeteo)
    {
        var meteoData = ResourceObjectHolder.Instance.GetResourceByType((ResourceType)startDataMeteo);

        _currentMeteo = startDataMeteo;

        DialogLayout.SetMeteo(meteoData.Name.GetLocalizedString());

        EnvironmentManager.Instance.SetMeteo(startDataMeteo);
    }

    public void SetProgression(int indexProgression)
    {
        _progressionLayout.Init(indexProgression);
    }

    public void AddProgress()
    {
        _progressionLayout.SlideNext();
    }

    public void SetExpProgression(Sprite newSprite, int amountExp)
    {
        _expProgressionLayout.Init(newSprite, amountExp);
    }

    public void ShowExpProgression()
    {
        _expProgressionLayout.Show();
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

    public async Task BlastFainted(bool isPlayer, Blast blast)
    {
        HUDLayout faintedHUD;
        HUDLayout attackerHUD;

        faintedHUD = isPlayer ? _playerHUD : _opponentHUD;
        attackerHUD = isPlayer ? _opponentHUD : _playerHUD;

        DoZoomEffect(faintedHUD, "", 5);

        await Task.Delay(TimeSpan.FromMilliseconds(500));

        CameraManager.Instance.DoShakeCamera(1f, .125f);

        faintedHUD.DoSpawnExpBall(attackerHUD, NakamaLogic.GetAmountExpBall(_dataUtils.GetBlastDataById(blast.data_id)));
        faintedHUD.DoFaintedAnim();

        await Task.Delay(TimeSpan.FromMilliseconds(500));

        if (isPlayer == false)
        {
            ShowExpProgression();
        }

        ResetZoomEffect(faintedHUD);

        await Task.Delay(TimeSpan.FromMilliseconds(500));

        if (isPlayer == false)
        {
            int amountExp = Mathf.FloorToInt(NakamaLogic.CalculateExpGain(_dataUtils.GetBlastDataById(faintedHUD.Blast.data_id).expYield, faintedHUD.Blast.Level, faintedHUD.Blast.Level));
            SetExpProgression(_dataUtils.GetBlastDataRef(attackerHUD.Blast.data_id).Sprite, amountExp);

            await Task.Delay(TimeSpan.FromMilliseconds(1500));
        }
        else
        {
            await Task.Delay(TimeSpan.FromMilliseconds(500)); // Exp Ball finish tween

        }
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

        _expProgressionLayout.SetSprite(_dataUtils.GetBlastDataRef(newBlast.data_id).Sprite);

        await _playerHUD.ThrowBlast();
    }


    public async Task BlastUseItem(Item item, Blast selectedBlast = null, Blast wildBlast = null, bool isCaptured = false)
    {
        ItemData itemData = _dataUtils.GetItemDataById(item.data_id);
        ItemDataRef itemDataRef = _dataUtils.GetItemDataRef(item.data_id);
        BlastDataRef blastDataRef = _dataUtils.GetBlastDataRef(selectedBlast.data_id);


        switch (itemData.behaviour)
        {
            case ItemBehaviour.HEAL:
                await DoShowMessage("You use " + itemDataRef.Name.GetLocalizedString() + " on " + blastDataRef.Name.GetLocalizedString());

                _playerHUD.UpdateHpBar(selectedBlast.Hp);
                break;
            case ItemBehaviour.MANA:
                await DoShowMessage("You use " + itemDataRef.Name.GetLocalizedString() + " on " + blastDataRef.Name.GetLocalizedString());

                _playerHUD.UpdateManaBar(selectedBlast.Mana);
                break;
            case ItemBehaviour.STATUS:
                await DoShowMessage("You use " + itemDataRef.Name.GetLocalizedString() + " on " + blastDataRef.Name.GetLocalizedString());

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

        await Task.Delay(TimeSpan.FromMilliseconds(500));


        _dialogLayout.Hide();
    }

    public async Task BlastAttack(bool isPlayer, Blast attacker, Blast defender, Move move, int damage, MoveEffect moveEffect)
    {
        HUDLayout attackerHUD = null;
        HUDLayout defenderHUD = null;
        MoveDataRef moveDataRef = _dataUtils.GetMoveDataRef(move.id);
        float effective = NakamaLogic.GetTypeMultiplier(move.type, _dataUtils.GetBlastDataById(defender.data_id).type);
        attackerHUD = isPlayer ? _playerHUD : _opponentHUD;

        float shakeIntensity = .5f;
        if (damage > 50) shakeIntensity = 1f;
        else if (damage > 100) shakeIntensity = 2f;
        else if (damage > 200) shakeIntensity = 4f;

        DoZoomEffect(attackerHUD, moveDataRef.Name.GetLocalizedString(), -5f, move.type, shakeIntensity);

        await Task.Delay(TimeSpan.FromMilliseconds(1000));

        ResetZoomEffect(attackerHUD);

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

            Instantiate(ResourceObjectHolder.Instance.GetResourceByType((ResourceType)moveEffect).Prefab, defenderHUD.BlastInWorld.transform);

            defenderHUD.BlastInWorld.DoTakeDamageRender();

            await Task.Delay(TimeSpan.FromMilliseconds(500));
        }

        await Task.Delay(TimeSpan.FromMilliseconds(500));
    }

    public void ResetZoomEffect(HUDLayout attackerHUD)
    {
        attackerHUD.AttackLayout.Hide();
        CameraManager.Instance.ResetCamera();
        EnvironmentManager.Instance.SetDarkBackground(false);

        ShowHUD();
    }

    private void DoZoomEffect(HUDLayout attackerHUD, string text, float tilt = 5f, Type type = Type.Normal, float shakeIntensity = 0f)
    {
        HideHUD();

        if (text != "")
        {
            attackerHUD.AttackLayout.transform.rotation = Quaternion.Euler(0, 0, tilt);
            attackerHUD.AttackLayout.Show(text, type);
        }
        CameraManager.Instance.SetCameraPosition(new Vector3(attackerHUD.BlastInWorld.transform.position.x, attackerHUD.BlastInWorld.transform.position.y / 2, attackerHUD.BlastInWorld.transform.position.z / 2));
        CameraManager.Instance.SmoothCameraZoom(6);
        EnvironmentManager.Instance.SetDarkBackground(true);

        if (shakeIntensity > 0) CameraManager.Instance.DoShakeCamera(shakeIntensity, .125f, 1f);
    }

    public async Task DoShowMessage(string textToShow)
    {
        _dialogLayout.Show();
        await _dialogLayout.UpdateTextAsync(textToShow);
        _dialogLayout.Hide();
    }

    public void HandleOnProgressionClick()
    {
        _progressionLayout.Show();
    }
    #endregion
}
