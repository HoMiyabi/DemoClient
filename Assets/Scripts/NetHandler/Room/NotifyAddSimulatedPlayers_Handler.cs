using Kirara.Network;

namespace Kirara.NetHandler
{
    public class NotifyAddSimulatedPlayers_Handler : MsgHandler<NotifyAddSimulatedPlayers>
    {
        protected override void Run(Session session, NotifyAddSimulatedPlayers msg)
        {
            foreach (var simPlayer in msg.Players)
            {
                SimPlayerSystem.Instance.AddSimPlayer(simPlayer);
            }
        }
    }
}