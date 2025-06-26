using Kirara.Network;

namespace Kirara.NetHandler
{
    public class NotifyOtherPlayAction_Handler : MsgHandler<NotifyOtherRolePlayAction>
    {
        protected override void Run(Session session, NotifyOtherRolePlayAction message)
        {
            if (!SimPlayerSystem.Instance.TryGetSimPlayer(message.Uid, out var simPlayer)) return;

            simPlayer.FrontCh.PlayAction(message.ActionName, 0.15f);
        }
    }
}