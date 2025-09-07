using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Kirara.TimelineAction
{
    [DisplayName("相机震动通知")]
    public class CameraShakeMarker : ActionNotify
    {
        public float angle;
        public float speed;
        public PropertyName id { get; }
        public NotificationFlags flags => NotificationFlags.Retroactive | NotificationFlags.TriggerInEditMode;

        public void PlayInTimelineEditor(Playable origin)
        {
            var instance = CinemachineImpulseController.Instance;
            if (instance != null)
            {
                instance.GenerateImpulse(angle, speed);
            }
        }

        public override void Notify(ActionCtrl actionCtrl)
        {
            CinemachineImpulseController.Instance
                .GenerateImpulse(angle, speed);
        }
    }
}