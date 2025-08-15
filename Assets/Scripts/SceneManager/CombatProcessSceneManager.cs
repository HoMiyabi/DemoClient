using cfg.main;
using Kirara.Model;
using Kirara.TimelineAction;
using Kirara.UI;
using Manager;
using UnityEngine;

namespace Kirara
{
    public class CombatProcessSceneManager
    {
        private static bool GetIsCrit(Role role)
        {
            return Random.Range(0f, 1f) <= role.Set[EAttrType.CritRate];
        }

        private static double CalcDamage(Role role, double dmgMult, bool isCrit)
        {
            double damage = role.Set[EAttrType.Atk] * dmgMult;
            if (isCrit)
            {
                damage *= 1f + role.Set[EAttrType.CritDmg];
            }
            return damage;
        }

        private static double CalcDaze(Role role, double dazeMult)
        {
            double daze = role.Set[EAttrType.Impact] * dazeMult;
            return daze;
        }

        private static void CalcNumeric(Role role, double dmgMult, double dazeMult,
            out double damage, out double daze, out bool isCrit)
        {
            isCrit = GetIsCrit(role);
            damage = CalcDamage(role, dmgMult, isCrit);
            daze = CalcDaze(role, dazeMult);
        }

        private static void PlayVisual(RoleCtrl role, MonsterCtrl target, double dmg, bool isCrit, BoxPlayableAsset box)
        {
            // 命中特效
            // Debug.Log($"prefab={box.particlePrefab}, setRot={box.setRot}, rotValue={box.rotValue}");
            if (box.particlePrefab)
            {
                var vfxWPos = target.transform.position + new Vector3(0, 1f, 0);
                if (box.setRot)
                {
                    ParticleMgr.Instance.Play(box.particlePrefab, vfxWPos, role.transform.forward,
                        box.rotValue, box.rotMaxValue);
                }
                else
                {
                    ParticleMgr.Instance.Play(box.particlePrefab, vfxWPos);
                }
            }
        }

        public static void HandleRoleHitEachTarget(RoleCtrl role, MonsterCtrl target, BoxPlayableAsset box)
        {
            if (box.hitId == 0)
            {
                Debug.LogWarning("Hit Id == 0");
                return;
            }

            var config = ConfigMgr.tb.TbChHitNumericConfig[box.hitId];

            CalcNumeric(role.Role, config.DmgMult, config.DazeMult,
                out double dmg, out double daze, out bool isCrit);

            // target.TakeEffect(dmg, daze);
            NetFn.Send(new MsgMonsterTakeDamage
            {
                MonsterId = target.Model.MonsterId,
                Damage = (float)dmg,
                IsCrit = isCrit,
                Daze = (float)daze,
                HitGatherDist = box.hitGatherDist,
                CenterPos = role.transform.TransformPoint(box.center).Net(),
            });

            PlayVisual(role, target, dmg, isCrit, box);
        }

        public static void RoleAttack(RoleCtrl role, BoxPlayableAsset box)
        {
            int count = box.PhysicsOverlap(role.transform, LayerMask.GetMask("Monster"));

            role.lastHitMonsters.Clear();
            for (int i = 0; i < count; i++)
            {
                var col = box.cols[i];
                var monster = col.GetComponent<MonsterCtrl>();
                if (monster)
                {
                    HandleRoleHitEachTarget(role, monster, box);
                    role.lastHitMonsters.Add(monster);
                }
                else
                {
                    Debug.LogWarning("monster == null");
                }
            }
        }
    }
}