
using System.ComponentModel;

namespace Kirara.TimelineAction
{
    [DisplayName("正在攻击通知状态")]
    public class AttackingNotifyState : ActionNotifyState
    {
        public override void NotifyBegin(ActionCtrl actionCtrl)
        {
            base.NotifyBegin(actionCtrl);
            if (actionCtrl.TryGetComponent(out RoleCtrl roleCtrl))
            {
                roleCtrl.IsAttacking = true;
            }
        }

        public override void NotifyEnd(ActionCtrl actionCtrl)
        {
            base.NotifyEnd(actionCtrl);
            if (actionCtrl.TryGetComponent(out RoleCtrl roleCtrl))
            {
                roleCtrl.IsAttacking = false;
                if (roleCtrl != PlayerSystem.Instance.FrontRoleCtrl)
                {
                    actionCtrl.PlayAction(ActionName.Background);
                }
            }
        }
    }
}