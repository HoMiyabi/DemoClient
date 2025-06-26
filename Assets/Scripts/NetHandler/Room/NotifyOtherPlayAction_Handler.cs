using Kirara.Network;

namespace Kirara.NetHandler
{
    public class NotifyOtherPlayAction_Handler : MsgHandler<NotifyOtherPlayAction>
    {
        protected override void Run(Session session, NotifyOtherPlayAction message)
        {
            if (!SimPlayerSystem.Instance.TryGetSimPlayer(message.UId, out var simPlayer)) return;

            simPlayer.FrontCh.PlayAction(message.ActionName, 0.15f);
        }
    }
}