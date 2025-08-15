using Kirara.Network;

namespace Kirara.NetHandler.Monster
{
    public class NotifyUpdateMonster_Handler : MsgHandler<NotifyUpdateMonster>
    {
        protected override void Run(Session session, NotifyUpdateMonster msg)
        {
            foreach (var syncMonster in msg.Monsters)
            {
                if (MonsterSystem.Instance.monsters.TryGetValue(syncMonster.MonsterCid, out var monster))
                {
                    monster.UpdateSync(syncMonster);
                }
                else
                {
                    MonsterSystem.Instance.SpawnMonster(syncMonster);
                }
            }
        }
    }
}