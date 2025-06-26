/*using cfg.main;

namespace Kirara
{
    public class CombatAttackState : CombatState
    {
        protected CharacterNumericConfig numeric;

        // 攻击第几段
        protected int atkIdx;

        // 最近一次攻击是否命中
        protected bool latestAtkHit;

        public override void OnEnter()
        {
            base.OnEnter();
            atkIdx = 0;
            latestAtkHit = false;
        }

        public override void OnAtk()
        {
            base.OnAtk();

            // 面向敌人 防止丢伤害
            ch.LookAtMonster(numeric.DetectionDistance);

            // 命中判定
            var monsters = ch.DetectHit();

            // 造成伤害
            float dmgMultiplier = numeric.DmgMultiplier;
            float dazeMultiplier = numeric.DazeMultiplier;
            foreach (var monster in monsters)
            {
                CombatProcessSceneManager.HandleAttack(ch, monster, dmgMultiplier, dazeMultiplier);
            }

            if (monsters.Count > 0)
            {
                ch.ChModel.ae.SendEvent("普通攻击命中");
                AudioMgr.Instance.PlaySFX(sm.ch.hitClips.RandomItem(), ch.transform.position);
            }
            latestAtkHit = monsters.Count > 0;

            atkIdx++;
        }

        public bool TryTriggerHitstop(float duration, float speed)
        {
            if (!latestAtkHit) return false;
            ch.TriggerHitstop(duration, speed).Forget();
            return true;
        }
    }
}*/