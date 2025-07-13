using Kirara.Network;
using UnityEngine;

namespace Kirara.NetHandler
{
    public class NotifyUpdateFromAuthority_Handler : MsgHandler<NotifyUpdateFromAuthority>
    {
        protected override void Run(Session session, NotifyUpdateFromAuthority msg)
        {
            foreach (var syncPlayer in msg.Players)
            {
                if (syncPlayer.Uid == PlayerService.Player.Uid)
                {
                    continue;
                }
                if (SimPlayerSystem.Instance.TryGetSimPlayer(syncPlayer.Uid, out var simPlayer))
                {
                    simPlayer.Update(syncPlayer);
                }
                else
                {
                    SimPlayerSystem.Instance.AddSimPlayer(syncPlayer);
                }
            }
        }
    }
}