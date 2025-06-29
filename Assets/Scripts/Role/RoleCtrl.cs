using System;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using Animancer;
using cfg.main;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Kirara.Model;
using Kirara.TimelineAction;
using Manager;

namespace Kirara
{
    public class RoleCtrl : MonoBehaviour
    {
        public Transform vcamFollow;
        public Transform vcamLookAt;

        public CinemachineVirtualCamera leftAssistVCam;
        public CinemachineVirtualCamera rightAssistVCam;
        public AudioClip[] hitClips;

        public ClipTransition clip;

        public Transform Cam { get; private set; }
        private Animator Animator { get; set; }
        public Role Role { get; private set; }
        // private CombatStateMachine combatStateMachine { get; set; }
        public CinemachineVirtualCamera VCam { get; set; }
        private CharacterController CharacterController { get; set; }
        private ChGravity ChGravity { get; set; }
        public ActionCtrl ActionCtrl { get; private set; }

        private bool EnableRotation { get; set; }
        private bool EnableRecenter { get; set; }
        public bool EnableParryAid { get; set; }

        public List<Monster> lastHitMonsters = new();

        public bool lookAtMonster;

        private void Awake()
        {
            InitRef();
        }

        public void Set(Role role)
        {
            Role = role;
            Role.RoleCtrl = this;
        }

        private void Update()
        {
            UpdateEnergyRegen();
            Role.ae.Update();

            if (EnableRotation)
            {
                ProcessRotation();
            }
            if (EnableRecenter)
            {
                Recenter();
            }
            if (lookAtMonster)
            {
                UpdateLookAtMonster(15f);
            }
        }

        public void SetActionParams(ActionParams actionParams)
        {
            EnableRotation = actionParams.enableRotation;
            EnableRecenter = actionParams.enableRecenter;
            lookAtMonster = actionParams.lookAtMonster;
            SetShowState(actionParams.roleShowState);
        }

        private void InitRef()
        {
            Cam = Camera.main.transform;
            Animator = GetComponent<Animator>();

            // combatStateMachine = GetComponent<CombatStateMachine>();
            CharacterController = GetComponent<CharacterController>();
            ChGravity = GetComponent<ChGravity>();

            ActionCtrl = GetComponent<ActionCtrl>();

            leftAssistVCam.enabled = false;
            rightAssistVCam.enabled = false;
        }

        private void Recenter()
        {
            var inputDir = PlayerSystem.Instance.input.Combat.Move.ReadValue<Vector2>();
            if (inputDir == Vector2.zero)
            {
                return;
            }

            var pov = VCam.GetCinemachineComponent<CinemachinePOV>();
            float angle = Vector2.Angle(inputDir, Vector2.up);
            if (angle is < 10f or > 170f)
            {
                pov.m_HorizontalRecentering.m_enabled = false;
            }
            else
            {
                pov.m_HorizontalRecentering.m_enabled = true;
            }
        }

        private void ProcessRotation()
        {
            var inputDir = PlayerSystem.Instance.input.Combat.Move.ReadValue<Vector2>();
            if (inputDir == Vector2.zero)
            {
                return;
            }

            var camForward = VCam.transform.forward;
            var camRight = VCam.transform.right;
            camForward.y = 0;
            camRight.y = 0;
            camForward.Normalize();
            camRight.Normalize();
            var wsMoveDir = camForward * inputDir.y + camRight * inputDir.x;
            var rot = Quaternion.LookRotation(wsMoveDir);

            transform.DOKill();
            transform.DORotateQuaternion(rot, 0.1f);
        }

        public void TryTriggerHitstop(float duration, float speed)
        {
            if (lastHitMonsters.Count > 0)
            {
                TriggerHitstop(duration, speed).Forget();
                foreach (var monster in lastHitMonsters)
                {
                    monster.TriggerHitstop(duration, speed).Forget();
                }
            }
        }

        private async UniTaskVoid TriggerHitstop(float duration, float speed)
        {
            if (duration <= 0f) return;

            ActionCtrl.ActionPlayer.Speed = speed;
            await UniTask.WaitForSeconds(duration);
            ActionCtrl.ActionPlayer.Speed = 1f;
        }

        private void OnAnimatorMove()
        {
            AddPos(Animator.deltaPosition);
            transform.rotation *= Animator.deltaRotation;
        }

