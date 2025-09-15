using System.ComponentModel;

namespace Kirara.TimelineAction
{
    [DisplayName("控制格挡支援相机通知")]
    public class ControlAssistCameraNotify : ActionNotify
    {
        public bool enter;

        public override void Notify(ActionCtrl actionCtrl)
        {
            var roleCtrl = actionCtrl.GetComponent<RoleCtrl>();
            if (roleCtrl)
            {
                if (enter)
                {
                    roleCtrl.EnterAssistCamera();
                }
                else
                {
                    roleCtrl.ExitAssistCamera();
                }
            }
        }
    }
}