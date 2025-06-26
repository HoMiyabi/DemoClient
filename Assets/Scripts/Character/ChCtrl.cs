using System;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using Animancer;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Kirara.Model;
using Kirara.TimelineAction;
using UnityEngine.Serialization;

namespace Kirara
{
    public class ChCtrl : MonoBehaviour
    {
        public int characterId;
        public string characterName;

        public Transform vcamFollow;
        public Transform vcamLookAt;

        public CinemachineVirtualCamera leftAssistVCam;
        public CinemachineVirtualCamera rightAssistVCam;
        public AudioClip[] hitClips;

        public ClipTransition clip;

        public Transform Cam { get; private set; }
        public Animator Animator { get; private set; }
        public RoleModel ChModel { get; private set; }
        // private CombatStateMachine combatStateMachine { get; set; }
        public CinemachineVirtualCamera VCam { get; set; }
        public CharacterController CharacterController { get; private set; }
        public ChGravity ChGravity { get; private set; }
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

        public ChCtrl Set(RoleModel chModel)
        {
            ChModel = chModel;
            return this;
        }

        private void Update()
        {
            ChModel.ae.Update();

            if (EnableRotation)
            {
                CharacterRotation();
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
            CharacterController.enabled = actionParams.enableCharacterController;
            gameObject.SetActive(actionParams.activeGameObject);
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

        private void CharacterRotation()
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
            if (CharacterController.enabled)
            {
                CharacterController.Move(Animator.deltaPosition);
            }
            else
            {
                transform.position += Animator.deltaPosition;
            }
            transform.rotation *= Animator.deltaRotation;
        }

        public void PlaySFX(AudioClip clip)
        {
            AudioMgr.Instance.PlaySFX(clip, transform.position);
        }

        public void PlayAction(string actionName, float fadeDuration = 0f, Action onEnd = null)
        {
            Debug.Log($"不再发送PlayAction {actionName}");
            // NetFn.SendPlayAction(actionName);
            ActionCtrl.ExecuteAction(actionName, fadeDuration, onEnd);
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
            CharacterController.enabled = true;
            ChGravity.enabled = true;
            gameObject.SetActive(true);
            ActionCtrl.ExecuteAction(ActionName.Idle);
        }

        public void InitBackground()
        {
            CharacterController.enabled = false;
            ChGravity.enabled = false;
            gameObject.SetActive(false);
            ActionCtrl.ExecuteAction(ActionName.Background);
        }

        // 格挡切入
        public void SwitchInParryAid(Monster monster)
        {
            Debug.Log($"{name} 角色进入格挡");
            ChGravity.enabled = true;
            gameObject.SetActive(true);
            CharacterController.enabled = false;

            // 格挡
            // todo)) 格挡点数
            const float parryDist = 3f;
            transform.forward = -monster.transform.forward;
            transform.position = monster.transform.position + monster.transform.forward * parryDist;
            ActionCtrl.ExecuteAction(ActionName.Attack_ParryAid_Start);
            monster.parryingCh = this;

            CharacterController.enabled = true;
        }

        private Vector3 switchNextLocalPos = new(1, 0, -1);
        private Vector3 switchPrevLocalPos = new(-1, 0, -1);

        // 普通切入
        public void SwitchInNormal(ChCtrl prev, bool isNext)
        {
            ChGravity.enabled = true;
            gameObject.SetActive(true);
            CharacterController.enabled = false;

            transform.forward = prev.transform.forward;
            CharacterController.enabled = false;
            transform.position = prev.transform.TransformPoint(isNext ? switchNextLocalPos : switchPrevLocalPos);
            CharacterController.enabled = true;

            ActionCtrl.ExecuteAction(ActionName.SwitchIn_Normal);

            CharacterController.enabled = true;
        }

        public void SwitchOutNormal()
        {
            CharacterController.enabled = false;
            ChGravity.enabled = false;
            transform.DOKill();
            ActionCtrl.ExecuteAction(ActionName.SwitchOut_Normal);
        }

        public void SwitchOutAided()
        {
            ActionCtrl.ExecuteAction(ActionName.Background);
        }

        public async UniTaskVoid EnterParryAid()
        {
            ActionCtrl.ExecuteAction(ActionName.Attack_ParryAid_H);

            const float duration = 0.5f;
            ActionCtrl.ActionPlayer.Speed = 0f;
            await UniTask.WaitForSeconds(duration);
            ActionCtrl.ActionPlayer.Speed = 1f;
        }
    }
}