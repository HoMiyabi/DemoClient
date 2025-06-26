using System.ComponentModel;
using UnityEngine;

namespace Kirara.TimelineAction
{
    [DisplayName("命中停顿标记")]
    public class HitstopNotify1 : ActionNotify
    {
        [SecondFrame(60)]
        public float duration = 0.05f;
        public float animationSpeed;

        public override void Notify(ActionPlayer player)
        {
            var ch = player.GetComponent<ChCtrl>();
            if (ch != null)
            {
                ch.TryTriggerHitstop(duration, animationSpeed);
            }
            else
            {
                Debug.Log("ch == null");
            }
        }
    }
}