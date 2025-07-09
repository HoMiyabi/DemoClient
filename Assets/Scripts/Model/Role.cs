using System;
using System.Collections.Generic;
using cfg.main;
using Cysharp.Threading.Tasks;
using Kirara.AttrEffect;
using Manager;
using UnityEngine;

namespace Kirara.Model
{
    public class Role
    {
        public readonly CharacterConfig config;
        public string Id { get; set; }
        public int Level { get; set; }
        public int Exp { get; set; }

        private WeaponItem _weapon;
        public WeaponItem Weapon
        {
            get => _weapon;
            set
            {
                if (value == _weapon) return;
                if (_weapon != null)
                {
                    _weapon.RoleId = "";
                    RemoveWeaponAbilities(_weapon);
                }
                _weapon = value;
                if (_weapon != null)
                {
                    _weapon.RoleId = Id;
                    AddWeaponAbilities(_weapon);
                }

                OnWeaponChanged?.Invoke();
            }
        }
        public event Action OnWeaponChanged;

        public AttrSet AttrSet { get; private set; } = new();
        public AbilitySet AbilitySet { get; private set; } = new();

        private readonly Dictionary<int, int> discCidToCount = new();

        public NSyncRole SyncRole => new NSyncRole
        {
            Id = Id,
            Movement = new NMovement
            {
                Pos = new NFloat3().Set(RoleCtrl.transform.position),
                Rot = new NFloat3().Set(RoleCtrl.transform.rotation.eulerAngles)
            }
        };

        public RoleCtrl RoleCtrl { get; set; }

        public Role(NRole role, Player player)
        {
            config = ConfigMgr.tb.TbCharacterConfig[role.Cid];
            Id = role.Id;

            var chBaseAttrs = ConfigMgr.tb.TbCharacterBaseAttrConfig[role.Cid].ChBaseAttrs;

            foreach (var attr in chBaseAttrs)
            {
                AttrSet[attr.AttrType] = attr.Value;
            }

            // foreach (var type in ConfigMgr.tb.TbGlobalConfig.ChAttrTypes)
            // {
            //     AttrSet.TryAddAttr(new Attr(type, 0f));
            // }

            // 设置武器
            Weapon = player.Weapons.Find(it => it.Id == role.WeaponId);

            // 设置驱动盘
            discs = new DiscItem[6];
            for (int pos = 1; pos <= 6; pos++)
            {
                SetDisc(pos, player.Discs.Find(it => it.Id == role.DiscIds[pos - 1]));
            }

            AttrSet[EAttrType.CurrHp] = AttrSet[EAttrType.Hp];
        }

        // 更新能量恢复
        public void UpdateEnergyRegen(float dt)
        {
            const float mul = 8f;
            double maxEnergy = ConfigMgr.tb.TbGlobalConfig.ChMaxEnergy;

            if (AttrSet[EAttrType.CurrEnergy] >= maxEnergy) return;

            double regen = AttrSet[EAttrType.EnergyRegen] * mul;
            AttrSet[EAttrType.CurrEnergy] = Math.Min(AttrSet[EAttrType.CurrEnergy] + dt * regen, maxEnergy);
        }

        #region 武器 Weapon

        public async UniTask RemoveWeapon()
        {
            var rsp = await NetFn.ReqRoleRemoveWeapon(new ReqRoleRemoveWeapon
            {
                RoleId = Id
            });
            // if (rsp.Code != 0)
            // {
            //     Debug.LogWarning($"失败: {rsp.Msg}");
            //     return;
            // }
            Weapon = null;
        }

        public async UniTask EquipWeapon(WeaponItem weapon)
        {
            var rsp = await NetFn.ReqRoleEquipWeapon(new ReqRoleEquipWeapon
            {
                RoleId = Id,
                NewWeaponId = weapon.Id,
            });
            // if (rsp.Code != 0)
            // {
            //     Debug.LogWarning($"失败: {rsp.Msg}");
            //     return;
            // }
            Weapon = weapon;
        }

        private void RemoveWeaponAbilities(WeaponItem weapon)
        {
            // 移除主动能力
            string effName = weapon.Name + "主动";
            AbilitySet.RemoveAbility(effName);

            // 移除被动能力
            foreach (var abilityConfig in weapon.Config.PassiveAbilities)
            {
                AbilitySet.RemoveAbility(abilityConfig.Name);
            }
        }

