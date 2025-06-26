/*using Cinemachine;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Kirara
{
    public class CombatAttackParryAidState : CombatAttackState
    {
        private CinemachineVirtualCamera assistVCam;

        public override void OnEnter()
        {
            base.OnEnter();

            ch.PlayAction(ActionName.Attack_ParryAid_Start, 0f, () =>
            {
                sm.idle.Goto();
            });

            float leftDist = Vector3.Distance(
                sm.ch.leftAssistVCam.transform.position, sm.ch.vcam.transform.position);
            float rightDist = Vector3.Distance(
                sm.ch.rightAssistVCam.transform.position, sm.ch.vcam.transform.position);

            assistVCam = leftDist < rightDist ? sm.ch.leftAssistVCam : sm.ch.rightAssistVCam;

            assistVCam.enabled = true;
        }

        public async void TriggerParry()
        {
            sm.ch.ActionCtrl.ActionPlayer.Speed = 0f;
            await UniTask.WaitForSeconds(0.3f);
            sm.ch.ActionCtrl.ActionPlayer.Speed = 1f;
            sm.idle.Goto();
        }

        public override void OnExit()
        {
            base.OnExit();
            assistVCam.enabled = false;
        }
    }
}*/