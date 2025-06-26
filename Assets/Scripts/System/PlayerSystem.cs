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

        public List<ChCtrl> ChCtrls;
        public ChCtrl FrontCh => ChCtrls[FrontChIdx];

        private int frontChIdx = -1;
        public int FrontChIdx
        {
            get => frontChIdx;
            private set
            {
                if (frontChIdx == value) return;
                frontChIdx = value;

                var ch = ChCtrls[frontChIdx];
                ch.VCam = vcam;
                vcam.Follow = ch.vcamFollow;
                vcam.LookAt = ch.vcamLookAt;

                OnFrontChChanged?.Invoke();
            }
        }
        public event Action OnFrontChChanged;

        public bool switchEnabled = true;

        public readonly Dictionary<EActionCommand, bool> pressedDict = new();

        protected override void Awake()
        {
            base.Awake();

            player = PlayerService.player;

            ChCtrls = new List<ChCtrl>();
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
                req.PosRot.Pos.Set(FrontCh.transform.position);
                req.PosRot.Rot.Set(FrontCh.transform.eulerAngles);
                NetMgr.Instance.session.Send(req);
            }
        }

        public int GetNext(int idx)
        {
            return (idx + 1) % ChCtrls.Count;
        }

        public int GetPrev(int idx)
        {
            return (idx - 1 + ChCtrls.Count) % ChCtrls.Count;
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
            CreateCharacters();
            NetFn.Send(new MsgEnterRoom());
            UIMgr.Instance.PushPanel<CombatPanel>();

            FrontChIdx = PlayerService.player.playerInfo.FrontChIdx;

            for (int i = 0; i < ChCtrls.Count; i++)
            {
                if (i == FrontChIdx)
                {
                    ChCtrls[i].InitFront();
                }
                else
                {
                    ChCtrls[i].InitBackground();
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
            FrontCh.ActionCtrl.Input(command, EActionCommandPhase.Down);
        }

        private void HandleCanceledInputToFrontCommand(InputAction.CallbackContext ctx)
        {
            // started中调用Disable会导致ActionId改变
            if (ctx.phase == InputActionPhase.Disabled) return;

            if (TryConvertCommand(ctx.action.id, out var command))
            {
                pressedDict[command] = false;
            }
            FrontCh.ActionCtrl.Input(command, EActionCommandPhase.Up);
        }

        private void HandleSwitchCharacterNext(InputAction.CallbackContext ctx)
        {
            if (!switchEnabled) return;

            int idx = GetNext(FrontChIdx);
            NetFn.Send(new MsgSwitchRole
            {
                Idx = idx,
                Next = true
            });
            SwitchCh(idx, true);
        }

        private void HandleSwitchCharacterPrev(InputAction.CallbackContext ctx)
        {
            if (!switchEnabled) return;

            int idx = GetPrev(FrontChIdx);
            NetFn.Send(new MsgSwitchRole
            {
                Idx = idx,
                Next = false
            });
            SwitchCh(idx, false);
        }

        private void SwitchCh(int idx, bool isNext)
        {
            var prev = FrontCh;
            FrontChIdx = idx;

            var monster = MonsterSystem.Instance.ClosestAttackingMonster(prev.transform.position, out float dist);
            if (monster != null)
            {
                prev.SwitchOutAided();
                FrontCh.SwitchInParryAid(monster);
            }
            else
            {
                prev.SwitchOutNormal();
                FrontCh.SwitchInNormal(prev, isNext);
            }
        }

        private void CreateCharacters()
        {
            var cids = PlayerService.player.playerInfo.GroupChCids;
            var chModels = PlayerService.player.chModels;
            Debug.Log("队伍角色数量：" + chModels.Count);
            foreach (int chCid in cids)
            {
                var chModel = chModels.Find(it => it.config.Id == chCid);
                Debug.Log($"加载角色 {chModel.config.Name}");
                var handle = AssetMgr.Instance.package.LoadAssetSync<GameObject>(chModel.config.PrefabLoc);
                var go = handle.InstantiateSync(characterParent);
                handle.Release();

                ChCtrls.Add(go.GetComponent<ChCtrl>().Set(chModel));
            }
        }

        private void Update()
        {
            Player.transform.position = FrontCh.transform.position;

            UpdateCharactersEnergyRegen();
            FrontCh.ActionCtrl.UpdatePressed(pressedDict);
        }

        private void UpdateCharactersEnergyRegen()
        {
            const float mul = 8f;
            float maxEnergy = ConfigMgr.tb.TbGlobalConfig.ChMaxEnergy;
            foreach (var ch in ChCtrls)
            {
                var model = ch.ChModel;
                var currEnergyAttr = model.ae.GetAttr(EAttrType.CurrEnergy);
                if (currEnergyAttr.Evaluate() >= maxEnergy) continue;

                float regen = model.ae.GetAttr(EAttrType.EnergyRegen).Evaluate() * mul;
                currEnergyAttr.BaseValue = Mathf.Min(currEnergyAttr.BaseValue + Time.deltaTime * regen, maxEnergy);
            }
        }
    }
}