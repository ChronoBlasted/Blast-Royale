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

    GameObject _currentMeteoFX;

    public void Init()
    {
        GameStateManager.Instance.OnGameStateChanged += UpdateGameState;
    }

    public void SetMeteo(Meteo meteo)
    {
        Destroy(_currentMeteoFX);

        if (meteo != Meteo.None)
        {
            var meteoData = ResourceObjectHolder.Instance.GetResourceByType((ResourceType)meteo);

            _currentMeteoFX = Instantiate(meteoData.Prefab, _meteoTransform);
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

    public IEnumerator DOTravelWorldCor(TravelEffect travelEffect, Vector3 size, ResourceType resource, Transform startTransform, Transform endTransform, System.Action<bool> OnSpriteTravelStartOrFinish, float duration, int amountToSpawn)
    {
        int tempAmount = 0;

        for (int i = 0; i < amountToSpawn; i++)
        {
            Vector3 startWorldPos = startTransform.position;
            Vector3 endWorldPos = endTransform.position;

            GameObject go = PoolManager.Instance[resource].Get();

            go.transform.localScale = size;
            go.transform.position = startWorldPos;

            if (OnSpriteTravelStartOrFinish != null) OnSpriteTravelStartOrFinish.Invoke(true);

            Tween explodeMove = null;
            Tween travelTween = null;

            if (travelEffect == TravelEffect.EXPLODE)
            {
                Vector3 randomOffset = new Vector3(
                    Random.Range(-2f, 2f),
                    Random.Range(-2f, 2f),
                    0f);

                explodeMove = go.transform.DOMove(startWorldPos + randomOffset, 0.5f).SetEase(Ease.InOutSine);
            }

            travelTween = go.transform.DOMove(endWorldPos, duration).SetEase(
                travelEffect == TravelEffect.LINEAR ? Ease.Linear : Ease.InOutQuad
            );

            var seq = DOTween.Sequence();

            if (travelEffect == TravelEffect.EXPLODE)
            {
                seq.Join(explodeMove)
                   .Append(travelTween);
            }
            else
            {
                seq.Join(travelTween);
            }

            seq.OnComplete(() =>
            {
                if (OnSpriteTravelStartOrFinish != null) OnSpriteTravelStartOrFinish.Invoke(false);
                PoolManager.Instance[resource].Release(go);
                tempAmount++;
            });

            yield return new WaitForSeconds(Random.Range(0.05f, 0.1f));
        }
    }

}
