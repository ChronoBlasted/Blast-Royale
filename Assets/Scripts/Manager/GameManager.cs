using BaseTemplate.Behaviours;
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoSingleton<GameManager>
{
    void Awake()
    {
        UIManager.Instance.Init();

        TimeManager.Instance.Init();

        AudioManager.Instance.Init();

        PoolManager.Instance.Init();

        EnvironmentManager.Instance.Init();

        WildBattleManager.Instance.Init();

        CameraManager.Instance.Init();

        NakamaManager.Instance.Init();
    }

    public void AfterNakamaInit()
    {
        AdsManager.Instance.Init();

        GameStateManager.Instance.UpdateStateToMenu();
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.R))
        {
            ReloadScene();
        }
    }

    public void ReloadScene()
    {
        DOTween.KillAll();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitApp() => Application.Quit();
}