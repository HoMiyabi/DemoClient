using Kirara.Network;
using UnityEngine;

namespace Kirara.NetHandler.Monster
{
    public class NotifySpawnMonster_Handler : MsgHandler<NotifySpawnMonster>
    {
        protected override void Run(Session session, NotifySpawnMonster msg)
        {
            var instance = MonsterSystem.Instance;
            if (instance != null)
            {
                Debug.Log("Spawn");
                instance.SpawnMonster(msg.MonsterCid, msg.MonsterId,
                    msg.Movement.Pos(), msg.Movement.Rot());
            }
        }
    }
}