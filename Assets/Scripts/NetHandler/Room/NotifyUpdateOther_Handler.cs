using Kirara.Network;

namespace Kirara.NetHandler
{
    public class NotifyUpdateOther_Handler : MsgHandler<NotifyUpdateOther>
    {
        protected override void Run(Session session, NotifyUpdateOther message)
        {
            if (!SimPlayerSystem.Instance.TryGetSimPlayer(message.UId, out var simPlayer)) return;

            simPlayer.FrontCh.SetTarget(message.PosRot.Pos.Unity(), message.PosRot.Rot.Quat());
        }
    }
}