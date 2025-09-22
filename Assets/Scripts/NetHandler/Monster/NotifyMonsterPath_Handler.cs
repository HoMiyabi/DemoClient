using Kirara.Network;

namespace Kirara.NetHandler.Monster
{
    public class NotifyMonsterPath_Handler : MsgHandler<NotifyMonsterPath>
    {
        protected override void Run(Session session, NotifyMonsterPath msg)
        {
            if (MonsterSystem.Instance.monsterCtrls.TryGetValue(msg.MonsterId, out var monsterCtrl))
            {
                monsterCtrl.UpdatePath(msg);
            }
        }
    }
}