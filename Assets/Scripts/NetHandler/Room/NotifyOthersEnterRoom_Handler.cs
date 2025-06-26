using Kirara.Network;

namespace Kirara.NetHandler
{
    public class NotifyOthersEnterRoom_Handler : MsgHandler<NotifyOthersEnterRoom>
    {
        protected override void Run(Session session, NotifyOthersEnterRoom message)
        {
            foreach (var playerInfo in message.PlayerInfos)
            {
                SimPlayerSystem.Instance.AddSimPlayer(playerInfo);
            }
        }
    }
}