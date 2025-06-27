using Kirara.Network;

namespace Kirara.NetHandler
{
    public class NotifyUpdateFromAuthority_Handler : MsgHandler<NotifyUpdateFromAuthority>
    {
        protected override void Run(Session session, NotifyUpdateFromAuthority msg)
        {
            if (!SimPlayerSystem.Instance.TryGetSimPlayer(msg.Player.Uid, out var simPlayer)) return;

            simPlayer.Update(msg.Player);
        }
    }
}