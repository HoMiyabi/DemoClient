using Kirara.Network;

namespace Kirara.NetHandler.Monster
{
    public class NotifyMonsterPlayAction_Handler : MsgHandler<NotifyMonsterPlayAction>
    {
        protected override void Run(Session session, NotifyMonsterPlayAction msg)
        {
            if (string.IsNullOrEmpty(msg.ActionName)) return;
            if (MonsterSystem.Instance.monsters.TryGetValue(msg.MonsterId, out var monster))
            {
                monster.PlayAction(msg.ActionName, 0.15f);
            }
        }
    }
}