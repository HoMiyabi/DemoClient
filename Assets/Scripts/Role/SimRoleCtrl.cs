using System;
using Kirara.Model;
using Kirara.TimelineAction;
using UnityEngine;

namespace Kirara
{
    public class SimRoleCtrl : MonoBehaviour
    {
        private Animator Animator { get; set; }
        private SimRole SimRole { get; set; }
        private ActionCtrl ActionCtrl { get; set; }
        private float viewFollowSpeed = 25f;

        private void Awake()
        {
            Animator = GetComponent<Animator>();
            ActionCtrl = GetComponent<ActionCtrl>();
            ActionCtrl.OnSetActionArgs = SetActionArgs;
            ActionCtrl.EnableFinishTransition = false;
        }

        public void Set(SimRole simRole, string actionName)
        {
            SimRole = simRole;
            transform.position = simRole.Pos;
            transform.rotation = simRole.Rot;
            if (!string.IsNullOrEmpty(actionName))
            {
                PlayAction(actionName);
            }
        }

        private void Update()
        {
            transform.position = Vector3.Lerp(transform.position, SimRole.Pos, Time.deltaTime * viewFollowSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, SimRole.Rot, Time.deltaTime * viewFollowSpeed);
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

        public void PlayAction(string actionName, float fadeDuration = 0f, Action onFinish = null)
        {
            ActionCtrl.PlayAction(actionName, fadeDuration, onFinish);
        }

        private void SetActionArgs(ActionArgs actionArgs)
        {
            SetShowState(actionArgs.roleShowState);
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