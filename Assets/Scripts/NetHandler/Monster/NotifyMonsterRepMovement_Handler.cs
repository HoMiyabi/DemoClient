using Cysharp.Threading.Tasks;
using Kirara.Network;

namespace Kirara.NetHandler.Monster
{
    public class NotifyMonsterRepMovement_Handler : MsgHandler<NotifyMonsterRepMovement>
    {
        protected override void Run(Session session, NotifyMonsterRepMovement msg)
        {
            foreach (var syncMonster in msg.Monsters)
            {
                var monster = MonsterSystem.Instance.monsters[syncMonster.MonsterId];
                monster.RepMovement(syncMonster);
            }
        }
    }
}