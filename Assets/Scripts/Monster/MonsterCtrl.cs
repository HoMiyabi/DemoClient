using System;
using cfg.main;
using Cysharp.Threading.Tasks;
using Kirara.Model;
using Kirara.TimelineAction;
using Kirara.UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Kirara
{
    public class MonsterCtrl : MonoBehaviour
    {
        public Transform statusBarFollow;
        public Transform attackLightFollow;
        public Transform indicatorFollow;

        public BoxCollider boxCollider;
        public SphereCollider sphereCollider;

        public event Action onDie;

        public RoleCtrl ParryingRole { get; set; }
        private CharacterController CharacterController { get; set; }
        public MonsterModel Model { get; private set; }
        public ActionCtrl ActionCtrl { get; private set; }

        private void Awake()
        {
            CharacterController = GetComponent<CharacterController>();
            ActionCtrl = GetComponent<ActionCtrl>();
        }

        public void Set(MonsterModel model)
        {
            Model = model;
        }

        public void HandleTakeDamage(NotifyMonsterTakeDamage msg)
        {
            Model.Set[EAttrType.CurrHp] = msg.CurrHp;

            // 伤害跳字
            var popupTextLocalPos = new Vector3(0, 1.5f, 0) + Random.insideUnitSphere * 0.3f;

            UIMgr.Instance.AddHUD<UIPopupText>().
                SetDamage(transform, popupTextLocalPos, msg.Damage, msg.IsCrit).Play();

            // Model.AttrSet[EAttrType.CurrHp] = Math.Max(0, Model.AttrSet[EAttrType.CurrHp] - damage);
            //
            // Model.AttrSet[EAttrType.CurrDaze] = Math.Min(
            //     Model.AttrSet[EAttrType.CurrDaze] + daze, Model.AttrSet[EAttrType.MaxDaze]);
        }

        public void Die()
        {
            onDie?.Invoke();
            Destroy(gameObject);
        }

        public async UniTaskVoid TriggerHitstop(float duration, float speed)
        {
            if (duration <= 0f) return;

            ActionCtrl.ActionPlayer.Speed = speed;
            await UniTask.WaitForSeconds(duration);
            ActionCtrl.ActionPlayer.Speed = 1f;
        }

        public async UniTaskVoid EnterParried()
        {
            const float duration = 0.5f;
            ActionCtrl.ActionPlayer.Speed = 0f;
            await UniTask.WaitForSeconds(duration);
            ActionCtrl.ActionPlayer.Speed = 1f;
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
            ActionCtrl.PlayAction(actionName, fadeDuration, onFinish);
        }

        private void OnAnimatorMove()
        {

        }
    }
}