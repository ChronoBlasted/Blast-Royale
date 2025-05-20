using Chrono.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlastInfoPopup : Popup
{
    [SerializeField] TMP_Text _blastNameTxt, _blastDescTxt, _blastLevel, _blastExp, _blastHp, _blastMana, _blastAttack, _blastDefense, _blastSpeed, _blastIv, _blastType;
    [SerializeField] Image _blastImg, _bgBlast, _blastTypeImg;
    [SerializeField] GameObject _prestigeEvolveLayout, _moveLayout;
    [SerializeField] CustomButton _prestigeButton, _evolveButton;
    [SerializeField] List<MoveLayout> movesLayout;

    Blast _currentBlast;
    BlastData _currentBlastData;
    NakamaData _nakamaData;

    public override void Init()
    {
        base.Init();

        _nakamaData = NakamaData.Instance;
    }

    public override void OpenPopup()
    {
        base.OpenPopup();
    }

    public override void ClosePopup()
    {
        base.ClosePopup();

        _currentBlast = null;
    }

    public void UpdateData(Blast blast)
    {
        _currentBlast = blast;
        _currentBlastData = _nakamaData.GetBlastDataById(blast.data_id);

        SetBlastUI(_currentBlastData, blast.Level, blast.GetRatioExp(), blast.GetRatioExpNextLevel());
        SetStatsUI(blast.MaxHp, blast.MaxMana, blast.Attack, blast.Defense, blast.Speed, blast.iv);
        SetTypeUI(_currentBlastData.type);
        SetMovesUI(blast.activeMoveset);

        _moveLayout.SetActive(true);
        //_prestigeEvolveLayout.SetActive(true);
    }

    public void UpdateData(BlastData blastData)
    {
        _currentBlastData = blastData;

        SetBlastUI(_currentBlastData);
        SetStatsUI(blastData.hp, blastData.mana, blastData.attack, blastData.defense, blastData.speed, -1);
        SetTypeUI(blastData.type);

        _moveLayout.SetActive(false);
        _prestigeEvolveLayout.SetActive(false);
    }

    private void SetBlastUI(BlastData blastData, int level = -1, float exp = -1, float nextExp = -1)
    {
        var refData = _nakamaData.GetBlastDataRef(blastData.id);
        _blastNameTxt.text = refData.Name.GetLocalizedString();
        _blastDescTxt.text = refData.Desc.GetLocalizedString();

        _blastLevel.text = level >= 0 ? $"Lvl.{level}" : "";
        _blastExp.text = (exp >= 0 && nextExp >= 0) ? $" EXP : {exp} / {nextExp}" : "";

        if (_currentBlast == null) _blastImg.sprite = refData.Sprite;
        else _blastImg.sprite = NakamaData.Instance.GetSpriteWithBlast(_currentBlast);
    }

    private void SetStatsUI(int hp, int mana, float attack, float defense, float speed, int Iv)
    {
        _blastHp.text = hp.ToString();
        _blastMana.text = mana.ToString();
        _blastAttack.text = attack.ToString();
        _blastDefense.text = defense.ToString();
        _blastSpeed.text = speed.ToString();
        if (Iv > 0) _blastIv.text = "IV:" + Iv;
        else _blastIv.text = "";
    }

    private void SetTypeUI(Type type)
    {
        _blastType.text = type.ToString();
        var typeData = ResourceObjectHolder.Instance.GetTypeDataByType(type);
        _blastTypeImg.sprite = typeData.Sprite;
        _bgBlast.color = typeData.Color;
    }

    private void SetMovesUI(List<int> moveset)
    {
        for (int i = 0; i < movesLayout.Count; i++)
        {
            if (i < moveset.Count)
            {
                movesLayout[i].gameObject.SetActive(true);
                movesLayout[i].Init(_nakamaData.GetMoveById(moveset[i]), null);
            }
            else
            {
                movesLayout[i].gameObject.SetActive(false);
            }
        }
    }


    public void HandleOnEvolve()
    {
        NakamaManager.Instance.NakamaUserAccount.EvolveBlast(_currentBlast.uuid);
    }

    public void HandleOnPrestige()
    {

    }

    public void HandleSwapMove(int indexMove)
    {
        UIManager.Instance.MoveSelectorPopup.UpdateData(_currentBlast, _nakamaData.GetMoveById(_currentBlast.activeMoveset[indexMove]), indexMove);
        UIManager.Instance.MoveSelectorPopup.OpenPopup();
    }
}
