using Kirara.Network;

namespace Kirara.NetHandler.Monster
{
    public class NotifyMonsterTakeDamage_Handler : MsgHandler<NotifyMonsterTakeDamage>
    {
        protected override void Run(Session session, NotifyMonsterTakeDamage msg)
        {
            if (MonsterSystem.Instance.monsters.TryGetValue(msg.MonsterId, out var monster))
            {
                monster.HandleTakeDamage(msg);
            }
        }
    }
}