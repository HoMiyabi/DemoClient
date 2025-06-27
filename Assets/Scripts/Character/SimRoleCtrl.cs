using System;
using Kirara.Model;
using UnityEngine;

namespace Kirara
{
    public class SimRoleCtrl : MonoBehaviour
    {
        private Animator animator;

        private Vector3 targetPos;
        private Quaternion targetRot;

        public SimRole SimRole { get; private set; }
        private ActionCtrl actionCtrl;

        // private bool aiCtrl = false;

        private void Awake()
        {
            actionCtrl = GetComponent<ActionCtrl>();
            animator = GetComponent<Animator>();
        }

        public void Set(SimRole simRole)
        {
            SimRole = simRole;
        }

        private void Update()
        {
            // if (aiCtrl) return;

            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 25f);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * 25f);
        }

        public void UpdateImmediate()
        {
            transform.position = SimRole.Pos;
            transform.rotation = SimRole.Rot;
        }

        private void OnAnimatorMove()
        {
            // if (!aiCtrl) return;

            transform.position += animator.deltaPosition;
            transform.rotation *= animator.deltaRotation;
        }

        public void PlayAction(string actionName, float fadeDuration = 0f, Action onEnd = null)
        {
            actionCtrl.ExecuteAction(actionName, fadeDuration, onEnd);
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