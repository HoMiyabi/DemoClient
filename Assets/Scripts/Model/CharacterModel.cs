using System;
using System.Collections.Generic;
using cfg.main;
using Cysharp.Threading.Tasks;

using Kirara.AttrEffect;
using Manager;
using UnityEngine;

namespace Kirara.Model
{
    public class CharacterModel
    {
        public int Id { get; private set; }
        public readonly CharacterConfig config;


        public readonly AttrEffect.AttrEffect ae = new();

        private readonly Dictionary<int, int> discCidToCount = new();

        public CharacterModel(NCharacterInfo nChInfo)
        {
            config = ConfigMgr.tb.TbCharacterConfig[nChInfo.ConfigId];
            Id = nChInfo.Id;

            var chBaseAttrs = ConfigMgr.tb.TbCharacterBaseAttrConfig[nChInfo.ConfigId].ChBaseAttrs;

            foreach (var attr in chBaseAttrs)
            {
                ae.AddAttr(new Attr(attr.AttrType, attr.Value));
            }

            foreach (var type in ConfigMgr.tb.TbGlobalConfig.ChAttrTypes)
            {
                ae.TryAddAttr(new Attr(type, 0f));
            }

            // 设置武器
            Weapon = PlayerService.player.weapons.Find(it => it.Id == nChInfo.WeaponId);

            // 设置驱动盘
            discs = new DiscItem[6];
            for (int pos = 1; pos <= 6; pos++)
            {
                SetDisc(pos, PlayerService.player.discs.Find(it => it.Id == nChInfo.DiscIds[pos - 1]));
            }

            ae.GetAttr(EAttrType.CurrHp).BaseValue = ae.EvaluateAttr(EAttrType.Hp);
        }

        #region 武器 Weapon

        private WeaponItem _weapon;
        public WeaponItem Weapon
        {
            get => _weapon;
            set
            {
                if (value == _weapon) return;
                if (_weapon != null)
                {
                    _weapon.WearerId = 0;
                    RemoveWeaponAbilities(_weapon);
                }
                _weapon = value;
                if (_weapon != null)
                {
                    _weapon.WearerId = Id;
                    AddWeaponAbilities(_weapon);
                }

                OnWeaponChanged?.Invoke();
            }
        }
        public event Action OnWeaponChanged;

        public async UniTask RemoveWeapon()
        {
            var rsp = await NetFn.ReqRoleRemoveWeapon(new ReqRoleRemoveWeapon
            {
                RoleId = Id
            });
            if (rsp.Code != 0)
            {
                Debug.LogWarning($"失败: {rsp.Msg}");
                return;
            }
            Weapon = null;
        }

        public async UniTask EquipWeapon(WeaponItem weapon)
        {
            var rsp = await NetFn.ReqRoleEquipWeapon(new ReqRoleEquipWeapon
            {
                RoleId = Id,
                NewWeaponId = weapon.Id,
            });
            if (rsp.Code != 0)
            {
                Debug.LogWarning($"失败: {rsp.Msg}");
                return;
            }
            Weapon = weapon;
        }

        private void RemoveWeaponAbilities(WeaponItem weapon)
        {
            // 移除主动能力
            string effName = weapon.Name + "主动";
            ae.RemoveAbility(effName);

            // 移除被动能力
            foreach (var abilityConfig in weapon.PassiveAbilities)
            {
                ae.RemoveAbility(abilityConfig.Name);
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
            string abilityName = weapon.Name + "主动";
            ae.AddAbility(new Ability(
                abilityName,
                new Effect(abilityName, EffectDurationPolicy.Infinite, 0, modifiers)
                ));

            // 添加被动能力
            foreach (var abilityConfig in weapon.PassiveAbilities)
            {
                ae.AddAbility(abilityConfig.GetRuntime());
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
            var rsp = await NetFn.ReqCharacterRemoveDisc(new ReqCharacterRemoveDisc
            {
                CharacterId = Id,
                DiscPos = pos,
            });
            if (rsp.Code != 0)
            {
                Debug.LogWarning($"失败: {rsp.Msg}");
                return;
            }
            SetDisc(pos, null);
        }

        public async UniTaskVoid EquipDisc(int pos, DiscItem newDisc)
        {
            var rsp = await NetFn.ReqCharacterEquipDisc(new ReqCharacterEquipDisc
            {
                CharacterId = Id,
                DiscPos = pos,
                NewDiscId = newDisc.Id
            });
            if (rsp.Code != 0)
            {
                Debug.LogWarning($"失败: {rsp.Msg}");
                return;
            }
            SetDisc(pos, newDisc);
        }

        private void SetDisc(int pos, DiscItem newDisc)
        {
            if (Disc(pos) == newDisc) return;

            if (Disc(pos) != null)
            {
                Disc(pos).WearerId = 0;
                RemoveDiscAbilities(pos);
            }

            UpdateDiscSetAbility(pos, newDisc);

            discs[pos - 1] = newDisc;
            if (Disc(pos) != null)
            {
                Disc(pos).WearerId = Id;
                AddDiscAbilities(pos);
            }

            OnDiscChanged?.Invoke(pos);
        }

        private void RemoveDiscAbilities(int pos)
        {
            ae.RemoveAbility($"驱动盘{pos}");
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

            ae.AddAbility(new Ability(
                $"驱动盘{pos}",
                new Effect($"驱动盘{pos}", EffectDurationPolicy.Infinite, 0, modifiers)
                ));
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
                    foreach (var effCfg in oldDisc.SetAbilities4)
                    {
                        ae.RemoveAbility(effCfg.Name);
                    }
                }
                else if (cnt == 2)
                {
                    foreach (var effCfg in oldDisc.SetAbilities2)
                    {
                        ae.RemoveAbility(effCfg.Name);
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
                    foreach (var effCfg in newDisc.SetAbilities4)
                    {
                        ae.AddAbility(effCfg.GetRuntime());
                    }
                }
                else if (cnt == 2)
                {
                    foreach (var effCfg in newDisc.SetAbilities2)
                    {
                        ae.AddAbility(effCfg.GetRuntime());
                    }
                }
                discCidToCount[newDisc.Cid] = cnt;
            }
        }

        #endregion
    }
}