using BaseTemplate.Behaviours;
using System;
using UnityEngine;

public class GameStateManager : MonoSingleton<GameStateManager>
{
    public event Action<GameState> OnGameStateChanged;
    GameState _gameState;

    public GameState GameState { get => _gameState; }

    public void UpdateGameState(GameState newState)
    {
        _gameState = newState;

        Debug.Log("New GameState : " + _gameState);

        switch (_gameState)
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

        OnGameStateChanged?.Invoke(_gameState);
    }

    void HandleMenu()
    {
    }

    void HandleGame()
    {
    }

    void HandleEnd()
    {
    }

    void HandleWait()
    {
    }

    public void UpdateStateToMenu() => UpdateGameState(GameState.MENU);
    public void UpdateStateToGame() => UpdateGameState(GameState.GAME);
    public void UpdateStateToEnd() => UpdateGameState(GameState.END);
    public void UpdateStateToWait() => UpdateGameState(GameState.WAIT);
}
