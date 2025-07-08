using System.ComponentModel;

namespace Kirara.TimelineAction
{
    [DisplayName("取消窗口")]
    public class CancelWindowPlayableAsset : ActionNotifyState
    {
        public float inputBufferDuration = 0.1f;
        public CancelInfo cancelInfo;

        public override void NotifyBegin(ActionPlayer player)
        {
            var actionCtrl = player.GetComponent<ActionCtrl1>();
            if (!actionCtrl)
            {
                return;
            }
            actionCtrl.cancelWindowsAsset.Add(this);
        }

        public override void NotifyEnd(ActionPlayer player)
        {
            var actionCtrl = player.GetComponent<ActionCtrl1>();
            if (!actionCtrl)
            {
                return;
            }
            actionCtrl.cancelWindowsAsset.Remove(this);
        }

        public bool Check(EActionCommand command, EActionCommandPhase phase, float time)
        {
            return cancelInfo.command == command &&
                   cancelInfo.phase == phase &
                   time >= start &&
                   time < start + length;
        }

        public bool Inside(float time)
        {
            return time >= start &&
                   time < start + length;
        }
    }
}