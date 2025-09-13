using Kirara.Network;

namespace Kirara.NetHandler.Monster
{
    public class NotifyUpdateMonster_Handler : MsgHandler<NotifyUpdateMonster>
    {
        protected override void Run(Session session, NotifyUpdateMonster msg)
        {
            foreach (var syncMonster in msg.Monsters)
            {
                if (MonsterSystem.Instance.monsterCtrls.TryGetValue(syncMonster.MonsterCid, out var monster))
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