using System;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace Kirara.System
{
    public class DialogueSystem : UnitySingleton<DialogueSystem>
    {
        public delegate void OnDialogueFinishDel(int dialogueCid, Dictionary<string, int> blackBoard);

        public OnDialogueFinishDel OnDialogueFinish;

        #region 对话相机相关

        [SerializeField] private CinemachineVirtualCamera vcam;
        [SerializeField] private CinemachineTargetGroup targetGroup;

        public Quaternion[] dialogueVCamRots;

        private void Start()
        {
            targetGroup.AddMember(null, 0.5f, 0);
            targetGroup.AddMember(null, 0.5f, 0);
        }

        public void EnableDialogueVCam(Transform target1, Transform target2)
        {
            targetGroup.m_Targets[0].target = target1;
            targetGroup.m_Targets[1].target = target2;

            var forward = target2.position - target1.position;
            var lkRot = Quaternion.LookRotation(forward, Vector3.up);
            var vcam1 = PlayerSystem.Instance.vcam;

            float minDist = float.MaxValue;
            int minIdx = -1;
            for (int i = 0; i < dialogueVCamRots.Length; i++)
            {
                var rot = lkRot * dialogueVCamRots[i];
                targetGroup.transform.rotation = rot;
                vcam.InternalUpdateCameraState(Vector3.up, 0f);
                float dist = Vector3.Distance(vcam1.transform.position, vcam.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    minIdx = i;
                }
            }
            targetGroup.transform.rotation = lkRot * dialogueVCamRots[minIdx];

            vcam.enabled = true;
        }

        public void DisableDialogueVCam()
        {
            targetGroup.m_Targets[0].target = null;
            targetGroup.m_Targets[1].target = null;
            vcam.enabled = false;
        }

        #endregion

    }
}