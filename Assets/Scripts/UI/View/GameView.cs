using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
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
    [SerializeField] TimerLayout _timerLayout;

    [SerializeField] StartGameLayout _startGameLayout;

    public HUDLayout PlayerHUD { get => _playerHUD; }
    public HUDLayout OpponentHUD { get => _opponentHUD; }
    public DialogLayout DialogLayout { get => _dialogLayout; }

    public MoveMiniPanel AttackPanel { get => _attackPanel; }
    public BagMiniPanel BagPanel { get => _bagPanel; }
    public SquadMiniPanel SquadPanel { get => _squadPanel; }
    public ExpProgressionLayout ExpProgressionLayout { get => _expProgressionLayout; }

    Panel _currentPanel;

    NakamaData _dataUtils;

    Meteo _currentMeteo;
    Coroutine _timerCoroutine;

    #region Setup
    public override void Init()
    {
        base.Init();

        _attackPanel.Init();
        _bagPanel.Init();
        _squadPanel.Init();

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

    public async Task StartBattleAnim(StartStateData startData)
    {
        _startGameLayout.UpdateData(startData.opponentName, startData.opponentTrophy, startData.opponentStats);

        await _startGameLayout.DoStartAnim();
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

    public void StartTimer(int timer)
    {
        _timerLayout.Show();

        if (_timerCoroutine != null)
        {
            StopCoroutine(_timerCoroutine);
            _timerCoroutine = null;
        }

        _timerCoroutine = StartCoroutine(TimerCor(timer));
    }

    public void StopTimer()
    {
        if (_timerCoroutine != null)
        {
            StopCoroutine(_timerCoroutine);
            _timerCoroutine = null;
        }

        _timerLayout.Hide();
    }


    IEnumerator TimerCor(int timer)
    {
        while (timer >= 0)
        {
            _timerLayout.SetTimer(timer);

            yield return new WaitForSeconds(1);

            timer--;
        }

        _timerLayout.Hide();
    }

    public void UpdateGameviewState(BattleMode battleMode)
    {
        _timerLayout.gameObject.SetActive(false);
        _progressionLayout._permanentSlot.gameObject.SetActive(false);
        BagPanel.UpdateTabState(battleMode);

        switch (battleMode)
        {
            case BattleMode.PvP:
                _timerLayout.gameObject.SetActive(true);
                _timerLayout.Hide(true);
                break;
            case BattleMode.PvE:
                _progressionLayout._permanentSlot.gameObject.SetActive(true);
                break;
        }
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

    public async Task BlastSwap(bool isPlayer, Blast currentBlast, Blast newCurrentBlast)
    {
        await ComeBackBlast(isPlayer, currentBlast);

        await ThrowBlast(isPlayer, newCurrentBlast);
    }

    public async Task ComeBackBlast(bool isPlayer, Blast currentBlast)
    {
        HUDLayout HUDLayout = isPlayer ? PlayerHUD : OpponentHUD;

        await HUDLayout.ComeBackBlast();
    }

    public async Task ThrowBlast(bool isPlayer, Blast newBlast)
    {
        HUDLayout HUDLayout = isPlayer ? PlayerHUD : OpponentHUD;
        HUDLayout.Init(newBlast);

        if (isPlayer)
        {
            AttackPanel.UpdateAttack(newBlast);
            _expProgressionLayout.SetSprite(_dataUtils.GetBlastDataRef(newBlast.data_id).Sprite);
        }

        await HUDLayout.ThrowBlast();
    }


    public async Task BlastUseItem(Item item, Blast selectedBlast = null, Blast wildBlast = null, bool isCaptured = false)
    {
        ItemData itemData = _dataUtils.GetItemDataById(item.data_id);
        ItemDataRef itemDataRef = _dataUtils.GetItemDataRef(item.data_id);
        BlastDataRef blastDataRef = _dataUtils.GetBlastDataRef(selectedBlast.data_id);


        switch (itemData.behaviour)
        {
            case ItemBehaviour.Heal:
                await DoShowMessage("You use " + itemDataRef.Name.GetLocalizedString() + " on " + blastDataRef.Name.GetLocalizedString());

                _playerHUD.UpdateHpBar(selectedBlast.Hp);
                break;
            case ItemBehaviour.Mana:
                await DoShowMessage("You use " + itemDataRef.Name.GetLocalizedString() + " on " + blastDataRef.Name.GetLocalizedString());

                _playerHUD.UpdateManaBar(selectedBlast.Mana);
                break;
            case ItemBehaviour.Status:
                await DoShowMessage("You use " + itemDataRef.Name.GetLocalizedString() + " on " + blastDataRef.Name.GetLocalizedString());

                _playerHUD.SetStatus(selectedBlast.status);
                break;
            case ItemBehaviour.Catch:
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

    public async Task BlastAttack(bool isPlayer, Blast attacker, Blast defender, Move move, int damage, List<MoveEffectData> moveEffects)
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

        // TODO Mettre FX super efficace / pas super efficace

        if (moveEffects != null)
        {
            foreach (var effect in moveEffects)
            {
                if (effect.effect != MoveEffect.None)
                {
                    string dialogText;
                    dialogText = NakamaLogic.GetEffectMessage(effect.effect);

                    defenderHUD.SetStatus(defender.status);

                    var isStatusMove = move.attackType == AttackType.Status;
                    defenderHUD.AddModifier(effect.effect, isStatusMove ? effect.effectModifier : 1);

                    Instantiate(ResourceObjectHolder.Instance.GetResourceByType((ResourceType)effect.effect).Prefab, defenderHUD.BlastInWorld.transform);

                    defenderHUD.BlastInWorld.DoTakeDamageRender();

                    await Task.Delay(TimeSpan.FromMilliseconds(500));
                }
            }
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
    }

    public void HandleOnProgressionClick()
    {
        _progressionLayout.Show();
    }
    #endregion
}
