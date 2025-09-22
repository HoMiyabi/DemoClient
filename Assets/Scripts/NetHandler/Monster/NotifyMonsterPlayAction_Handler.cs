using Kirara.Network;
using UnityEngine;

namespace Kirara.NetHandler.Monster
{
    public class NotifyMonsterPlayAction_Handler : MsgHandler<NotifyMonsterPlayAction>
    {
        protected override void Run(Session session, NotifyMonsterPlayAction msg)
        {
            if (string.IsNullOrEmpty(msg.ActionName)) return;
            if (MonsterSystem.Instance.IdToMonsterCtrl.TryGetValue(msg.MonsterId, out var monster))
            {
                monster.PlayAction(msg.ActionName, 0);
            }
            else
            {
                Debug.LogWarning($"MonsterSystem.Instance.monsterCtrls.TryGetValue(msg.MonsterId, out var monster) == false");
            }
        }
    }
}