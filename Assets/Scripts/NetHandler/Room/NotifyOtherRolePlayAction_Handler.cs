using Kirara.Network;

namespace Kirara.NetHandler
{
    public class NotifyOtherRolePlayAction_Handler : MsgHandler<NotifyOtherRolePlayAction>
    {
        protected override void Run(Session session, NotifyOtherRolePlayAction msg)
        {
            if (!SimPlayerSystem.Instance.TryGetSimPlayer(msg.Uid, out var simPlayer)) return;

            simPlayer.RolePlayAction(msg.RoleId, msg.ActionName);
        }
    }
}