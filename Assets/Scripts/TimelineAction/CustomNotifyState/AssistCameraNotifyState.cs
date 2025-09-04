namespace Kirara.TimelineAction
{
    public class AssistCameraNotifyState : ActionNotifyState
    {
        public override void NotifyBegin(ActionPlayer player)
        {
            base.NotifyBegin(player);
            var roleCtrl = player.GetComponent<RoleCtrl>();
            if (roleCtrl)
            {
                roleCtrl.EnterAssistCamera();
            }
        }

        public override void NotifyEnd(ActionPlayer player)
        {
            base.NotifyEnd(player);
            var roleCtrl = player.GetComponent<RoleCtrl>();
            if (roleCtrl)
            {
                roleCtrl.ExitAssistCamera();
            }
        }
    }
}