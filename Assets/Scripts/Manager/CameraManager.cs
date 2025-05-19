using BaseTemplate.Behaviours;
using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraManager : MonoSingleton<CameraManager>
{
    [SerializeField] CinemachineVirtualCamera _mainCamera;
    [SerializeField] CinemachineShake _cinemachineShake;

    Vector3 _startPos;
    int _startSize;

    Tweener ZoomTween { get; set; }
    Tweener PosTween { get; set; }

    public Vector3 StartPos { get => _startPos; }

    public void Init()
    {
        _startPos = _mainCamera.transform.position;
        _startSize = (int)_mainCamera.m_Lens.OrthographicSize;
    }

    public void SetCameraPosition(Vector3 position, float duration = .5f)
    {
        PosTween.Kill(true);

        position.z = _startPos.z;
        PosTween = _mainCamera.transform.DOMove(position, duration);
    }

    public void SetCameraZoom(float zoom)
    {
        ZoomTween.Kill(true);
        _mainCamera.m_Lens.OrthographicSize = zoom;
    }

    public void SmoothCameraZoom(float zoom, float duration = .5f, Ease ease = Ease.OutSine)
    {
        ZoomTween.Kill(true);

        float currentZoom = _mainCamera.m_Lens.OrthographicSize;
        ZoomTween = DOVirtual.Float(currentZoom, zoom, duration, value =>
        {
            _mainCamera.m_Lens.OrthographicSize = value;
        }).SetEase(ease);
    }

    public void AddCameraZoom(float zoomToAdd)
    {
        ZoomTween.Kill(true);
        float targetZoom = _mainCamera.m_Lens.OrthographicSize + zoomToAdd;
        SmoothCameraZoom(targetZoom);
    }

    public void ResetCamera(float duration = .5f)
    {
        SetCameraPosition(_startPos);
        SmoothCameraZoom(_startSize, duration);
    }

    public void DoShakeCamera(float intensity = 4, float duration = .125f, float durationBeforeFade = 0f)
    {
        _cinemachineShake.ShakeCamera(intensity, duration, durationBeforeFade);
    }
}
