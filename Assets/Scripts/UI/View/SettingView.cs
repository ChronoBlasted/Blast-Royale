using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class SettingView : View
{
    [SerializeField] SwitchButtonLayout _music, _SFX;
    [SerializeField] AudioMixer _mixer;

    bool _isMusicOn;
    bool _isSfxOn;

    public override void Init()
    {
        gameObject.SetActive(true);

        StartCoroutine(InitCoroutine());
    }

    public void Close()
    {
        UIManager.Instance.ChangeView(UIManager.Instance.MenuView);
    }

    IEnumerator InitCoroutine()
    {
        yield return new WaitForEndOfFrame();

        _isMusicOn = SaveHandler.LoadValue("musicOnOff", true);
        _isSfxOn = SaveHandler.LoadValue("sfxOnOff", true);

        SetMusic(_isMusicOn);
        SetSFX(_isSfxOn);

        base.Init();
    }

    public override void OpenView(bool _instant = false)
    {
        base.OpenView(_instant);
    }

    public override void CloseView()
    {
        base.CloseView();
    }

    public void SwitchMusic()
    {
        _isMusicOn = !_isMusicOn;
        SetMusic(_isMusicOn);
    }

    public void SwitchSFX()
    {
        _isSfxOn = !_isSfxOn;
        SetSFX(_isSfxOn);
    }

    void SetMusic(bool OnOff)
    {
        if (OnOff)
        {
            _music.SetOn();
            _mixer.SetFloat("VolumeMusic", 0);
            SaveHandler.SaveValue("musicOnOff", true);
        }
        else
        {
            _music.SetOff();
            _mixer.SetFloat("VolumeMusic", -80);
            SaveHandler.SaveValue("musicOnOff", false);
        }
    }

    void SetSFX(bool OnOff)
    {
        if (OnOff)
        {
            _SFX.SetOn();
            _mixer.SetFloat("VolumeSFX", 0);
            SaveHandler.SaveValue("sfxOnOff", true);
        }
        else
        {
            _SFX.SetOff();
            _mixer.SetFloat("VolumeSFX", -80);
            SaveHandler.SaveValue("sfxOnOff", false);
        }
    }

    public void HandleJoinDiscord()
    {
        Application.OpenURL("https://discord.gg/ApzM4bQh8w");
    }
    public void HandleResetAccount()
    {
        UIManager.Instance.ConfirmPopup.OpenPopup();
        UIManager.Instance.ConfirmPopup.UpdateData("DELETE ACCOUNT", "Are you sure to delete your account ?", NakamaManager.Instance.NakamaUserAccount.DeleteAccount);
    }

    public void HandleOpenLanguagePopUp()
    {
        UIManager.Instance.LanguagePopup.OpenPopup();
    }
}
