using Kirara.Network;
using UnityEngine;

namespace Kirara.NetHandler.Monster
{
    public class NotifySpawnMonster_Handler : MsgHandler<NotifySpawnMonster>
    {
        protected override void Run(Session session, NotifySpawnMonster message)
        {
            var instance = MonsterSystem.Instance;
            if (instance != null)
            {
                Debug.Log("Spawn");
                instance.SpawnMonster(message.MonsterCid, message.MonsterId,
                    message.PosRot.Pos(), message.PosRot.Rot());
            }
        }
    }
}