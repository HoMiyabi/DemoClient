namespace Kirara.TimelineAction
{
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