
using Kirara.Network;

namespace Kirara.NetHandler.Monster
{
    public class NotifyMonsterDie_Handler : MsgHandler<NotifyMonsterDie>
    {
        protected override void Run(Session session, NotifyMonsterDie msg)
        {
            var instance = MonsterSystem.Instance;
            if (instance != null)
            {
                instance.MonsterDie(msg.MonsterId);
            }
        }
    }
}