using BaseTemplate.Behaviours;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoSingleton<EnvironmentManager>
{
    [SerializeField] GameObject _environment;
    [SerializeField] Transform _meteoTransform;
    [SerializeField] SpriteRenderer _backgroundArea;

    public void Init()
    {
        GameStateManager.Instance.OnGameStateChanged += UpdateGameState;
    }

    public void SetMeteo(Meteo meteo)
    {
        foreach (Transform t in _meteoTransform)
        {
            Destroy(t.gameObject);
        }

        if (meteo != Meteo.None)
        {
            var meteoData = ResourceObjectHolder.Instance.GetResourceByType((ResourceType)meteo);

            var currentFX = Instantiate(meteoData.Prefab, _meteoTransform);
        }
    }

    public void SetDarkBackground(bool isDark)
    {
        if (isDark)
        {
            _backgroundArea.DOColor(new Color(0.4f, 0.4f, 0.4f, 1), .5f);
        }
        else
        {
            _backgroundArea.DOColor(Color.white, .5f);
        }
    }

    #region state

    public void UpdateGameState(GameState newState)
    {
        switch (newState)
        {
            case GameState.MENU:
                HandleMenu();
                break;
            case GameState.GAME:
                HandleGame();
                break;
            case GameState.END:
                HandleEnd();
                break;
            case GameState.WAIT:
                HandleWait();
                break;
            default:
                break;
        }
    }

    void HandleMenu()
    {
        _environment.SetActive(false);
    }

    void HandleGame()
    {
        _environment.SetActive(true);
    }

    void HandleEnd()
    {
    }

    void HandleWait()
    {
    }

    #endregion
}
