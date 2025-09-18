using System;
using System.Collections.Generic;
using System.Threading;
using Cinemachine;
using Cysharp.Threading.Tasks;
using Kirara.TimelineAction;
using Kirara.UI.Panel;
using UnityEngine;
using UnityEngine.InputSystem;
using YooAsset;

namespace Kirara
{
    public class PlayerSystem : UnitySingleton<PlayerSystem>
    {
        public AudioClip[] dodgeSuccessTipClips;
        public GameObject PerfectDodgeSparklePrefab;

        [SerializeField] private Transform RoleParent;
        [SerializeField] public CinemachineVirtualCamera vcam;
        [SerializeField] private GameObject playerPrefab;
        public GameObject Player { get; private set; }

        public GameInput input { get; private set; }

        private CancellationTokenSource cts;

        public List<RoleCtrl> RoleCtrls;
        public RoleCtrl FrontRoleCtrl => RoleCtrls[FrontRoleIdx];

        private int _frontRoleIdx = -1;
        public int FrontRoleIdx
        {
            get => _frontRoleIdx;
            private set
            {
                if (_frontRoleIdx == value) return;
                _frontRoleIdx = value;

                var roleCtrl = RoleCtrls[_frontRoleIdx];
                roleCtrl.VCam = vcam;
                vcam.Follow = roleCtrl.vcamFollow;
                vcam.LookAt = roleCtrl.vcamLookAt;

                OnFrontRoleChanged?.Invoke();
            }
        }
        public event Action OnFrontRoleChanged;

        public string FrontRoleId
        {
            get => FrontRoleCtrl.Role.Id;
            set
            {
                FrontRoleIdx = PlayerService.Player.Roles.FindIndex(x => x.Id == value);
            }
        }

        public bool switchEnabled = true;

