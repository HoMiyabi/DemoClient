using System.ComponentModel;

namespace Kirara.TimelineAction
{
    [DisplayName("控制支援相机通知状态")]
    public class AssistCameraNotifyState : ActionNotifyState
    {
        public override void NotifyBegin(ActionCtrl actionCtrl)
        {
            var roleCtrl = actionCtrl.GetComponent<RoleCtrl>();
            if (roleCtrl)
            {
                roleCtrl.EnterAssistCamera();
            }
        }

        public override void NotifyEnd(ActionCtrl actionCtrl)
        {
            var roleCtrl = actionCtrl.GetComponent<RoleCtrl>();
            if (roleCtrl)
            {
                roleCtrl.ExitAssistCamera();
            }
        }
    }
}