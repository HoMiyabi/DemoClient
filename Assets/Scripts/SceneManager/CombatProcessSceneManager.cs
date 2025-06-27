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
        private static bool GetIsCrit(RoleModel ch)
        {
            return Random.Range(0f, 1f) <= ch.ae.GetAttr(EAttrType.CritRate).Evaluate();
        }

        private static float CalcDamage(RoleModel ch, float dmgMult, bool isCrit)
        {
            float dmg = ch.ae.GetAttr(EAttrType.Atk).Evaluate() * dmgMult;
            if (isCrit)
            {
                dmg *= 1f + ch.ae.GetAttr(EAttrType.CritDmg).Evaluate();
            }
            return dmg;
        }

        private static float CalcDaze(RoleModel ch, float dazeMult)
        {
            float daze = ch.ae.GetAttr(EAttrType.Impact).Evaluate() * dazeMult;
            return daze;
        }

        private static void CalcNumeric(RoleModel ch, float dmgMult, float dazeMult,
            out float dmg, out float daze, out bool isCrit)
        {
            isCrit = GetIsCrit(ch);
            dmg = CalcDamage(ch, dmgMult, isCrit);
            daze = CalcDaze(ch, dazeMult);
        }

        private static void PlayVisual(ChCtrl ch, Monster target, float dmg, bool isCrit, BoxPlayableAsset box)
        {
            // 命中特效
            // Debug.Log($"prefab={box.particlePrefab}, setRot={box.setRot}, rotValue={box.rotValue}");
            if (box.particlePrefab != null)
            {
                var vfxWPos = target.transform.position + new Vector3(0, 1f, 0);
                if (box.setRot)
                {
                    ParticleMgr.Instance.Play(box.particlePrefab, vfxWPos, ch.transform.forward,
                        box.rotValue, box.rotMaxValue);
                }
                else
                {
                    ParticleMgr.Instance.Play(box.particlePrefab, vfxWPos);
                }
            }

            // 伤害跳字
            var popupTextLocalPos = new Vector3(0, 1.5f, 0) + Random.insideUnitSphere * 0.3f;

            UIMgr.Instance.AddHUD<UIPopupText>().
                SetDamage(target.transform, popupTextLocalPos, dmg, isCrit).Play();
        }

        public static void HandleChSingleTarget(ChCtrl ch, Monster target, BoxPlayableAsset box)
        {
            if (box.hitId == 0)
            {
                Debug.LogWarning("Hit Id == 0");
                return;
            }

            var config = ConfigMgr.tb.TbChHitNumericConfig[box.hitId];

            CalcNumeric(ch.RoleModel, config.DmgMult, config.DazeMult,
                out float dmg, out float daze, out bool isCrit);

            target.TakeEffect(dmg, daze, (ch.transform.position - target.transform.position).normalized);

            PlayVisual(ch, target, dmg, isCrit, box);

            // 聚怪效果
            if (box.hitGatherDist != 0f)
            {
                var worldCenter = ch.transform.TransformPoint(box.center);

                // 移动向量的水平投影，最长不能超过v
                var v = (worldCenter - target.transform.position);
                v.y = 0f;

                float dist = Mathf.Min(box.hitGatherDist, v.magnitude); // 不能越过中心
                var dir = v.normalized; // 方向
                target.Move(dir * dist);
            }
        }

        public static void ChHit(ChCtrl ch, BoxPlayableAsset box)
        {
            int count = box.PhysicsOverlap(ch.transform, LayerMask.GetMask("Monster"));

            ch.lastHitMonsters.Clear();
            for (int i = 0; i < count; i++)
            {
                var col = box.cols[i];
                var mon = col.GetComponent<Monster>();
                if (mon != null)
                {
                    HandleChSingleTarget(ch, mon, box);
                    ch.lastHitMonsters.Add(mon);
                }
                else
                {
                    Debug.LogWarning("mon == null");
                }
            }
        }
    }
}