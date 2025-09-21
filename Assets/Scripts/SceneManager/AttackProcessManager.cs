using cfg.main;
using Kirara.AttrBuff;
using Kirara.TimelineAction;
using Manager;
using UnityEngine;

namespace Kirara
{
    public static class AttackProcessManager
    {
        private static bool CalcIsCrit(AttrBuffSet set)
        {
            return Random.Range(0f, 1f) <= set[EAttrType.CritRate];
        }

        private static double CalcDamage(AttrBuffSet set, double dmgMult, bool isCrit)
        {
            double damage = set[EAttrType.Atk] * dmgMult;
            if (isCrit)
            {
                damage *= 1 + set[EAttrType.CritDmg];
            }
            damage *= 1 + set[EAttrType.ZengShang];
            return damage;
        }

        private static double CalcDaze(AttrBuffSet set, double dazeMult)
        {
            double daze = set[EAttrType.Impact] * dazeMult;
            return daze;
        }

        private static void CalcNumeric(AttrBuffSet set, double dmgMult, double dazeMult,
            out double damage, out double daze, out bool isCrit)
        {
            isCrit = CalcIsCrit(set);
            damage = CalcDamage(set, dmgMult, isCrit);
            daze = CalcDaze(set, dazeMult);
        }

        private static void PlayVisual(RoleCtrl role, MonsterCtrl target, double dmg, bool isCrit, BoxNotifyState box)
        {
            // 命中特效
            // Debug.Log($"prefab={box.particlePrefab}, setRot={box.setRot}, rotValue={box.rotValue}");
            if (box.particlePrefab)
            {
                var vfxWPos = target.transform.position + new Vector3(0, 1f, 0);
                if (box.setParticleRot)
                {
                    ParticleMgr.Instance.PlayAt(box.particlePrefab, vfxWPos, role.transform.forward,
                        box.rotValue, box.rotMaxValue);
                }
                else
                {
                    ParticleMgr.Instance.PlayAt(box.particlePrefab, vfxWPos);
                }
            }
        }

        public static void HandleRoleHitEachTarget(RoleCtrl role, MonsterCtrl target, BoxNotifyState box)
        {
            if (box.hitId == 0)
            {
                Debug.LogWarning("Hit Id == 0");
                return;
            }

            var config = ConfigMgr.tb.TbChHitNumericConfig[box.hitId];

            CalcNumeric(role.Role.Set, config.DmgMult, config.DazeMult,
                out double dmg, out double daze, out bool isCrit);

            // target.TakeEffect(dmg, daze);
            NetFn.Send(new MsgMonsterTakeDamage
            {
                MonsterId = target.Model.MonsterId,
                Damage = dmg,
                IsCrit = isCrit,
                Daze = daze,
                HitGatherDist = box.hitGatherDist,
                CenterPos = role.transform.TransformPoint(box.center).Net(),
                RolePos = role.transform.position.Net(),
            });

            PlayVisual(role, target, dmg, isCrit, box);
        }

        public static void RoleAttack(RoleCtrl role, BoxNotifyState box, string actionType)
        {
            int count = PhysicsOverlap(box, role.transform, LayerMask.GetMask("Monster"));

            bool hitMonster = false;
            for (int i = 0; i < count; i++)
            {
                var col = cols[i];
                if (col.TryGetComponent<MonsterCtrl>(out var monsterCtrl))
                {
                    hitMonster = true;
                    HandleRoleHitEachTarget(role, monsterCtrl, box);
                    monsterCtrl.TriggerHitstop(box.hitstopDuration, box.hitstopSpeed).Forget();
                }
                else
                {
                    Debug.LogWarning("Can't find monsterCtrl");
                }
            }
            if (hitMonster)
            {
                AudioMgr.Instance.PlaySFX(box.hitAudio, role.transform.position + Vector3.forward);
                role.TriggerHitstop(box.hitstopDuration, box.hitstopSpeed).Forget();

                role.Role.Set.OnAttackHit(new OnAttackHitContext
                {
                    actionType = actionType
                });
            }
        }

        private static readonly Collider[] cols = new Collider[128];

        private static int PhysicsOverlap(BoxNotifyState box, Transform parent, int layerMask)
        {
            var worldPos = parent.TransformPoint(box.center);
            if (box.boxShape == EBoxShape.Sphere)
            {
                return Physics.OverlapSphereNonAlloc(worldPos, box.radius, cols, layerMask);
            }
            if (box.boxShape == EBoxShape.Box)
            {
                return Physics.OverlapBoxNonAlloc(worldPos, box.size / 2, cols, parent.rotation, layerMask);
            }
            Debug.LogError("BoxPlayableAsset.PhysicsOverlap: Unknown box type");
            return 0;
        }
    }
}