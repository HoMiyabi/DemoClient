using System;
using System.Collections.Generic;
using System.Threading;
using cfg.main;
using Cinemachine;
using Cysharp.Threading.Tasks;
using Kirara.Manager;
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

        private PlayerModel player;

        public List<ChCtrl> RoleCtrls;
        public ChCtrl FrontRoleCtrl => RoleCtrls[FrontRoleIdx];

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

        public string FrontRoleId => FrontRoleCtrl.RoleModel.Id;

        public bool switchEnabled = true;

        public readonly Dictionary<EActionCommand, bool> pressedDict = new();

        protected override void Awake()
        {
            base.Awake();

            player = PlayerService.player;

            RoleCtrls = new List<ChCtrl>();
            input = new GameInput();
            cts = new CancellationTokenSource();
        }

        private void OnDestroy()
        {
            cts.Cancel();
        }

        private int updateSelfInterval = 16;

        private async UniTaskVoid SendUpdateSelf()
        {
            var req = new MsgUpdateSelf()
            {
                PosRot = new NPosRot()
                {
                    Pos = new NFloat3(),
                    Rot = new NFloat3()
                }
            };
            var token = cts.Token;

            while (!token.IsCancellationRequested)
            {
                await UniTask.Delay(updateSelfInterval, true, PlayerLoopTiming.Update, token);
                if (token.IsCancellationRequested)
                {
                    return;
                }
                req.PosRot.Pos.Set(FrontRoleCtrl.transform.position);
                req.PosRot.Rot.Set(FrontRoleCtrl.transform.eulerAngles);
                NetMgr.Instance.session.Send(req);
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

            FrontRoleIdx = PlayerService.player.FrontRoleId;

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

            SendUpdateSelf().Forget();
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
            NetFn.Send(new MsgSwitchRole
            {
                FrontRoleId = FrontRoleCtrl.RoleModel.Id
            });
            SwitchRole(idx, true);
        }

        private void HandleSwitchCharacterPrev(InputAction.CallbackContext ctx)
        {
            if (!switchEnabled) return;

            int idx = GetPrev(FrontRoleIdx);
            NetFn.Send(new MsgSwitchRole
            {
                FrontRoleId = FrontRoleCtrl.RoleModel.Id
            });
            SwitchRole(idx, false);
        }

        private void SwitchRole(int idx, bool isNext)
        {
            var prev = FrontRoleCtrl;
            FrontRoleIdx = idx;

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
            var teamRoleIds = PlayerService.player.TeamRoleIds;
            var roles = PlayerService.player.Roles;
            Debug.Log("拥有角色数量：" + roles.Count);
            foreach (string roleId in teamRoleIds)
            {
                var role = roles.Find(it => it.Id == roleId);
                Debug.Log($"加载角色 {role.config.Name}");
                var go = AssetMgr.Instance.InstantiateGO(role.config.PrefabLoc, characterParent);
                var roleCtrl = go.GetComponent<ChCtrl>();
                roleCtrl.Set(role);

                RoleCtrls.Add(roleCtrl);
            }
        }

        private void Update()
        {
            Player.transform.position = FrontRoleCtrl.transform.position;

            UpdateRolesEnergyRegen();
            FrontRoleCtrl.ActionCtrl.UpdatePressed(pressedDict);
        }

        // 更新角色的能量回复
        private void UpdateRolesEnergyRegen()
        {
            const float mul = 8f;
            float maxEnergy = ConfigMgr.tb.TbGlobalConfig.ChMaxEnergy;
            foreach (var ch in RoleCtrls)
            {
                var model = ch.RoleModel;
                var currEnergyAttr = model.ae.GetAttr(EAttrType.CurrEnergy);
                if (currEnergyAttr.Evaluate() >= maxEnergy) continue;

                float regen = model.ae.GetAttr(EAttrType.EnergyRegen).Evaluate() * mul;
                currEnergyAttr.BaseValue = Mathf.Min(currEnergyAttr.BaseValue + Time.deltaTime * regen, maxEnergy);
            }
        }
    }
}