        private void AddWeaponAbilities(WeaponItem weapon)
        {
            // 添加主动能力
            var modifiers = new List<Modifier>(2)
            {
                // Base 基本属性
                weapon.BaseAttr.GetModifier(),
                // Advanced 进阶属性
                weapon.AdvancedAttr.GetModifier()
            };
            string name = weapon.Name + "主动";
            AbilitySet.AttachAbility(new Ability(name));

            // 添加被动能力
            foreach (var abilityConfig in weapon.Config.PassiveAbilities)
            {
                AbilitySet.AttachAbility(abilityConfig.GetRuntime());
            }
        }

        #endregion

        #region 驱动盘 Disc

        private DiscItem[] discs;
        public event Action<int> OnDiscChanged;

        public DiscItem Disc(int pos)
        {
            return discs[pos - 1];
        }

        public async UniTaskVoid RemoveDisc(int pos)
        {
            var rsp = await NetFn.ReqRoleRemoveDisc(new ReqRoleRemoveDisc
            {
                RoleId = Id,
                DiscPos = pos,
            });
            SetDisc(pos, null);
        }

        public async UniTaskVoid EquipDisc(int pos, DiscItem newDisc)
        {
            var rsp = await NetFn.ReqRoleEquipDisc(new ReqRoleEquipDisc
            {
                RoleId = Id,
                DiscPos = pos,
                NewDiscId = newDisc.Id
            });
            SetDisc(pos, newDisc);
        }

        private void SetDisc(int pos, DiscItem newDisc)
        {
            if (Disc(pos) == newDisc) return;

            if (Disc(pos) != null)
            {
                Disc(pos).RoleId = "";
                RemoveDiscAbilities(pos);
            }

            UpdateDiscSetAbility(pos, newDisc);

            discs[pos - 1] = newDisc;
            if (Disc(pos) != null)
            {
                Disc(pos).RoleId = Id;
                AddDiscAbilities(pos);
            }

            OnDiscChanged?.Invoke(pos);
        }

        private void RemoveDiscAbilities(int pos)
        {
            AbilitySet.RemoveAbility($"驱动盘{pos}");
        }

        private void AddDiscAbilities(int pos)
        {
            var disc = Disc(pos);

            var modifiers = new List<Modifier>(1 + disc.SubAttrs.Count)
            {
                disc.MainAttr.GetModifier(),
            };
            foreach (var nIdModifier in disc.SubAttrs)
            {
                modifiers.Add(nIdModifier.GetModifier());
            }

            AbilitySet.AddAbility(new Ability($"驱动盘{pos}"));
        }

        private bool IsDiscSameConfig(DiscItem disc1, DiscItem disc2)
        {
            return (disc1 != null && disc2 != null && disc1.Cid == disc2.Cid)
                || (disc1 == null && disc2 == null);
        }

        private void UpdateDiscSetAbility(int pos, DiscItem newDisc)
        {
            var oldDisc = Disc(pos);
            if (IsDiscSameConfig(oldDisc, newDisc)) return;

            if (oldDisc != null)
            {
                int cnt = discCidToCount[oldDisc.Cid];
                if (cnt == 4)
                {
                    foreach (var effCfg in oldDisc.Config.SetAbilities4)
                    {
                        AttrSet.RemoveAbility(effCfg.Name);
                    }
                }
                else if (cnt == 2)
                {
                    foreach (var effCfg in oldDisc.Config.SetAbilities2)
                    {
                        AttrSet.RemoveAbility(effCfg.Name);
                    }
                }
                cnt--;
                if (cnt == 0)
                {
                    discCidToCount.Remove(oldDisc.Cid);
                }
                else
                {
                    discCidToCount[oldDisc.Cid] = cnt;
                }
            }

            if (newDisc != null)
            {
                int cnt = discCidToCount.GetValueOrDefault(newDisc.Cid) + 1;
                if (cnt == 4)
                {
                    foreach (var effCfg in newDisc.Config.SetAbilities4)
                    {
                        AbilitySet.AddAbility(effCfg.GetRuntime());
                    }
                }
                else if (cnt == 2)
                {
                    foreach (var effCfg in newDisc.Config.SetAbilities2)
                    {
                        AbilitySet.AddAbility(effCfg.GetRuntime());
                    }
                }
                discCidToCount[newDisc.Cid] = cnt;
            }
        }

        #endregion
    }
}