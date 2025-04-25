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

    public void Init()
    {
        _startPos = _mainCamera.transform.position;
        _startSize = (int)_mainCamera.m_Lens.OrthographicSize;
    }

    public void SetCameraPosition(Vector3 position)
    {
        position.z = _startPos.z;
        _mainCamera.transform.DOMove(position, .5f);
    }

    public void SetCameraZoom(float zoom)
    {
        float currentZoom = _mainCamera.m_Lens.OrthographicSize;
        DOVirtual.Float(currentZoom, zoom, 0.5f, value =>
        {
            _mainCamera.m_Lens.OrthographicSize = value;
        });
    }

    public void AddCameraZoom(float zoomToAdd)
    {
        float targetZoom = _mainCamera.m_Lens.OrthographicSize + zoomToAdd;
        SetCameraZoom(targetZoom);
    }

    public void Reset()
    {
        SetCameraPosition(_startPos);
        SetCameraZoom(_startSize);
    }

    public void DoShakeCamera(float intensity = 4, float duration = .125f)
    {
        _cinemachineShake.ShakeCamera(intensity, duration);
    }
}
