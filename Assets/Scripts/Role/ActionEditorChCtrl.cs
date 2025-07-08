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
        private ActionCtrl1 actionCtrl;

        private Dictionary<EActionCommand, bool> commandPressed = new();

        private void Awake()
        {
            input = new GameInput();
            actionCtrl = GetComponent<ActionCtrl1>();
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
            foreach (var action in input.Combat.Get().actions)
            {
                action.started += ctx =>
                {
                    // started中调用Disable会导致ActionId改变
                    if (ctx.phase == InputActionPhase.Disabled) return;
                    if (TryConvertCommand(ctx.action.id, out var command))
                    {
                        commandPressed[command] = true;
                    }
                    var phase = EActionCommandPhase.Down;
                    actionCtrl.Input(command, phase);
                };
                action.canceled += ctx =>
                {
                    // started中调用Disable会导致ActionId改变
                    if (ctx.phase == InputActionPhase.Disabled) return;

                    if (TryConvertCommand(ctx.action.id, out var command))
                    {
                        commandPressed[command] = false;
                    }
                    var phase = EActionCommandPhase.Up;
                    actionCtrl.Input(command, phase);
                };
            }
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

        public bool TryConvertCommand(Guid id, out EActionCommand command)
        {
            if (id == input.Combat.BaseAttack.id)
            {
                command = EActionCommand.BaseAttack;
            }
            else if (id == input.Combat.Dodge.id)
            {
                command = EActionCommand.Dodge;
            }
            else if (id == input.Combat.Move.id)
            {
                command = EActionCommand.Move;
            }
            else if (id == input.Combat.SpecialAttack.id)
            {
                command = EActionCommand.SpecialAttack;
            }
            else if (id == input.Combat.Ultimate.id)
            {
                command = EActionCommand.Ultimate;
            }
            else
            {
                command = EActionCommand.Always;
                return false;
            }
            return true;
        }
    }
}