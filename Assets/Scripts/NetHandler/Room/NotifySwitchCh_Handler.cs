/*using Kirara.Network;

namespace Kirara.NetHandler
{
    public class NotifySwitchCh_Handler : MsgHandler<NotifySwitchCh>
    {
        protected override void Run(Session session, NotifySwitchCh message)
        {
            if (!SimPlayerSystem.Instance.TryGetSimPlayer(message.UId, out var simPlayer)) return;

            simPlayer.SwitchCh(message.Idx, message.Next);
        }
    }
}*/