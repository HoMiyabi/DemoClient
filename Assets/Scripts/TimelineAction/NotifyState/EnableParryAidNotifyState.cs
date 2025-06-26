using System.ComponentModel;
using UnityEngine;

namespace Kirara.TimelineAction
{
    [DisplayName("启用格挡支援")]
    public class EnableParryAidNotifyState : ActionNotifyState
    {
        public override void NotifyBegin(ActionPlayer player)
        {
            var ch = player.GetComponent<ChCtrl>();
            if (ch == null)
            {
                Debug.Log("ch == null");
                return;
            }
            ch.EnableParryAid = true;
        }

        public override void NotifyEnd(ActionPlayer player)
        {
            var ch = player.GetComponent<ChCtrl>();
            if (ch == null)
            {
                Debug.Log("ch == null");
                return;
            }
            ch.EnableParryAid = false;
        }
    }
}