        public void PlaySFX(AudioClip clip)
        {
            AudioMgr.Instance.PlaySFX(clip, transform.position);
        }

        public void LookAtMonster(float maxDist)
        {
            var monster = MonsterSystem.Instance.ClosestMonster(transform.position, out float dist);
            if (monster == null) return;

            if (dist < maxDist)
            {
                transform.DOLookAt(monster.transform.position,
                    0.05f, AxisConstraint.None, transform.up);
            }
        }

        public void UpdateLookAtMonster(float maxDist)
        {
            var monster = MonsterSystem.Instance.ClosestMonster(transform.position, out float dist);
            if (monster == null) return;

            const float rotSpeed = 20f;
            if (dist < maxDist)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation,
                    Quaternion.LookRotation(monster.transform.position - transform.position),
                    Time.deltaTime * rotSpeed);
            }
        }

        public void InitFront()
        {
            ActionCtrl.PlayAction(ActionName.Idle);
        }

        public void InitBackground()
        {
            ActionCtrl.PlayAction(ActionName.Background);
        }

        private void SetPos(Vector3 pos)
        {
            if (CharacterController.enabled)
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

        private void AddPos(Vector3 delta)
        {
            if (CharacterController.enabled)
            {
                CharacterController.Move(delta);
            }
            else
            {
                transform.position += delta;
            }
        }

        // 格挡切入
        public void SwitchInParryAid(Monster monster)
        {
            Debug.Log($"{name} 角色进入格挡");
            ChGravity.enabled = true;
            gameObject.SetActive(true);

            // 格挡
            // todo)) 格挡点数
            const float parryDist = 3f;
            transform.forward = -monster.transform.forward;
            SetPos(monster.transform.position + monster.transform.forward * parryDist);
            ActionCtrl.PlayAction(ActionName.Attack_ParryAid_Start);
            monster.ParryingRole = this;

        }

        private Vector3 switchNextLocalPos = new(1, 0, -1);
        private Vector3 switchPrevLocalPos = new(-1, 0, -1);

        private void SetShowState(ERoleShowState state)
        {
            switch (state)
            {
                case ERoleShowState.Front:
                    ChGravity.enabled = true;
                    CharacterController.enabled = true;
                    gameObject.SetActive(true);
                    break;
                case ERoleShowState.Ghost:
                    ChGravity.enabled = false;
                    CharacterController.enabled = false;
                    gameObject.SetActive(true);
                    break;
                case ERoleShowState.Background:
                    ChGravity.enabled = false;
                    CharacterController.enabled = false;
                    gameObject.SetActive(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        // 普通切入
        public void SwitchInNormal(RoleCtrl prev, bool isNext)
        {
            transform.forward = prev.transform.forward;
            SetPos(prev.transform.TransformPoint(isNext ? switchNextLocalPos : switchPrevLocalPos));

            ActionCtrl.PlayAction(ActionName.SwitchIn_Normal);
        }

        public void SwitchOutNormal()
        {
            transform.DOKill();
            ActionCtrl.PlayAction(ActionName.SwitchOut_Normal);
        }

        public void SwitchOutAided()
        {
            ActionCtrl.PlayAction(ActionName.Background);
        }

        public async UniTaskVoid EnterParryAid()
        {
            ActionCtrl.PlayAction(ActionName.Attack_ParryAid_H);

            const float duration = 0.5f;
            ActionCtrl.ActionPlayer.Speed = 0f;
            await UniTask.WaitForSeconds(duration);
            ActionCtrl.ActionPlayer.Speed = 1f;
        }

        // 更新能量恢复
        private void UpdateEnergyRegen()
        {
            const float mul = 8f;
            float maxEnergy = ConfigMgr.tb.TbGlobalConfig.ChMaxEnergy;

            var currEnergyAttr = Role.ae.GetAttr(EAttrType.CurrEnergy);
            if (currEnergyAttr.Evaluate() >= maxEnergy) return;

            float regen = Role.ae.GetAttr(EAttrType.EnergyRegen).Evaluate() * mul;
            currEnergyAttr.BaseValue = Mathf.Min(currEnergyAttr.BaseValue + Time.deltaTime * regen, maxEnergy);
        }
    }
}