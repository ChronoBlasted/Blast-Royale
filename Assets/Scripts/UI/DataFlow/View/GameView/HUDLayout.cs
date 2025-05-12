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
        _blastLevelTxt.text = "LVL." + NakamaLogic.CalculateLevelFromExperience(blast.exp);

        _hpSlider.Init(_blast.Hp, _blast.MaxHp);
        _manaSlider.Init(_blast.Mana, _blast.MaxMana);

        Sprite blastSprite = NakamaData.Instance.GetBlastDataRef(data.id).Sprite;

        SetStatus(_blast.status);

        _modifierManager.Init();
        ApplyAllModifiers(_blast);

        _blastInWorld.Init(blastSprite);
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
        _lastOpponentHUD.BlastInWorld.DoTakeDamageRender();

        if (effective > 1) TimeManager.Instance.DoLagTime(.2f, .15f);

        CameraManager.Instance.DoShakeCamera(4 * effective);

        if (_lastMoveDataRef.AttackAnimType == AA_Type.Distance)
        {
            StartCoroutine(HitCoroutineFX(.1f));
        }

        StartCoroutine(HitCoroutine(damage, .1f, effective > 1));

        _lastOpponentHUD.UpdateHpBar(defender.Hp, 0.2f * effective, .1f);
    }

    private IEnumerator HitCoroutineFX(float delay)
    {
        yield return new WaitForSeconds(delay);

        var fx = ResourceObjectHolder.Instance.GetTypeDataByType(_lastMove.type).TypeHitFX;
        var targetTransform = _lastOpponentHUD.BlastInWorld.transform;
        var hitFX = Instantiate(fx, targetTransform);
    }

    private IEnumerator HitCoroutine(int damage, float delay, bool isEffective)
    {
        yield return new WaitForSeconds(delay);

        var currentFloatingText = PoolManager.Instance[ResourceType.FloatingText].Get();
        currentFloatingText.transform.position = _lastOpponentHUD.BlastTransformInUI.position;
        currentFloatingText.GetComponent<FloatingText>().Init(damage.ToString(), Color.white, TextStyle.H1, isEffective);
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

        StartCoroutine(EnvironmentManager.Instance.DOTravelWorldCor(TravelEffect.EXPLODE, Vector3.one, ResourceType.BlastExp, _blastInWorld.transform, opponentHUD.BlastInWorld.transform, DoScaleExpBall, 0.5f, amountExp));
    }

    public void DoScaleExpBall(bool isStart)
    {
        if (isStart)
        {
        }
        else
        {
            BlastData currentData = NakamaData.Instance.GetBlastDataById(_blast.data_id);
            float amountExp = Mathf.FloorToInt(NakamaLogic.CalculateExpGain(currentData.expYield, _blast.Level, _lastOpponentHUD.Blast.Level) / NakamaLogic.GetAmountExpBall(NakamaData.Instance.GetBlastDataById(_lastOpponentHUD.Blast.data_id)));

            DOTween.Kill(_lastOpponentHUD.BlastInWorld.BlastRender.transform, true);

            _lastOpponentHUD.BlastInWorld.BlastRender.transform.DOPunchScale(new Vector3(.1f, .1f, .1f), .2f);

            var currentFloatingText = PoolManager.Instance[ResourceType.FloatingText].Get();
            currentFloatingText.transform.position = _lastOpponentHUD.BlastInWorld.transform.position + new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, 0);
            currentFloatingText.GetComponent<FloatingText>().Init("+" + amountExp + "exp", _expColor, TextStyle.H3);
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
