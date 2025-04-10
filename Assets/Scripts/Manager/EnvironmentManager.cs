using BaseTemplate.Behaviours;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoSingleton<EnvironmentManager>
{
    [SerializeField] GameObject _environment;
    [SerializeField] Transform _meteoTransform;

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
