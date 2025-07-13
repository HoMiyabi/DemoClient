using System;
using System.Collections.Generic;
using cfg.main;
using Cysharp.Threading.Tasks;
using Kirara.AttrAbility;
using Manager;

namespace Kirara.Model
{
    public class Role
    {
        public CharacterConfig Config { get; private set; }
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

        public AttrAbilitySet Set { get; private set; } = new();

        private readonly Dictionary<int, int> discCidToCount = new();

        public NSyncRole SyncRole => new()
        {
            Id = Id,
            Movement = new NMovement
            {
                Pos = new NVector3().Set(RoleCtrl.transform.position),
                Rot = new NVector3().Set(RoleCtrl.transform.rotation.eulerAngles)
            }
        };

        public RoleCtrl RoleCtrl { get; set; }

        public Role(NRole role, Player player)
        {
            Config = ConfigMgr.tb.TbCharacterConfig[role.Cid];
            Id = role.Id;

            var chBaseAttrs = ConfigMgr.tb.TbCharacterBaseAttrConfig[role.Cid].ChBaseAttrs;

            foreach (var type in ConfigMgr.tb.TbGlobalConfig.ChAttrTypes)
            {
                Set[type] = 0f;
            }

            foreach (var attr in chBaseAttrs)
            {
                Set[attr.AttrType] = attr.Value;
            }

            // 设置武器
            Weapon = player.Weapons.Find(it => it.Id == role.WeaponId);

            // 设置驱动盘
            discs = new DiscItem[6];
            for (int pos = 1; pos <= 6; pos++)
            {
                SetDisc(pos, player.Discs.Find(it => it.Id == role.DiscIds[pos - 1]));
            }

            Set[EAttrType.CurrHp] = Set[EAttrType.Hp];
        }

        // 更新能量恢复
        public void UpdateEnergyRegen(float dt)
        {
            const float mul = 8f;
            double maxEnergy = ConfigMgr.tb.TbGlobalConfig.ChMaxEnergy;

            if (Set[EAttrType.CurrEnergy] >= maxEnergy) return;

            double regen = Set[EAttrType.EnergyRegen] * mul;
            Set[EAttrType.CurrEnergy] = Math.Min(Set[EAttrType.CurrEnergy] + dt * regen, maxEnergy);
        }

        #region 武器 Weapon

        public async UniTask RemoveWeapon()
        {
            var rsp = await NetFn.ReqRoleRemoveWeapon(new ReqRoleRemoveWeapon
            {
                RoleId = Id
            });
            Weapon = null;
        }

        public async UniTask EquipWeapon(WeaponItem weapon)
        {
            var rsp = await NetFn.ReqRoleEquipWeapon(new ReqRoleEquipWeapon
            {
                RoleId = Id,
                NewWeaponId = weapon.Id,
            });
            Weapon = weapon;
        }

        private void RemoveWeaponAbilities(WeaponItem weapon)
        {
            // 移除属性能力
            string attrName = weapon.Name + "属性";
            Set.RemoveAbility(attrName);

            // 移除被动能力
            Set.RemoveAbility(weapon.Config.PassiveAbilityName);
        }

        private void AddWeaponAbilities(WeaponItem weapon)
        {
            // 添加属性能力
            string name = weapon.Name + "属性";
            Set.AttachAbility(name, new Dictionary<string, double>()
            {
                [GetAttrTypeString(weapon.BaseAttr.AttrTypeId)] = weapon.BaseAttr.Value,
                [GetAttrTypeString(weapon.AdvancedAttr.AttrTypeId)] = weapon.AdvancedAttr.Value,
            });

            // 添加被动能力
            Set.AttachAbility(weapon.Config.PassiveAbilityName);
        }

        #endregion

        private string GetAttrTypeString(int attrTypeId)
        {
            return Enum.ToObject(typeof(EAttrType), attrTypeId).ToString();
        }

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
                RemoveDiscAbility(pos);
            }

            UpdateDiscSetAbility(pos, newDisc);

            discs[pos - 1] = newDisc;
            if (Disc(pos) != null)
            {
                Disc(pos).RoleId = Id;
                AddDiscAbility(pos);
            }

            OnDiscChanged?.Invoke(pos);
        }

        private void RemoveDiscAbility(int pos)
        {
            Set.RemoveAbility($"驱动盘{pos}");
        }

        private void AddDiscAbility(int pos)
        {
            var disc = Disc(pos);
            var attrs = new Dictionary<string, double>
            {
                [GetAttrTypeString(disc.MainAttr.AttrTypeId)] = disc.MainAttr.Value,
            };
            foreach (var discAttr in disc.SubAttrs)
            {
                attrs.Add(GetAttrTypeString(discAttr.AttrTypeId), discAttr.Value);
            }

            Set.AttachAbility($"驱动盘{pos}", attrs);
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
                    Set.RemoveAbility(oldDisc.Config.SetAbility4Name);
                }
                else if (cnt == 2)
                {
                    Set.RemoveAbility(oldDisc.Config.SetAbility2Name);
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
                    Set.AttachAbility(newDisc.Config.SetAbility4Name);
                }
                else if (cnt == 2)
                {
                    Set.AttachAbility(newDisc.Config.SetAbility2Name);
                }
                discCidToCount[newDisc.Cid] = cnt;
            }
        }

        #endregion
    }
}