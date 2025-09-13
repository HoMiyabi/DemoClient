using Kirara.Network;

namespace Kirara.NetHandler.Monster
{
    public class NotifyMonsterTakeDamage_Handler : MsgHandler<NotifyMonsterTakeDamage>
    {
        protected override void Run(Session session, NotifyMonsterTakeDamage msg)
        {
            MonsterSystem.Instance.MonsterTakeDamage(msg);
        }
    }
}