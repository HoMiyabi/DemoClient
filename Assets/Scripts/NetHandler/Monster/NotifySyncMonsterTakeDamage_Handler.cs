using Kirara.Network;

namespace Kirara.NetHandler.Monster
{
    public class NotifySyncMonsterTakeDamage_Handler : MsgHandler<NotifyMonsterTakeDamage>
    {
        protected override void Run(Session session, NotifyMonsterTakeDamage msg)
        {
            var instance = MonsterSystem.Instance;
            if (instance != null)
            {
                if (instance.monsters.TryGetValue(msg.MonsterId, out var monster))
                {
                    monster.TakeEffect(msg.Damage, 0f);
                }
            }
        }
    }
}