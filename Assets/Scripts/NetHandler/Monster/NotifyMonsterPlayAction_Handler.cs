using Kirara.Network;

namespace Kirara.NetHandler.Monster
{
    public class NotifyMonsterPlayAction_Handler : MsgHandler<NotifyMonsterPlayAction>
    {
        protected override void Run(Session session, NotifyMonsterPlayAction msg)
        {
            var monster = MonsterSystem.Instance.monsters[msg.MonsterId];
            monster.PlayAction(msg.ActionName, 0.15f);
        }
    }
}