using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDLayout : MonoBehaviour
{
    [SerializeField] CanvasGroup _cg;
    [SerializeField] TMP_Text _blastNameTxt, _blastLevelTxt;
    [SerializeField] Image _blastImg, _borderHUD;
    [SerializeField] SliderBar _hpSlider, _manaSlider;

    [SerializeField] bool _isPlayerBlast;

    Blast _blast;
    Vector3 _startPos;

    Tweener _doBlastAnimTween;

    public void Init(Blast blast)
    {
        BlastData data = NakamaData.Instance.GetBlastDataById(blast.data_id);

        _blast = blast;
        _startPos = transform.position;

        _blastNameTxt.text = NakamaData.Instance.GetBlastDataRef(data.id).Name.GetLocalizedString();

        _blastLevelTxt.text = "LVL." + NakamaLogic.CalculateLevelFromExperience(blast.exp);

        _blastImg.sprite = NakamaData.Instance.GetBlastDataRef(data.id).Sprite;
        _borderHUD.color = ResourceObjectHolder.Instance.GetTypeDataByType(data.type).Color;

        _hpSlider.Init(_blast.Hp, _blast.MaxHp);
        _manaSlider.Init(_blast.Mana, _blast.MaxMana);
    }

    public void UpdateHpBar(int newHp)
    {
        _hpSlider.SetValueSmooth(newHp);
    }

    public void UpdateManaBar(int newMana)
    {
        _manaSlider.SetValueSmooth(newMana);
    }

    public async Task DoAttackAnimAsync()
    {
        if (_doBlastAnimTween.IsActive())
        {
            _doBlastAnimTween.Kill(true);
            _doBlastAnimTween = null;
        }

        if (_isPlayerBlast) _doBlastAnimTween = _blastImg.rectTransform.DOAnchorPos(new Vector2(100, 100), 0.5f).SetLoops(2, LoopType.Yoyo);
        else _doBlastAnimTween = _blastImg.rectTransform.DOAnchorPos(new Vector2(-100, -100), 0.5f).SetLoops(2, LoopType.Yoyo);

        await Task.Delay(TimeSpan.FromMilliseconds(1000));
    }


    public async Task DoFaintedAnim()
    {
        if (_doBlastAnimTween.IsActive())
        {
            _doBlastAnimTween.Kill(true);
            _doBlastAnimTween = null;
        }

        _blastImg.DOFade(0, .5f);

        if (_isPlayerBlast) _doBlastAnimTween = _blastImg.rectTransform.DOAnchorPos(new Vector2(0, -100), 0.5f);
        else _doBlastAnimTween = _blastImg.rectTransform.DOAnchorPos(new Vector2(0, -100), 0.5f);

        transform.DOScale(0f, .5f);
        _cg.DOFade(0f, .5f);

        await Task.Delay(TimeSpan.FromMilliseconds(500));
    }


    public async Task ComeBackBlast(bool isIntant = false)
    {
        var duration = isIntant ? 0f : .5f;

        if (_doBlastAnimTween.IsActive())
        {
            _doBlastAnimTween.Kill(true);
            _doBlastAnimTween = null;
        }

        if (_isPlayerBlast) _doBlastAnimTween = _blastImg.rectTransform.DOAnchorPos(new Vector2(-100, -100), duration);
        else _doBlastAnimTween = _blastImg.rectTransform.DOAnchorPos(new Vector2(100, 100), duration);

        _cg.DOFade(0f, duration);
        _blastImg.DOFade(0, duration);

        transform.DOScale(0f, duration);

        await Task.Delay(TimeSpan.FromMilliseconds(500));
    }

    public async Task ThrowBlast(bool isIntant = false)
    {
        var duration = isIntant ? 0f : .5f;

        if (_doBlastAnimTween.IsActive())
        {
            _doBlastAnimTween.Kill(true);
            _doBlastAnimTween = null;
        }

        if (_isPlayerBlast) _doBlastAnimTween = _blastImg.rectTransform.DOAnchorPos(Vector2.zero, duration);
        else _doBlastAnimTween = _blastImg.rectTransform.DOAnchorPos(Vector2.zero, duration);

        _cg.DOFade(1f, duration);
        _blastImg.DOFade(1f, duration);

        transform.DOScale(1f, duration);

        await Task.Delay(TimeSpan.FromMilliseconds(500));
    }
}
