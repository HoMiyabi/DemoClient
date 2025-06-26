using System;
using Kirara.Model;
using UnityEngine;

namespace Kirara
{
    public class SimChCtrl : MonoBehaviour
    {
        public string characterName;

        private Animator animator;

        private Vector3 targetPos;
        private Quaternion targetRot;

        public SimChModel Model { get; private set; }
        private ActionCtrl actionCtrl;

        private bool aiCtrl = false;

        private void Awake()
        {
            actionCtrl = GetComponent<ActionCtrl>();
            animator = GetComponent<Animator>();
        }

        public void Set(SimChModel model)
        {
            Model = model;
        }

        public void SetImmediate(Vector3 position, Quaternion rotation)
        {
            targetPos = position;
            targetRot = rotation;
            transform.position = position;
            transform.rotation = rotation;
        }

        public void SetTarget(Vector3 newPos, Quaternion newRot)
        {
            targetPos = newPos;
            targetRot = newRot;
        }

        private void Update()
        {
            if (!aiCtrl)
            {
                transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 25f);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * 25f);
            }
        }

        private void OnAnimatorMove()
        {
            if (aiCtrl)
            {
                transform.position += animator.deltaPosition;
                transform.rotation *= animator.deltaRotation;
            }
        }

        public void PlayAction(string actionName, float fadeDuration = 0f, Action onEnd = null)
        {
            actionCtrl.ExecuteAction(actionName, fadeDuration, onEnd);
        }

        public void AIControl()
        {
            aiCtrl = true;
            PlayAction(ActionName.SwitchOut_Normal, 0f, () =>
            {
                gameObject.SetActive(false);
            });
        }

        public void SimControl()
        {
            gameObject.SetActive(true);
            aiCtrl = false;
        }
    }
}