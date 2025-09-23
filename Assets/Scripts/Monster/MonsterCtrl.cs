using System;
using System.Collections.Generic;
using System.Linq;
using cfg.main;
using Cysharp.Threading.Tasks;
using Kirara.Model;
using Kirara.TimelineAction;
using UnityEngine;

namespace Kirara
{
    public class MonsterCtrl : MonoBehaviour
    {
        public Transform statusBarFollow;
        public Transform attackLightFollow;
        public Transform indicatorFollow;

        [SerializeField] private BoxCollider boxCollider;
        [SerializeField] private SphereCollider sphereCollider;
        [SerializeField] private LineRenderer pathLineRenderer;

        // public RoleCtrl ParryingRole { get; set; }
        private CharacterController CharacterController { get; set; }
        public MonsterModel Model { get; private set; }
        private ActionCtrl ActionCtrl { get; set; }

        private List<Vector3> Path { get; set; } = new();

        private void Awake()
        {
            CharacterController = GetComponent<CharacterController>();
            ActionCtrl = GetComponent<ActionCtrl>();
            ActionCtrl.EnableFinishTransition = false;
        }

        public void Set(MonsterModel model)
        {
            Model = model;
        }

        public void HandleSelfTakeDamage(NotifyMonsterTakeDamage msg)
        {
            Model.Set[EAttrType.CurrHp] = msg.CurrHp;

            // Model.AttrSet[EAttrType.CurrHp] = Math.Max(0, Model.AttrSet[EAttrType.CurrHp] - damage);
            //
            // Model.AttrSet[EAttrType.CurrDaze] = Math.Min(
            //     Model.AttrSet[EAttrType.CurrDaze] + daze, Model.AttrSet[EAttrType.MaxDaze]);
        }

        public void HandleSelfDie()
        {
            Destroy(gameObject);
        }

        public async UniTaskVoid TriggerHitstop(float duration, float speed)
        {
            if (duration <= 0f) return;

            ActionCtrl.Speed = speed;
            await UniTask.WaitForSeconds(duration);
            ActionCtrl.Speed = 1f;
        }

        public async UniTaskVoid EnterParried()
        {
            const float duration = 0.5f;
            ActionCtrl.Speed = 0f;
            await UniTask.WaitForSeconds(duration);
            ActionCtrl.Speed = 1f;
            // todo)) 恢复进入Hit
            // MonsterAICtrl.EnterState(MonsterAICtrl.State.Hit);
        }

        public void Move(Vector3 value)
        {
            if (CharacterController)
            {
                CharacterController.Move(value);
            }
            else
            {
                transform.position += value;
            }
        }

        public void SetPos(Vector3 pos)
        {
            if (CharacterController)
            {
                CharacterController.enabled = false;
                transform.position = pos;
                CharacterController.enabled = true;
            }
            else
            {
                transform.position = pos;
            }
        }

        public void UpdateSync(NSyncMonster syncMonster)
        {
            SetPos(syncMonster.Pos.Unity());
            transform.rotation = syncMonster.Rot.Unity();
            // Debug.Log($"RepMovement, ThreadId: {Environment.CurrentManagedThreadId}, " +
            //           $"Pos: {syncMonster.Pos}, transform.position: {transform.position}");
        }

        public void PlayAction(string actionName, float fadeDuration = 0f, Action onFinish = null)
        {
            ActionCtrl.PlayActionFullName(actionName, fadeDuration, onFinish);
        }

        private void OnAnimatorMove()
        {

        }

        public void BeginDodgeDetect(BoxNotifyState box)
        {
            switch (box.boxShape)
            {
                case EBoxShape.Sphere:
                {
                    sphereCollider.center = box.center;
                    sphereCollider.radius = box.radius;

                    sphereCollider.enabled = true;
                    break;
                }
                case EBoxShape.Box:
                {
                    boxCollider.center = box.center;
                    boxCollider.size = box.size;

                    boxCollider.enabled = true;
                    break;
                }
                default:
                {
                    Debug.Log("未知Box形状");
                    break;
                }
            }
            MonsterSystem.Instance.DodgeDetectMonsters.Add(this);
        }

        public void EndDodgeDetect(BoxNotifyState box)
        {
            switch (box.boxShape)
            {
                case EBoxShape.Sphere:
                {
                    sphereCollider.enabled = false;
                    break;
                }
                case EBoxShape.Box:
                {
                    boxCollider.enabled = false;
                    break;
                }
                default:
                {
                    Debug.Log("未知Box形状");
                    break;
                }
            }
            MonsterSystem.Instance.DodgeDetectMonsters.Remove(this);
        }

        public void BoxBegin(BoxNotifyState box)
        {
            switch (box.boxType)
            {
                case EBoxType.HitBox:
                    BeginHit();
                    break;
                case EBoxType.DodgeBox:
                    BeginDodgeDetect(box);
                    break;
                default:
                    Debug.Log("未知Box类型");
                    break;
            }
        }

        public void BoxEnd(BoxNotifyState box)
        {
            if (box.boxType == EBoxType.DodgeBox)
            {
                EndDodgeDetect(box);
            }
        }

        private void BeginHit()
        {
            // if (ParryingRole != null)
            // {
            //     // 有角色在格挡自己
            //     Debug.Log("触发格挡");
            //     ParryingRole.EnterParryAid().Forget();
            //     EnterParried().Forget();
            //
            //     ParryingRole = null;
            // }
            // else
            // {
            //     int count = PhysicsOverlap(monsterCtrl.transform, LayerMask.GetMask("Character"));
            //     for (int i = 0; i < count; i++)
            //     {
            //         var col = cols[i];
            //         if (col.TryGetComponent<RoleCtrl>(out var roleCtrl))
            //         {
            //             var invAttr = roleCtrl.Role.Set[EAttrType.Invincible];
            //             if (invAttr == 0)
            //             {
            //                 Debug.Log($"Monster命中{roleCtrl.Role.Config.Name}");
            //             }
            //         }
            //     }
            // }
        }

        public void UpdatePath(NotifyMonsterPath notify)
        {
            // 绘制服务器下发的寻路路径
            Path.Clear();
            Path.AddRange(notify.Path.Select(v => v.Unity()));
            pathLineRenderer.positionCount = Path.Count;
            for (int i = 0; i < Path.Count; i++)
            {
                pathLineRenderer.SetPosition(i, Path[i]);
            }
        }
    }
}