using Kirara.Network;

namespace Kirara.NetHandler
{
    public class NotifyRoleTakeDamage_Handler : MsgHandler<NotifyRoleTakeDamage>
    {
        protected override void Run(Session session, NotifyRoleTakeDamage msg)
        {
            var roleCtrls = PlayerSystem.Instance.RoleCtrls;
            foreach (var roleCtrl in roleCtrls)
            {
                if (roleCtrl.Role.Id == msg.RoleId)
                {
                    roleCtrl.HandleTakeDamage(msg.Damage);
                }
            }
        }
    }
}