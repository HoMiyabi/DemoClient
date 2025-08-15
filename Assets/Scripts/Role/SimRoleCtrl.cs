using System;
using System.Collections.Generic;
using Kirara.Model;
using Kirara.TimelineAction;
using UnityEngine;

namespace Kirara
{
    public class SimRoleCtrl : MonoBehaviour
    {
        public KiraraActionListSO actionList;
        private Dictionary<string, KiraraActionSO> ActionDict { get; set; }
        private Animator Animator { get; set; }
        private SimRole SimRole { get; set; }
        private ActionPlayer ActionPlayer { get; set; }
        private float followSpeed = 25f;

        private void Awake()
        {
            Animator = GetComponent<Animator>();
            ActionPlayer = GetComponent<ActionPlayer>();
            ActionDict = actionList.ActionDict;
        }

        public void Set(SimRole simRole)
        {
            SimRole = simRole;
            transform.position = simRole.Pos;
            transform.rotation = simRole.Rot;
        }

        private void Update()
        {
            transform.position = Vector3.Lerp(transform.position, SimRole.Pos, Time.deltaTime * followSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, SimRole.Rot, Time.deltaTime * followSpeed);
        }

        public void UpdateImmediate()
        {
            transform.position = SimRole.Pos;
            transform.rotation = SimRole.Rot;
        }

        private void OnAnimatorMove()
        {
            transform.position += Animator.deltaPosition;
            transform.rotation *= Animator.deltaRotation;
        }

        public void PlayAction(string actionName, float fadeDuration = 0f, Action onEnd = null)
        {
            var action = ActionDict[actionName];
            SetActionParams(action.actionParams);
            ActionPlayer.Play(action, actionName, fadeDuration, onEnd);
        }

        private void SetActionParams(ActionParams actionParams)
        {
            SetShowState(actionParams.roleShowState);
        }

        private void SetShowState(ERoleShowState state)
        {
            switch (state)
            {
                case ERoleShowState.Front:
                    gameObject.SetActive(true);
                    break;
                case ERoleShowState.Ghost:
                    gameObject.SetActive(true);
                    break;
                case ERoleShowState.Background:
                    gameObject.SetActive(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        //
        // public void AIControl()
        // {
        //     aiCtrl = true;
        //     PlayAction(ActionName.SwitchOut_Normal, 0f, () =>
        //     {
        //         gameObject.SetActive(false);
        //     });
        // }
        //
        // public void SimControl()
        // {
        //     gameObject.SetActive(true);
        //     aiCtrl = false;
        // }
    }
}