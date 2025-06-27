using Kirara.Network;

namespace Kirara.NetHandler
{
    public class NotifyUpdateOther_Handler : MsgHandler<NotifyUpdateFromAuthority>
    {
        protected override void Run(NotifyUpdateOther message, Session session)
        {
            if (!SimPlayerSystem.Instance.TryGetSimPlayer(message.UId, out var simPlayer)) return;

            simPlayer.FrontCh.SetTarget(message.PosRot.Pos.Unity(), message.PosRot.Rot.Quat());
        }

        protected override void Run(Session session, NotifyUpdateFromAuthority msg)
        {

        }
    }
}