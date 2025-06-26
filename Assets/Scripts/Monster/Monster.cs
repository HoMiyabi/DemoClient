using System;
using cfg.main;
using Cysharp.Threading.Tasks;

using Kirara.AttrEffect;
using Kirara.Manager;
using Kirara.TimelineAction;
using Manager;
using UnityEngine;

namespace Kirara
{
    public class Monster : MonoBehaviour
    {
        public string monsterName;
        public Transform statusBarFollow;
        public Transform attackLightFollow;
        public Transform indicatorFollow;

        public event Action onDie;

        private MonsterAICtrl monsterAICtrl;
        public AttrEffect.AttrEffect ae;

        public int monsterCid;
        public int monsterId;

        public ChCtrl parryingCh;

        public BoxCollider boxCollider;
        public SphereCollider sphereCollider;

        private ActionPlayer actionPlayer;
        private CharacterController characterController;

        private void Awake()
        {
            monsterAICtrl = GetComponent<MonsterAICtrl>();
            actionPlayer = GetComponent<ActionPlayer>();
            characterController = GetComponent<CharacterController>();
        }

        public void Set(int monsterConfigId, int monsterId)
        {
            this.monsterCid = monsterConfigId;
            this.monsterId = monsterId;
            InitAE();
        }

        private void InitAE()
        {
            var config = ConfigMgr.tb.TbMonsterConfig[monsterCid];
            ae = new AttrEffect.AttrEffect();
            ae.AddAttr(new Attr(EAttrType.Atk, config.Atk));
            ae.AddAttr(new Attr(EAttrType.Def, config.Def));
            ae.AddAttr(new Attr(EAttrType.Hp, config.Hp));
            ae.AddAttr(new Attr(EAttrType.MaxDaze, config.MaxDaze));
            ae.AddAttr(new Attr(EAttrType.StunDuration, config.StunDuration));
            ae.AddAttr(new Attr(EAttrType.StunDmgMultiplier, config.StunDmgMultiplier));

            ae.AddAttr(new Attr(EAttrType.CurrHp, config.Hp));
            ae.AddAttr(new Attr(EAttrType.CurrDaze, 0f));
        }

        private void HandleNumeric(float damage, float daze)
        {
            var currHpAttr = ae.GetAttr(EAttrType.CurrHp);
            currHpAttr.BaseValue = Mathf.Max(0f, currHpAttr.BaseValue - damage);

            var dazeAttr = ae.GetAttr(EAttrType.CurrDaze);
            var maxDazeAttr = ae.GetAttr(EAttrType.MaxDaze);
            dazeAttr.BaseValue = Mathf.Min(dazeAttr.BaseValue + daze, maxDazeAttr.Evaluate());

            NetFn.Send(new MsgMonsterTakeDamage
            {
                MonsterId = monsterId,
                Damage = damage
            });
        }

        public void TakeEffect(float damage, float daze)
        {
            HandleNumeric(damage, daze);
        }

        public void TakeEffect(float damage, float daze, Vector3 from)
        {
            HandleNumeric(damage, daze);

            monsterAICtrl.GetHit(from);
        }

        public void Die()
        {
            onDie?.Invoke();
            Destroy(gameObject);
        }

        public async UniTaskVoid TriggerHitstop(float duration, float speed)
        {
            if (duration <= 0f) return;

            actionPlayer.Speed = speed;
            await UniTask.WaitForSeconds(duration);
            actionPlayer.Speed = 1f;
        }

        public async UniTaskVoid EnterParried()
        {
            const float duration = 0.5f;
            actionPlayer.Speed = 0f;
            await UniTask.WaitForSeconds(duration);
            actionPlayer.Speed = 1f;
            monsterAICtrl.EnterState(MonsterAICtrl.State.Hit);
        }

        public void Move(Vector3 value)
        {
            characterController.Move(value);
        }
    }
}