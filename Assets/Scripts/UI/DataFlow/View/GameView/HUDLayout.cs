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
    [SerializeField] Image _borderHUD;
    [SerializeField] TMP_Text _blastNameTxt, _blastLevelTxt;
    [SerializeField] SliderBar _hpSlider, _manaSlider;

    [SerializeField] RectTransform _blastTransformInUI;
    [SerializeField] BlastInWorld _blastInWorld;

    [SerializeField] ModifierManager _modifierManager;
    [SerializeField] StatusLayout _statusLayout;

    Blast _blast;

    Tweener _attackAnimTween, _takeDamageAnimTween;

    MoveDataRef _lastMoveDataRef;
    Move _lastMove;
    HUDLayout _lastOpponentHUD;

    public RectTransform BlastTransformInUI { get => _blastTransformInUI; }
    public BlastInWorld BlastInWorld { get => _blastInWorld; }

    public void Init(Blast blast)
    {
        BlastData data = NakamaData.Instance.GetBlastDataById(blast.data_id);

        _blast = blast;

        _blastNameTxt.text = NakamaData.Instance.GetBlastDataRef(data.id).Name.GetLocalizedString();
        _blastLevelTxt.text = "LVL." + NakamaLogic.CalculateLevelFromExperience(blast.exp);

        _borderHUD.color = ResourceObjectHolder.Instance.GetTypeDataByType(data.type).Color;

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

    public async Task DoAttackAnimAsync(HUDLayout opponentHUD, Blast defender, Move move, float effective)
    {
        if (_attackAnimTween != null)
        {
            _attackAnimTween.Kill(true);
            _attackAnimTween = null;
        }

        _lastMoveDataRef = NakamaData.Instance.GetMoveDataRef(move.id);
        _lastMove = move;
        _lastOpponentHUD = opponentHUD;

        await _lastMoveDataRef.AA_Data.PlayAnimation(_blastInWorld, _lastOpponentHUD.BlastInWorld, _lastMoveDataRef.ParticleSystem, () =>
        {
            DoTakeDamageAnim(defender, effective);
        });

        await Task.Delay(1000);
    }

    public void DoTakeDamageAnim(Blast defender, float effective)
    {
        if (_takeDamageAnimTween != null)
        {
            _takeDamageAnimTween.Kill(true);
            _takeDamageAnimTween = null;
        }

        var render = _lastOpponentHUD.BlastInWorld.BlastRender;

        var tweenLoopDuration = .1f;

        _takeDamageAnimTween = render.DOColor(Color.black, tweenLoopDuration)
                                     .SetLoops(2, LoopType.Yoyo)
                                     .SetEase(Ease.OutSine);

        if (_lastMoveDataRef.AttackAnimType == AA_Type.Distance)
        {
            StartCoroutine(HitMoveTypeFXCoroutine(tweenLoopDuration));
        }

        _lastOpponentHUD.UpdateHpBar(defender.Hp, 0.2f * effective, .5f);
    }

    private IEnumerator HitMoveTypeFXCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        var fx = ResourceObjectHolder.Instance.GetTypeDataByType(_lastMove.type).TypeHitFX;
        var targetTransform = _lastOpponentHUD.BlastInWorld.transform;
        var hitFX = Instantiate(fx, targetTransform);
        Destroy(hitFX, 2f);
    }

    public async Task DoFaintedAnim()
    {
        if (_attackAnimTween.IsActive())
        {
            _attackAnimTween.Kill(true);
            _attackAnimTween = null;
        }

        _blastInWorld.BlastRender.DOFade(0, .5f);

        _attackAnimTween = _blastInWorld.BlastRender.transform.DOLocalMove(new Vector2(0, -2), 0.5f);

        transform.DOScale(0f, .5f);
        _cg.DOFade(0f, .5f);

        await Task.Delay(TimeSpan.FromMilliseconds(500));
    }

    public async Task ComeBackBlast(bool isIntant = false)
    {
        var duration = isIntant ? 0f : .5f;

        if (_attackAnimTween.IsActive())
        {
            _attackAnimTween.Kill(true);
            _attackAnimTween = null;
        }

        if (_isPlayerBlast) _attackAnimTween = _blastInWorld.BlastRender.transform.DOLocalMove(new Vector2(-2, -2), duration);
        else _attackAnimTween = _blastInWorld.BlastRender.transform.DOLocalMove(new Vector2(2, 2), duration);

        _cg.DOFade(0f, duration);
        _blastInWorld.BlastRender.DOFade(0, duration);

        transform.DOScale(0f, duration);

        await Task.Delay(TimeSpan.FromMilliseconds(500));
    }

    public async Task ThrowBlast(bool isIntant = false)
    {
        var duration = isIntant ? 0f : .5f;

        if (_attackAnimTween.IsActive())
        {
            _attackAnimTween.Kill(true);
            _attackAnimTween = null;
        }

        if (_isPlayerBlast) _attackAnimTween = _blastInWorld.BlastRender.transform.DOLocalMove(Vector2.zero, duration);
        else _attackAnimTween = _blastInWorld.BlastRender.transform.DOLocalMove(Vector2.zero, duration);

        _cg.DOFade(1f, duration);
        _blastInWorld.BlastRender.DOFade(1f, duration);

        transform.DOScale(1f, duration);

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
