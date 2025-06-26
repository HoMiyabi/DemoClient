using System;
using cfg.main;

using Manager;

namespace Kirara.Model
{
    public class WeaponItem : BaseItem
    {
        public static int MaxLevel => ConfigMgr.tb.TbGlobalConfig.WeaponMaxLevel;
        public WeaponConfig Config { get; private set; }

        public string Id { get; set; }
        public int Level { get; set; }
        public int Exp { get; set; }

        private string _roleId;

        public string RoleId
        {
            get => _roleId;
            set
            {
                if (_roleId == value) return;
                _roleId = value;
                OnRoleIdChanged?.Invoke(_roleId);
            }
        }
        public event Action<string> OnRoleIdChanged;
        public bool Locked { get; set; }
        public int RefineLevel { get; set; }

        public NWeaponAttr BaseAttr { get; set; }
        public NWeaponAttr AdvancedAttr { get; set; }

        public override string Name => Config.Name;
        public override string IconLoc => Config.IconLoc;

        public WeaponItem(NWeaponItem item)
        {
            Config = ConfigMgr.tb.TbWeaponConfig[item.Cid];

            Id = item.Id;
            Level = item.Level;
            Exp = item.Exp;
            RoleId = item.RoleId;
            Locked = item.Locked;
            RefineLevel = item.RefineLevel;

            BaseAttr = item.BaseAttr;
            AdvancedAttr = item.AdvancedAttr;
        }
    }
}