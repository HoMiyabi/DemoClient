using System;
using System.Collections.Generic;
using cfg.main;
using Cysharp.Threading.Tasks;
using Kirara.AttrBuff;
using Manager;
using UnityEngine;

namespace Kirara.Model
{
    public class Role
    {
        public RoleConfig Config { get; private set; }
        public string Id { get; set; }
        public int Level { get; set; }
        public int Exp { get; set; }
        public Vector3 Pos { get; set; }
        public Vector3 Rot { get; set; }

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
                    RemoveWeaponBuffs(_weapon);
                }
                _weapon = value;
                if (_weapon != null)
                {
                    _weapon.RoleId = Id;
                    AddWeaponBuffs(_weapon);
                }

                OnWeaponChanged?.Invoke();
            }
        }
        public event Action OnWeaponChanged;

        public AttrBuffSet Set { get; private set; } = new();

        private readonly Dictionary<int, int> discCidToCount = new();

        public Role(NRole role, Player player)
        {
            Config = ConfigMgr.tb.TbRoleConfig[role.Cid];
            Id = role.Id;
            Pos = role.Pos.Unity();
            Rot = role.Rot.Unity();

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

            // 添加能量恢复Buff
            Set.AttachBuff("RoleEnergyRegen");
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

        private void RemoveWeaponBuffs(WeaponItem weapon)
        {
            // 移除属性Buff
            string attrName = weapon.Name + "属性";
            Set.RemoveBuff(attrName);

            // 移除被动Buff
            Set.RemoveBuff(weapon.Config.PassiveAbilityName);
        }

        private void AddWeaponBuffs(WeaponItem weapon)
        {
            // 添加属性Buff
            string name = weapon.Name + "属性";
            Set.AttachBuff(name, new Dictionary<EAttrType, double>()
            {
                [(EAttrType)weapon.BaseAttr.AttrTypeId] = weapon.BaseAttr.Value,
                [(EAttrType)weapon.AdvancedAttr.AttrTypeId] = weapon.AdvancedAttr.Value,
            });

            // 添加被动Buff
            Set.AttachBuff(weapon.Config.PassiveAbilityName);
        }

        #endregion

        // private string GetAttrTypeString(int attrTypeId)
        // {
        //     return Enum.ToObject(typeof(EAttrType), attrTypeId).ToString();
        // }

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
                RemoveDiscBuff(pos);
            }

            UpdateDiscSetBuff(pos, newDisc);

            discs[pos - 1] = newDisc;
            if (Disc(pos) != null)
            {
                Disc(pos).RoleId = Id;
                AddDiscBuff(pos);
            }

            OnDiscChanged?.Invoke(pos);
        }

        private void RemoveDiscBuff(int pos)
        {
            Set.RemoveBuff($"驱动盘{pos}");
        }

        private void AddDiscBuff(int pos)
        {
            var disc = Disc(pos);
            var attrs = new Dictionary<EAttrType, double>
            {
                [(EAttrType)disc.MainAttr.AttrTypeId] = disc.MainAttr.Value,
            };
            foreach (var discAttr in disc.SubAttrs)
            {
                attrs.Add((EAttrType)discAttr.AttrTypeId, discAttr.Value);
            }

            Set.AttachBuff($"驱动盘{pos}", attrs);
        }

        private bool IsDiscSameConfig(DiscItem disc1, DiscItem disc2)
        {
            return (disc1 != null && disc2 != null && disc1.Cid == disc2.Cid)
                || (disc1 == null && disc2 == null);
        }

        private void UpdateDiscSetBuff(int pos, DiscItem newDisc)
        {
            var oldDisc = Disc(pos);
            if (IsDiscSameConfig(oldDisc, newDisc)) return;

            if (oldDisc != null)
            {
                int cnt = discCidToCount[oldDisc.Cid];
                if (cnt == 4)
                {
                    Set.RemoveBuff(oldDisc.Config.SetAbility4Name);
                }
                else if (cnt == 2)
                {
                    Set.RemoveBuff(oldDisc.Config.SetAbility2Name);
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
                    Set.AttachBuff(newDisc.Config.SetAbility4Name);
                }
                else if (cnt == 2)
                {
                    Set.AttachBuff(newDisc.Config.SetAbility2Name);
                }
                discCidToCount[newDisc.Cid] = cnt;
            }
        }

        #endregion
    }
}