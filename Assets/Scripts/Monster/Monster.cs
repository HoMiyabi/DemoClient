﻿using System;
using cfg.main;
using Cysharp.Threading.Tasks;
using Kirara.Model;
using Kirara.TimelineAction;
using UnityEngine;

namespace Kirara
{
    public class Monster : MonoBehaviour
    {
        public Transform statusBarFollow;
        public Transform attackLightFollow;
        public Transform indicatorFollow;

        public BoxCollider boxCollider;
        public SphereCollider sphereCollider;

        public event Action onDie;

        public RoleCtrl ParryingRole { get; set; }

        private MonsterAICtrl MonsterAICtrl { get; set; }
        private ActionPlayer ActionPlayer { get; set; }
        private CharacterController CharacterController { get; set; }
        public MonsterModel Model { get; private set; }

        private void Awake()
        {
            MonsterAICtrl = GetComponent<MonsterAICtrl>();
            ActionPlayer = GetComponent<ActionPlayer>();
            CharacterController = GetComponent<CharacterController>();
        }

        public void Set(MonsterModel model)
        {
            Model = model;
        }

        private void HandleNumeric(double damage, double daze)
        {
            Model.AttrSet[EAttrType.CurrHp] = Math.Max(0, Model.AttrSet[EAttrType.CurrHp] - damage);

            Model.AttrSet[EAttrType.CurrDaze] = Math.Min(
                Model.AttrSet[EAttrType.CurrDaze] + daze, Model.AttrSet[EAttrType.MaxDaze]);

            NetFn.Send(new MsgMonsterTakeDamage
            {
                MonsterId = Model.MonsterId,
                Damage = (float)damage
            });
        }

        public void TakeEffect(double damage, double daze)
        {
            HandleNumeric(damage, daze);
        }

        public void TakeEffect(double damage, double daze, Vector3 from)
        {
            HandleNumeric(damage, daze);

            MonsterAICtrl.GetHit(from);
        }

        public void Die()
        {
            onDie?.Invoke();
            Destroy(gameObject);
        }

        public async UniTaskVoid TriggerHitstop(float duration, float speed)
        {
            if (duration <= 0f) return;

            ActionPlayer.Speed = speed;
            await UniTask.WaitForSeconds(duration);
            ActionPlayer.Speed = 1f;
        }

        public async UniTaskVoid EnterParried()
        {
            const float duration = 0.5f;
            ActionPlayer.Speed = 0f;
            await UniTask.WaitForSeconds(duration);
            ActionPlayer.Speed = 1f;
            MonsterAICtrl.EnterState(MonsterAICtrl.State.Hit);
        }

        public void Move(Vector3 value)
        {
            CharacterController.Move(value);
        }
    }
}