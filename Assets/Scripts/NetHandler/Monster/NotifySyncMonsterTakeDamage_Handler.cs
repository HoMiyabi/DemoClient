using Kirara.Network;

namespace Kirara.NetHandler.Monster
{
    public class NotifySyncMonsterTakeDamage_Handler : MsgHandler<NotifySyncMonsterTakeDamage>
    {
        protected override void Run(Session session, NotifySyncMonsterTakeDamage message)
        {
            var instance = MonsterSystem.Instance;
            if (instance != null)
            {
                if (instance.monsters.TryGetValue(message.MonsterId, out var monster))
                {
                    monster.TakeEffect(message.Damage, 0f);
                }
            }
        }
    }
}