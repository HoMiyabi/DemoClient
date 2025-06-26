using System;
using UnityEngine;
using Cinemachine;

public class CinemachineZoomController : MonoBehaviour
{
    [Range(1, 8), SerializeField, Header("默认距离")]
    private float defaultDistance;

    [Range(1, 8), SerializeField, Header("最小距离")]
    private float minDistance;

    [Range(1, 8), SerializeField, Header("最大距离")]
    private float maxDistance;

    public float zoomSensitivity = 1;
    public float targetDistance;

    private CinemachineFramingTransposer cinemachineFramingTransposer;
    private CinemachineInputProvider cinemachineInputProvider;

    private void Awake()
    {
        cinemachineFramingTransposer = GetComponent<CinemachineVirtualCamera>()
            .GetCinemachineComponent<CinemachineFramingTransposer>();
        cinemachineInputProvider = GetComponent<CinemachineInputProvider>();
        targetDistance = defaultDistance;
    }

    private void Update()
    {
        float delta = -Math.Clamp(cinemachineInputProvider.GetAxisValue(2), -1, 1) * zoomSensitivity;
        targetDistance = Math.Clamp(targetDistance + delta, minDistance, maxDistance);
        cinemachineFramingTransposer.m_CameraDistance = targetDistance;
    }
}