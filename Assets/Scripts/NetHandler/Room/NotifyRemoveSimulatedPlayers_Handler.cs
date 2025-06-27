using Kirara.Network;

namespace Kirara.NetHandler
{
    public class NotifyRemoveSimulatedPlayers_Handler : MsgHandler<NotifyRemoveSimulatedPlayers>
    {
        protected override void Run(Session session, NotifyRemoveSimulatedPlayers msg)
        {
            SimPlayerSystem.Instance.RemoveSimPlayer(msg.Uids);
        }
    }
}