using System;
using System.Collections.Generic;
using System.Threading;
using Cinemachine;
using Cysharp.Threading.Tasks;
using Kirara.Model;
using Kirara.TimelineAction;
using Kirara.UI.Panel;
using Manager;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Kirara
{
    public class PlayerSystem : UnitySingleton<PlayerSystem>
    {
        [SerializeField] private Transform characterParent;
        [SerializeField] public CinemachineVirtualCamera vcam;
        [SerializeField] private GameObject playerPrefab;
        public GameObject Player { get; private set; }

        public GameInput input { get; private set; }

        private CancellationTokenSource cts;

        private Player player;

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

                var ch = RoleCtrls[_frontRoleIdx];
                ch.VCam = vcam;
                vcam.Follow = ch.vcamFollow;
                vcam.LookAt = ch.vcamLookAt;

                OnFrontRoleChanged?.Invoke();
            }
        }
        public event Action OnFrontRoleChanged;

        public string FrontRoleId
        {
            get => FrontRoleCtrl.Role.Id;
            set
            {
                FrontRoleIdx = player.Roles.FindIndex(x => x.Id == value);
            }
        }

        public bool switchEnabled = true;

        public readonly Dictionary<EActionCommand, bool> pressedDict = new();

        protected override void Awake()
        {
            base.Awake();

            player = PlayerService.Player;

            RoleCtrls = new List<RoleCtrl>();
            input = new GameInput();
            cts = new CancellationTokenSource();
        }

        private void OnDestroy()
        {
            cts.Cancel();
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
                    req.Player.Roles.Add(roleCtrl.Role.SyncRole);
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

        private void OnEnable()
        {
            vcam.GetComponent<CinemachineInputProvider>().enabled = true;
            input.Combat.Enable();
        }

        private void OnDisable()
        {
            vcam.GetComponent<CinemachineInputProvider>().enabled = false;
            input.Combat.Disable();
        }

        private void Start()
        {
            var handle = AssetMgr.Instance.package.LoadAssetSync<AudioClip>("music1");
            AudioMgr.Instance.PlayMusic(handle.AssetObject as AudioClip);

            Init();

            // 输入
            input.Combat.SwitchCharactersNext.started += HandleSwitchCharacterNext;
            input.Combat.SwitchCharactersPrevious.started += HandleSwitchCharacterPrev;
            foreach (var action in input.Combat.Get().actions)
            {
                action.started += HandleStartedInputToFrontCommand;
                action.canceled += HandleCanceledInputToFrontCommand;
            }

            Player = Instantiate(playerPrefab, transform);
        }

        public static bool TryConvertCommand(Guid id, out EActionCommand command)
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
                command = EActionCommand.Always;
                return false;
            }
            return true;
        }

        private void Init()
        {
            CreateTeamRoles();
            NetFn.Send(new MsgEnterRoom());
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
            if (TryConvertCommand(ctx.action.id, out var command))
            {
                pressedDict[command] = true;
            }
            FrontRoleCtrl.ActionCtrl.Input(command, EActionCommandPhase.Down);
        }

        private void HandleCanceledInputToFrontCommand(InputAction.CallbackContext ctx)
        {
            // started中调用Disable会导致ActionId改变
            if (ctx.phase == InputActionPhase.Disabled) return;

            if (TryConvertCommand(ctx.action.id, out var command))
            {
                pressedDict[command] = false;
            }
            FrontRoleCtrl.ActionCtrl.Input(command, EActionCommandPhase.Up);
        }

        private void HandleSwitchCharacterNext(InputAction.CallbackContext ctx)
        {
            if (!switchEnabled) return;

            int idx = GetNext(FrontRoleIdx);
            SwitchRole(idx, true);
        }

        private void HandleSwitchCharacterPrev(InputAction.CallbackContext ctx)
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

            var monster = MonsterSystem.Instance.ClosestAttackingMonster(prev.transform.position, out float dist);
            if (monster != null)
            {
                prev.SwitchOutAided();
                FrontRoleCtrl.SwitchInParryAid(monster);
            }
            else
            {
                prev.SwitchOutNormal();
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
                Debug.Log($"加载角色 {role.config.Name}");
                var go = AssetMgr.Instance.InstantiateGO(role.config.PrefabLoc, characterParent);
                var roleCtrl = go.GetComponent<RoleCtrl>();
                roleCtrl.Set(role);

                RoleCtrls.Add(roleCtrl);
            }
        }

        private void Update()
        {
            Player.transform.position = FrontRoleCtrl.transform.position;
            FrontRoleCtrl.ActionCtrl.UpdatePressed(pressedDict);
        }
    }
}