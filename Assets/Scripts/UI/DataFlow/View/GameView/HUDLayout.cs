using DG.Tweening;
using System;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDLayout : MonoBehaviour
{
    [SerializeField] bool _isPlayerBlast;
    [SerializeField] CanvasGroup _cg;
    [SerializeField] TMP_Text _blastNameTxt, _blastLevelTxt;
    [SerializeField] SliderBar _hpSlider, _manaSlider;
    [SerializeField] AttackLayout _attackLayout;

    [SerializeField] RectTransform _blastTransformInUI;
    [SerializeField] BlastInWorld _blastInWorld;

    [SerializeField] ModifierManager _modifierManager;
    [SerializeField] StatusLayout _statusLayout;
    [SerializeField] Color _expColor;
    [SerializeField] ParticleSystem _onEndExpBall;
    [SerializeField] ParticleSystem _onBossSpawn, _onShinySpawn;

    Blast _blast;

    MoveDataRef _lastMoveDataRef;
    Move _lastMove;
    HUDLayout _lastOpponentHUD;

    public RectTransform BlastTransformInUI { get => _blastTransformInUI; }
    public BlastInWorld BlastInWorld { get => _blastInWorld; }
    public AttackLayout AttackLayout { get => _attackLayout; }
    public Blast Blast { get => _blast; }

    public void Init(Blast blast)
    {
        BlastData data = NakamaData.Instance.GetBlastDataById(blast.data_id);

        _blast = blast;

        _blastNameTxt.text = NakamaData.Instance.GetBlastDataRef(data.id).Name.GetLocalizedString();
        UpdateData(blast);

        SetStatus(_blast.status);

        _modifierManager.Init();
        ApplyAllModifiers(_blast);

        _blastInWorld.Init(NakamaData.Instance.GetSpriteWithBlast(_blast));
    }

    void UpdateData(Blast blast)
    {
        _blastLevelTxt.text = "LVL." + NakamaLogic.CalculateLevelFromExperience(blast.exp);

        _hpSlider.Init(_blast.Hp, _blast.MaxHp);
        _manaSlider.Init(_blast.Mana, _blast.MaxMana);
    }

    public async Task DoLevelUp(int level)
    {
        _blastLevelTxt.text = "LVL." + level;

        _hpSlider.Init(_blast.Hp, _blast.CalculateBlastHp(NakamaData.Instance.GetBlastDataById(_blast.data_id).hp, _blast.iv, level));
        _manaSlider.Init(_blast.Mana, _blast.CalculateBlastMana(NakamaData.Instance.GetBlastDataById(_blast.data_id).mana, _blast.iv, level));

        await transform.DOPunchScale(new Vector3(.2f, .2f, .2f), .5f, 1, 1).AsyncWaitForCompletion();
    }

    public void UpdateHpBar(int newHp, float duration = .2f, float delay = 0f)
    {
        _hpSlider.SetValueSmooth(newHp, duration, delay);
    }

    public void UpdateManaBar(int newMana)
    {
        _manaSlider.SetValueSmooth(newMana);
    }

    public void Show()
    {
        _cg.DOFade(1f, .2f);
    }

    public void Hide()
    {
        _cg.DOFade(0f, .2f);
    }

    public async Task DoAttackAnimAsync(HUDLayout opponentHUD, Blast defender, Move move, int damage, float effective)
    {
        _lastMoveDataRef = NakamaData.Instance.GetMoveDataRef(move.id);
        _lastMove = move;
        _lastOpponentHUD = opponentHUD;

        await _lastMoveDataRef.AA_Data.PlayAnimation(_blastInWorld, _lastOpponentHUD.BlastInWorld, _lastMoveDataRef.ParticleSystem, () =>
        {
            DoTakeDamageAnim(defender, damage, effective);
        });

        await Task.Delay(500);
    }

    public void DoTakeDamageAnim(Blast defender, int damage, float effective)
    {
        if (defender != Blast)
        {
            _lastOpponentHUD.BlastInWorld.DoTakeDamageRender();

            CameraManager.Instance.DoShakeCamera(4 * effective);

            if (effective > 1) TimeManager.Instance.DoLagTime(.2f, .15f);
        }

        if (_lastMoveDataRef.AttackAnimType == AA_Type.Distance)
        {
            StartCoroutine(HitCoroutineFX(.1f));
        }

        if (damage > 0)
        {
            StartCoroutine(HitCoroutine(damage, .1f, effective));

            _lastOpponentHUD.UpdateHpBar(defender.Hp, 0.2f * effective, .1f);
        }
    }

    private IEnumerator HitCoroutineFX(float delay)
    {
        yield return new WaitForSeconds(delay);

        var fx = ResourceObjectHolder.Instance.GetTypeDataByType(_lastMove.type).TypeHitFX;
        var targetTransform = _lastOpponentHUD.BlastInWorld.transform;
        var hitFX = Instantiate(fx, targetTransform);
    }

    private IEnumerator HitCoroutine(int damage, float delay, float effective)
    {
        yield return new WaitForSeconds(delay);

        var currentFloatingText = PoolManager.Instance[ResourceType.FloatingText].Get();
        currentFloatingText.transform.position = _lastOpponentHUD.BlastTransformInUI.position;

        currentFloatingText.GetComponent<FloatingText>().Init(damage.ToString(), ColorManager.Instance.GetEffectiveColor(effective), new Vector3(effective, effective, effective), TextStyle.H1);
    }

    public void DoFaintedAnim()
    {
        _blastInWorld.BlastRender.DOFade(0, .5f);

        Instantiate(ResourceObjectHolder.Instance.GetResourceByType(ResourceType.BlastFainted).Prefab, _blastInWorld.transform.position, Quaternion.identity);

        _blastInWorld.BlastRender.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack);

        transform.DOScale(0f, .5f);
        _cg.DOFade(0f, .5f);
    }

    public void DoSpawnExpBall(HUDLayout opponentHUD, int amountExp)
    {
        _lastOpponentHUD = opponentHUD;

        StartCoroutine(EnvironmentManager.Instance.DOTravelWorldCor(TravelEffect.EXPLODE, Vector3.one, ResourceType.BlastExp, _blastInWorld.transform, _lastOpponentHUD.BlastInWorld.transform, DoScaleExpBall, 0.5f, amountExp));
    }

    public void DoScaleExpBall(bool isStart)
    {
        if (isStart)
        {
        }
        else
        {
            BlastData currentData = NakamaData.Instance.GetBlastDataById(_blast.data_id);

            DOTween.Kill(_lastOpponentHUD.BlastInWorld.BlastRender.transform, true);

            _lastOpponentHUD.BlastInWorld.BlastRender.transform.DOPunchScale(new Vector3(.1f, .1f, .1f), .2f);

            DOTween.Kill(UIManager.Instance.GameView.ExpProgressionLayout.Amount.transform, true);

            UIManager.Instance.GameView.ExpProgressionLayout.Amount.transform.DOPunchScale(new Vector3(.3f, .3f, .3f), .2f);

            Instantiate(_onEndExpBall.gameObject, _lastOpponentHUD.BlastInWorld.transform);
        }
    }

    public async Task ComeBackBlast(bool isIntant = false)
    {
        var duration = isIntant ? 0f : .5f;

        if (_isPlayerBlast) _blastInWorld.BlastRender.transform.DOLocalMove(new Vector2(-2, -2), duration);
        else _blastInWorld.BlastRender.transform.DOLocalMove(new Vector2(2, 2), duration);

        _cg.DOFade(0f, duration);
        _blastInWorld.BlastRender.DOFade(0, duration);

        transform.DOScale(0f, duration);

        await Task.Delay(TimeSpan.FromMilliseconds(500));
    }

    public async Task ThrowBlast(bool isIntant = false)
    {
        var duration = isIntant ? 0f : .5f;

        if (_isPlayerBlast) _blastInWorld.BlastRender.transform.DOLocalMove(Vector2.zero, duration);
        else _blastInWorld.BlastRender.transform.DOLocalMove(Vector2.zero, duration);

        _cg.DOFade(1f, duration);
        _blastInWorld.BlastRender.DOFade(1f, duration);

        transform.DOScale(1f, duration);

        _blastInWorld.BlastRender.transform.localScale = Vector3.zero;
        _blastInWorld.BlastRender.transform.DOScale(Vector3.one, duration);

        await Task.Delay(TimeSpan.FromMilliseconds(500));

        if (_blast.shiny || _blast.boss)
        {
            if (_blast.shiny)
            {
                EnvironmentManager.Instance.SetDarkBackground(true);
                Instantiate(_onShinySpawn, BlastInWorld.transform);
            }
            else
            {
                Instantiate(_onBossSpawn, BlastInWorld.transform);
            }

            await Task.Delay(500);

            if (_blast.shiny) EnvironmentManager.Instance.SetDarkBackground(false);
        }
    }

    public void SetStatus(Status newStatus)
    {
        _statusLayout.gameObject.SetActive(newStatus != Status.None);

        _statusLayout.Init(newStatus);
    }

    public void AddModifier(MoveEffect newModifier, int amount)
    {
        switch (newModifier)
        {
            case MoveEffect.AttackBoost:
                _modifierManager.AddModifier(StatType.Attack, amount);
                break;
            case MoveEffect.DefenseBoost:
                _modifierManager.AddModifier(StatType.Defense, amount);
                break;
            case MoveEffect.SpeedBoost:
                _modifierManager.AddModifier(StatType.Speed, amount);
                break;
            case MoveEffect.AttackReduce:
                _modifierManager.AddModifier(StatType.Attack, -amount);
                break;
            case MoveEffect.DefenseReduce:
                _modifierManager.AddModifier(StatType.Defense, -amount);
                break;
            case MoveEffect.SpeedReduce:
                _modifierManager.AddModifier(StatType.Speed, -amount);
                break;
        }
    }

    public void ApplyAllModifiers(Blast newBlast)
    {
        foreach (var mod in newBlast.modifiers)
        {
            if (mod.amount == 0) continue;

            MoveEffect effect = _modifierManager.GetMoveEffectFromStat(mod.stats, mod.amount);
            if (effect != MoveEffect.None)
            {
                AddModifier(effect, mod.amount);
            }
        }
    }
}
