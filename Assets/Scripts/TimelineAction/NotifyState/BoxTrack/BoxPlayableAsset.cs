using System;
using System.ComponentModel;
using cfg.main;
using UnityEngine;
using UnityEngine.Playables;

namespace Kirara.TimelineAction
{
    [DisplayName("盒子")]
    public class BoxPlayableAsset : ActionNotifyState
    {
        [NonSerialized] public GameObject owner;

        public EBoxType boxType = EBoxType.HitBox;
        public EBoxShape boxShape = EBoxShape.Sphere;
        public Vector3 center = new(0, 1, 1.5f);
        public float radius = 1f;
        public Vector3 size = new(2, 2, 2);
        public EHitStrength hitStrength;
        public int hitId;
        public GameObject particlePrefab;
        public bool setRot;
        public float rotValue;
        public float rotMaxValue;
        public float hitGatherDist;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            this.owner = owner;
            return ScriptPlayable<BoxPlayable>.Create(graph);
        }

        public int PhysicsOverlap(Transform parent, int layerMask)
        {
            var worldPos = parent.TransformPoint(center);
            if (boxShape == EBoxShape.Sphere)
            {
                return Physics.OverlapSphereNonAlloc(worldPos, radius, cols, layerMask);
            }
            if (boxShape == EBoxShape.Box)
            {
                return Physics.OverlapBoxNonAlloc(worldPos, size / 2, cols, parent.rotation, layerMask);
            }
            Debug.LogError("BoxPlayableAsset.PhysicsOverlap: Unknown box type");
            return 0;
        }

        public readonly Collider[] cols = new Collider[16];
        private Monster monster;
        private ChCtrl ch;

        public override void NotifyBegin(ActionPlayer player)
        {
            monster = player.GetComponent<Monster>();
            if (monster != null)
            {
                MonsterBoxBegin();
                return;
            }

            ch = player.GetComponent<ChCtrl>();
            if (ch != null)
            {
                if (boxType == EBoxType.HitBox)
                {
                    ChHitBoxBegin();
                }
                else
                {
                    Debug.Log("未知Box类型");
                }
                return;
            }
            Debug.Log("未知类型");
        }

        public override void NotifyEnd(ActionPlayer player)
        {
            if (monster != null && boxType == EBoxType.DodgeBox)
            {
                MonsterDodgeBoxEnd();
            }

            // HitstopNotify不知道怎么不见了在Timeline里面
        }

        private void MonsterBoxBegin()
        {
            switch (boxType)
            {
                case EBoxType.HitBox:
                    MonsterHitBoxBegin();
                    break;
                case EBoxType.DodgeBox:
                    MonsterDodgeBoxBegin();
                    break;
                default:
                    Debug.Log("未知Box类型");
                    break;
            }
        }

        private void MonsterHitBoxBegin()
        {
            if (monster.parryingCh != null)
            {
                // 有角色在格挡自己
                Debug.Log("触发格挡");
                monster.parryingCh.EnterParryAid().Forget();
                monster.EnterParried().Forget();

                monster.parryingCh = null;
            }
            else
            {
                int count = PhysicsOverlap(monster.transform, LayerMask.GetMask("Character"));
                for (int i = 0; i < count; i++)
                {
                    var col = cols[i];
                    if (col.TryGetComponent<ChCtrl>(out var chCtrl))
                    {
                        var invAttr = chCtrl.ChModel.ae.GetAttr(EAttrType.Invincible);
                        if (invAttr.BaseValue == 0f)
                        {
                            Debug.Log($"Monster命中{chCtrl.characterName}");
                        }
                    }
                }
            }
        }

        private void MonsterDodgeBoxBegin()
        {
            switch (boxShape)
            {
                case EBoxShape.Sphere:
                {
                    // 这里认为开启闪避检测盒就是在攻击
                    monster.sphereCollider.center = center;
                    monster.sphereCollider.radius = radius;

                    monster.sphereCollider.enabled = true;
                    MonsterSystem.Instance.AttackingMonsters.Add(monster);
                    break;
                }
                case EBoxShape.Box:
                {
                    monster.boxCollider.center = center;
                    monster.boxCollider.size = size;

                    monster.boxCollider.enabled = true;
                    MonsterSystem.Instance.AttackingMonsters.Add(monster);
                    break;
                }
                default:
                {
                    Debug.Log("未知Box形状");
                    break;
                }
            }
        }

        private void MonsterDodgeBoxEnd()
        {
            switch (boxShape)
            {
                case EBoxShape.Sphere:
                {
                    monster.sphereCollider.enabled = false;
                    MonsterSystem.Instance.AttackingMonsters.Remove(monster);
                    break;
                }
                case EBoxShape.Box:
                {
                    monster.boxCollider.enabled = false;
                    MonsterSystem.Instance.AttackingMonsters.Remove(monster);
                    break;
                }
                default:
                {
                    Debug.Log("未知Box形状");
                    break;
                }
            }
        }

        private void ChHitBoxBegin()
        {
            CombatProcessSceneManager.ChHit(ch, this);
        }
    }
}