using UnityEngine;

namespace Kirara.TimelineAction
{
    public class HoldCameraFollowNotifyState : ActionNotifyState
    {
        private Transform vcamFollow;
        private Vector3 worldPos;
        private Vector3 localPos;

        public override void NotifyBegin(ActionCtrl actionCtrl)
        {
            var ch = actionCtrl.GetComponent<RoleCtrl>();
            if (ch != null)
            {
                vcamFollow = ch.vcamFollow;
                worldPos = vcamFollow.position;
                localPos = vcamFollow.localPosition;
            }
            else
            {
                vcamFollow = null;
            }
        }

        public override void NotifyTick(ActionCtrl actionCtrl, float time)
        {
            if (vcamFollow != null)
            {
                vcamFollow.position = worldPos;
            }
        }

        public override void NotifyEnd(ActionCtrl actionCtrl)
        {
            if (vcamFollow != null)
            {
                vcamFollow.localPosition = localPos;
            }
        }
    }
}