using System;
using System.Collections.Generic;
using Cinemachine;
using Kirara.TimelineAction;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Kirara
{
    public class ActionEditorChCtrl : MonoBehaviour
    {
        public Transform follow;
        public Transform lookAt;
        public CinemachineVirtualCamera vcam;
        private GameInput input;
        private ActionCtrl actionCtrl;

        private Dictionary<EActionCommand, bool> commandPressed = new();

        private void Awake()
        {
            input = new GameInput();
            actionCtrl = GetComponent<ActionCtrl>();
        }

        public void OnEnable()
        {
            input.Enable();
        }

        public void OnDisable()
        {
            input.Disable();
        }

        private void Start()
        {
        }

        private void Update()
        {
            if (vcam == null)
            {
                vcam = GameObject.Find("第三人称VCam").GetComponent<CinemachineVirtualCamera>();
                vcam.Follow = follow;
                vcam.LookAt = lookAt;
            }
        }
    }
}