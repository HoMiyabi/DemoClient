using System;
using System.Collections.Generic;
using System.Linq;
using cfg.main;
using Cinemachine;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Kirara.AttrBuff;
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

        public float inputRotationSpeed = 15f;
        public float lookAtMonsterRotationSpeed = 20f;
        private Quaternion TargetRotation { get; set; }

        public Role Role { get; private set; }
        public CinemachineVirtualCamera VCam { get; set; }
        public Transform Cam { get; private set; }
        public ActionCtrl ActionCtrl { get; private set; }
        private Animator Animator { get; set; }
        private CharacterController CharacterController { get; set; }
        private ChGravity ChGravity { get; set; }

        private bool EnableRotation { get; set; }
        private bool EnableRecenter { get; set; }
        private bool EnableLookAtMonster { get; set; }

        public bool IsAttacking { get; set; }

        // public List<MonsterCtrl> lastHitMonsters = new();


        public NSyncRole SyncRole => new()
        {
            Id = Role.Id,
            Movement = new NMovement
            {
                Pos = new NVector3().Set(transform.position),
                Rot = new NVector3().Set(transform.rotation.eulerAngles)
            }
        };

        private void Awake()
        {
            Cam = Camera.main.transform;
            Animator = GetComponent<Animator>();

            CharacterController = GetComponent<CharacterController>();
            ChGravity = GetComponent<ChGravity>();

            ActionCtrl = GetComponent<ActionCtrl>();
            ActionCtrl.IsActionExecutable = IsActionExecutable;
            ActionCtrl.OnExecuteAction = OnExecuteAction;
            ActionCtrl.OnSetActionArgs = SetActionArgs;

            leftAssistVCam.enabled = false;
            rightAssistVCam.enabled = false;
        }

        private void OnExecuteAction(KiraraActionSO action, string actionName)
        {
            NetFn.Send(new MsgRolePlayAction
            {
                RoleId = Role.Id,
                ActionName = actionName
            });

            Role.Set.OnActionStart(new OnActionStartContext
            {
                actionType = action.actionType
            });
        }

        private bool IsActionExecutable(KiraraActionSO action)
        {
            int actionId = action.actionId;
            if (actionId == 0) return true;
            var config = ConfigMgr.tb.TbChActionNumericConfig[actionId];
            if (config.EnergyCost <= Role.Set[EAttrType.CurrEnergy])
            {
                return true;
            }
            return false;
        }

        public void Set(Role role)
        {
            Role = role;
            SetPos(role.Pos);
            transform.rotation = Quaternion.Euler(role.Rot);
        }

        private void Update()
        {
            UpdateRotation();
            UpdateRecenter();
            UpdateLookAtMonster();
        }

        private void SetActionArgs(ActionArgs actionArgs)
        {
            EnableRotation = actionArgs.enableRotation;
            EnableRecenter = actionArgs.enableRecenter;
            EnableLookAtMonster = actionArgs.enableLookAtMonster;
            SetShowState(actionArgs.roleShowState);
        }

        private void UpdateRecenter()
        {
            var pov = VCam.GetCinemachineComponent<CinemachinePOV>();
            bool enableRecenter = false;
            if (EnableRecenter)
            {
                var inputDir = PlayerSystem.Instance.input.Combat.Move.ReadValue<Vector2>();
                if (inputDir != Vector2.zero)
                {
                    float angle = Vector2.Angle(inputDir, Vector2.up);
                    if (angle is > 10f and < 170f)
                    {
                        enableRecenter = true;
                    }
                }
            }
            pov.m_HorizontalRecentering.m_enabled = enableRecenter;
        }

        private void UpdateRotation()
        {
            if (EnableRotation)
            {
                var inputDir = PlayerSystem.Instance.input.Combat.Move.ReadValue<Vector2>();
                if (inputDir != Vector2.zero)
                {
                    var camForward = Cam.transform.forward;
                    var camRight = Cam.transform.right;
                    camForward.y = 0;
                    camRight.y = 0;
                    camForward.Normalize();
                    camRight.Normalize();
                    var moveDirWS = camForward * inputDir.y + camRight * inputDir.x;
                    TargetRotation = Quaternion.LookRotation(moveDirWS, Vector3.up);
                }
                transform.rotation = Quaternion.Slerp(transform.rotation, TargetRotation, Time.deltaTime * inputRotationSpeed);
            }
        }

        // public void TriggerHitstopIfHitMonster(float duration, float speed)
        // {
        //     if (lastHitMonsters.Count > 0)
        //     {
        //         TriggerHitstop(duration, speed).Forget();
        //         foreach (var monster in lastHitMonsters)
        //         {
        //             monster.TriggerHitstop(duration, speed).Forget();
        //         }
        //     }
        // }

        public async UniTaskVoid TriggerHitstop(float duration, float speed)
        {
            if (duration <= 0f) return;

            ActionCtrl.Speed = speed;
            await UniTask.WaitForSeconds(duration);
            ActionCtrl.Speed = 1f;
        }

        private void OnAnimatorMove()
        {
            AddPos(Animator.deltaPosition);
            transform.rotation *= Animator.deltaRotation;
        }

        // public void PlaySFX(AudioClip clip)
        // {
        //     AudioMgr.Instance.PlaySFX(clip, transform.position);
        // }

        // public void LookAtMonster(float maxDist)
        // {
        //     var monster = MonsterSystem.Instance.ClosestMonster(transform.position, out float dist);
        //     if (monster == null) return;
        //
        //     if (dist < maxDist)
        //     {
        //         transform.DOLookAt(monster.transform.position,
        //             0.05f, AxisConstraint.None, transform.up);
        //     }
        // }

        private void UpdateLookAtMonster()
        {
            if (!EnableLookAtMonster) return;
            float maxDist = 15f;

            var monster = MonsterSystem.Instance.ClosestMonster(transform.position, out float dist);
            if (monster == null) return;

            if (dist < maxDist)
            {
                var dir = monster.transform.position - transform.position;
                dir.y = 0;
                TargetRotation = Quaternion.LookRotation(dir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, TargetRotation, Time.deltaTime * lookAtMonsterRotationSpeed);
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

        public void SetPos(Vector3 pos)
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

        public void AddPos(Vector3 delta)
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
        public void SwitchInParryAid(MonsterCtrl monster)
        {
            Debug.Log($"{name} 角色进入格挡");

            // 格挡
            // todo)) 格挡点数
            const float parryDist = 3.5f;
            // 面向敌人
            transform.forward = -monster.transform.forward;
            SetPos(monster.transform.position + monster.transform.forward * parryDist);

            // monster.ParryingRole = this;
            ActionCtrl.PlayAction(ActionName.Attack_ParryAid_Start);
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

        public void ShouldSwitchOutNormal()
        {
            if (!IsAttacking)
            {
                transform.DOKill();
                ActionCtrl.PlayAction(ActionName.SwitchOut_Normal);
            }
        }

        public void ShouldSwitchOutAided()
        {
            if (!IsAttacking)
            {
                ActionCtrl.PlayAction(ActionName.Background);
            }
        }

        public async UniTaskVoid EnterAttackParryAid()
        {
            Debug.Log($"{name} 角色成功格挡");
            ActionCtrl.PlayAction(ActionName.Attack_ParryAid_L);

            const float duration = 0.5f;
            ActionCtrl.Speed = 0f;
            await UniTask.WaitForSeconds(duration);
            ActionCtrl.Speed = 1f;
        }

        public void BoxBegin(BoxNotifyState box)
        {
            if (box.boxType == EBoxType.HitBox)
            {
                AttackProcessManager.RoleAttack(this, box, ActionCtrl.Action.actionType);
            }
        }

        public void ConsumeEnergy(float cost)
        {
            Role.Set[EAttrType.CurrEnergy] -= cost;
        }

        public void HandleMonsterAttackRole(NotifyMonsterAttackRole msg)
        {
            if (msg.Parried)
            {
                EnterAttackParryAid().Forget();
                var monsterCtrl = MonsterSystem.Instance.monsterCtrls[msg.MonsterId];
                monsterCtrl.EnterParried().Forget();
            }
            else
            {
                Debug.Log($"{name} 角色被攻击，伤害：{msg.Damage}");
                Role.Set[EAttrType.CurrHp] -= msg.Damage;
                if (ActionCtrl.TryGetAction(ActionName.Hit_L_Front, out _))
                {
                    ActionCtrl.PlayAction(ActionName.Hit_L_Front);
                }
            }
        }

        public void EnterAssistCamera()
        {
            float leftDist = Vector3.Distance(
                leftAssistVCam.transform.position, Cam.position);
            float rightDist = Vector3.Distance(
                rightAssistVCam.transform.position, Cam.position);

            var assistVCam = leftDist < rightDist ? leftAssistVCam : rightAssistVCam;

            assistVCam.enabled = true;
        }

        public void ExitAssistCamera()
        {
            leftAssistVCam.enabled = false;
            rightAssistVCam.enabled = false;
        }

        private Collider[] cols = new Collider[8];

        public bool TryTriggerPerfectDodge()
        {
            GetCharacterControllerPoints(out var p0, out var p1, out float radius);
            int size = Physics.OverlapCapsuleNonAlloc(p0, p1, radius, cols,
                LayerMask.GetMask("DodgeJudge"));
            if (size > 0)
            {
                Debug.Log("完美闪避");
                Debug.Log($"检测到碰撞体: {string.Join(", ", cols.Take(size).Select(x => x.name).ToArray())}");
                // 播放音效
                var clips = PlayerSystem.Instance.dodgeSuccessTipClips;
                AudioMgr.Instance.PlaySFX(clips.RandomItem(), transform.position);

                // 播放特效
                ParticleMgr.Instance.PlayAsChild(PlayerSystem.Instance.PerfectDodgeSparklePrefab, vcamFollow);

                // 后处理
                PostVolumeMgr.Instance.StartPerfectDodgeProfile();
                return true;
            }
            return false;
        }

        private void GetCharacterControllerPoints(out Vector3 p0, out Vector3 p1, out float radius)
        {
            var chCtrl = CharacterController;
            var offset = new Vector3(0f, chCtrl.height * 0.5f - chCtrl.radius, 0f);
            var p0Local = chCtrl.center - offset;
            var p1Local = chCtrl.center + offset;
            p0 = transform.TransformPoint(p0Local);
            p1 = transform.TransformPoint(p1Local);
            radius = chCtrl.radius;
        }
    }
}