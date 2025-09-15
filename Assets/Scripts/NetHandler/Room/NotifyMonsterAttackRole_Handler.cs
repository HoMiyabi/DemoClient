using Kirara.Network;

namespace Kirara.NetHandler
{
    public class NotifyMonsterAttackRole_Handler : MsgHandler<NotifyMonsterAttackRole>
    {
        protected override void Run(Session session, NotifyMonsterAttackRole msg)
        {
            var roleCtrls = PlayerSystem.Instance.RoleCtrls;
            foreach (var roleCtrl in roleCtrls)
            {
                if (roleCtrl.Role.Id == msg.RoleId)
                {
                    roleCtrl.HandleMonsterAttackRole(msg);
                }
            }
        }
    }
}