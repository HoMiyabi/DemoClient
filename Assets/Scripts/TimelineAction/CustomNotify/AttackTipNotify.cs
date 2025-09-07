using System.ComponentModel;

namespace Kirara.TimelineAction
{
    [DisplayName("攻击提示通知")]
    public class AttackTipNotify : ActionNotify
    {
        public bool canParry;

        public override void Notify(ActionPlayer player)
        {
            if (player.TryGetComponent<MonsterCtrl>(out var monsterCtrl))
            {
                monsterCtrl.DoAttackTip(canParry);
            }
        }
    }
}