        private bool _enableInput = true;
        public bool EnableInput
        {
            get => _enableInput;
            set
            {
                _enableInput = value;
                vcam.GetComponent<CinemachineInputProvider>().enabled = value;
                if (value)
                {
                    input.Combat.Enable();
                }
                else
                {
                    input.Combat.Disable();
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();

            RoleCtrls = new List<RoleCtrl>();
            input = new GameInput();
            cts = new CancellationTokenSource();
        }

        private void OnDestroy()
        {
            cts.Cancel();
            input.Dispose();
        }

        private int updateInterval = 16;

        private async UniTaskVoid RepeatSendUpdateFromAutonomous()
        {
            var req = new MsgUpdateFromAutonomous
            {
                Player = new NSyncPlayer
                {
                    Uid = PlayerService.Player.Uid,
                }
            };
            var token = cts.Token;

            while (!token.IsCancellationRequested)
            {
                await UniTask.Delay(updateInterval, true, PlayerLoopTiming.Update, token);
                if (token.IsCancellationRequested)
                {
                    return;
                }
                req.Player.Roles.Clear();
                foreach (var roleCtrl in RoleCtrls)
                {
                    req.Player.Roles.Add(roleCtrl.SyncRole);
                }
                NetFn.Send(req);
            }
        }

        public int GetNext(int idx)
        {
            return (idx + 1) % RoleCtrls.Count;
        }

        public int GetPrev(int idx)
        {
            return (idx - 1 + RoleCtrls.Count) % RoleCtrls.Count;
        }

        private void Start()
        {
            var handle = YooAssets.LoadAssetSync<AudioClip>("music1");
            AudioMgr.Instance.PlayMusic(handle.AssetObject as AudioClip);

            Init();

            // 输入
            input.Combat.SwitchCharactersNext.started += HandleSwitchRoleNext;
            input.Combat.SwitchCharactersPrevious.started += HandleSwitchRolePrev;
            foreach (var action in input.Combat.Get().actions)
            {
                action.started += HandleStartedInputToFrontCommand;
                action.canceled += HandleCanceledInputToFrontCommand;
            }

            Player = Instantiate(playerPrefab, transform);
        }

        private static bool TryConvertInputActionToCommand(Guid id, out EActionCommand command)
        {
            if (id == Instance.input.Combat.BaseAttack.id)
            {
                command = EActionCommand.BaseAttack;
            }
            else if (id == Instance.input.Combat.Dodge.id)
            {
                command = EActionCommand.Dodge;
            }
            else if (id == Instance.input.Combat.Move.id)
            {
                command = EActionCommand.Move;
            }
            else if (id == Instance.input.Combat.SpecialAttack.id)
            {
                command = EActionCommand.SpecialAttack;
            }
            else if (id == Instance.input.Combat.Ultimate.id)
            {
                command = EActionCommand.Ultimate;
            }
            else
            {
                command = EActionCommand.None;
                return false;
            }
            return true;
        }

        private void Init()
        {
            CreateTeamRoles();
            NetFn.Send(new MsgEnterRoom());

            UIMgr.Instance.PushPanel<PopupTextPanel>(UILayer.HUD);
            UIMgr.Instance.PushPanel<AttackTipPanel>(UILayer.HUD);
            UIMgr.Instance.PushPanel<CombatPanel>();

            FrontRoleId = PlayerService.Player.FrontRoleId;

            for (int i = 0; i < RoleCtrls.Count; i++)
            {
                if (i == FrontRoleIdx)
                {
                    RoleCtrls[i].InitFront();
                }
                else
                {
                    RoleCtrls[i].InitBackground();
                }
            }

            RepeatSendUpdateFromAutonomous().Forget();
        }

        private void HandleStartedInputToFrontCommand(InputAction.CallbackContext ctx)
        {
            // started中调用Disable会导致ActionId改变
            if (ctx.phase == InputActionPhase.Disabled) return;
            if (TryConvertInputActionToCommand(ctx.action.id, out var command))
            {
                FrontRoleCtrl.ActionCtrl.InputCommand(command, EActionCommandPhase.Down);
                // pressedDict[command] = true;
            }
        }

        private void HandleCanceledInputToFrontCommand(InputAction.CallbackContext ctx)
        {
            // started中调用Disable会导致ActionId改变
            if (ctx.phase == InputActionPhase.Disabled) return;

            if (TryConvertInputActionToCommand(ctx.action.id, out var command))
            {
                FrontRoleCtrl.ActionCtrl.InputCommand(command, EActionCommandPhase.Up);
                // pressedDict[command] = false;
            }
        }

        private void HandleSwitchRoleNext(InputAction.CallbackContext ctx)
        {
            if (!switchEnabled) return;

            int idx = GetNext(FrontRoleIdx);
            SwitchRole(idx, true);
        }

        private void HandleSwitchRolePrev(InputAction.CallbackContext ctx)
        {
            if (!switchEnabled) return;

            int idx = GetPrev(FrontRoleIdx);
            SwitchRole(idx, false);
        }

        private void SwitchRole(int idx, bool isNext)
        {
            var prev = FrontRoleCtrl;
            FrontRoleIdx = idx;

            NetFn.Send(new MsgSwitchRole
            {
                FrontRoleId = FrontRoleCtrl.Role.Id
            });

            var monster = MonsterSystem.Instance.ClosestDodgeDetectMonster(prev.transform.position, out float dist);
            if (monster != null)
            {
                prev.ShouldSwitchOutAided();
                FrontRoleCtrl.SwitchInParryAid(monster);
            }
            else
            {
                prev.ShouldSwitchOutNormal();
                FrontRoleCtrl.SwitchInNormal(prev, isNext);
            }
        }

        private void CreateTeamRoles()
        {
            var teamRoleIds = PlayerService.Player.TeamRoleIds;
            var roles = PlayerService.Player.Roles;
            Debug.Log("拥有角色数量：" + roles.Count);
            foreach (string roleId in teamRoleIds)
            {
                var role = roles.Find(it => it.Id == roleId);
                Debug.Log($"加载角色 {role.Config.Name}");
                var handle = YooAssets.LoadAssetSync<GameObject>(role.Config.PrefabLoc);
                var go = handle.InstantiateSync(RoleParent, false);
                var roleCtrl = go.GetComponent<RoleCtrl>();
                handle.Release();
                roleCtrl.Set(role);

                RoleCtrls.Add(roleCtrl);
            }
        }

        private void Update()
        {
            Player.transform.position = FrontRoleCtrl.transform.position;
            if (EnableInput)
            {
                UpdateActionCtrlInputPress();
            }

            foreach (var roleCtrl in RoleCtrls)
            {
                roleCtrl.Role.Set.Update(Time.deltaTime);
            }
        }

        private void UpdateActionCtrlInputPress()
        {
            // Debug.Log($"BaseAttack.phase: {input.Combat.BaseAttack.phase}\n" +
            //           $"Dodge.phase: {input.Combat.Dodge.phase}\n" +
            //           $"Move.phase: {input.Combat.Move.phase}");
            if (input.Combat.BaseAttack.phase is InputActionPhase.Started or InputActionPhase.Performed)
            {
                FrontRoleCtrl.ActionCtrl.InputCommand(EActionCommand.BaseAttack, EActionCommandPhase.Press);
            }
            if (input.Combat.Dodge.phase is InputActionPhase.Started or InputActionPhase.Performed)
            {
                FrontRoleCtrl.ActionCtrl.InputCommand(EActionCommand.Dodge, EActionCommandPhase.Press);
            }
            if (input.Combat.Move.phase is InputActionPhase.Started or InputActionPhase.Performed)
            {
                FrontRoleCtrl.ActionCtrl.InputCommand(EActionCommand.Move, EActionCommandPhase.Press);
            }
            if (input.Combat.SpecialAttack.phase is InputActionPhase.Started or InputActionPhase.Performed)
            {
                FrontRoleCtrl.ActionCtrl.InputCommand(EActionCommand.SpecialAttack, EActionCommandPhase.Press);
            }
            if (input.Combat.Ultimate.phase is InputActionPhase.Started or InputActionPhase.Performed)
            {
                FrontRoleCtrl.ActionCtrl.InputCommand(EActionCommand.Ultimate, EActionCommandPhase.Press);
            }
        }
    }
}