using Kirara.Network;

namespace Kirara.NetHandler
{
    public class NotifyOtherLeaveRoom_Handler : MsgHandler<NotifyOtherLeaveRoom>
    {
        protected override void Run(Session session, NotifyOtherLeaveRoom message)
        {
            SimPlayerSystem.Instance.RemoveSimPlayer(message.UId);
        }
    }
}