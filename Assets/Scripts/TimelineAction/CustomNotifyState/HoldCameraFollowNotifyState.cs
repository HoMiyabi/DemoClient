using UnityEngine;

namespace Kirara.TimelineAction
{
    public class HoldCameraFollowNotifyState : ActionNotifyState
    {
        private Transform vcamFollow;
        private Vector3 worldPos;
        private Vector3 localPos;

        public override void NotifyBegin(ActionPlayer player)
        {
            var ch = player.GetComponent<RoleCtrl>();
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

        public override void NotifyTick(ActionPlayer player, float time)
        {
            if (vcamFollow != null)
            {
                vcamFollow.position = worldPos;
            }
        }

        public override void NotifyEnd(ActionPlayer player)
        {
            if (vcamFollow != null)
            {
                vcamFollow.localPosition = localPos;
            }
        }
    }
}