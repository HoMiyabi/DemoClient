using System;
using System.Collections.Generic;
using cfg.main;

using Manager;

namespace Kirara.Model
{
    public class WeaponItem : BaseItem
    {
        private readonly WeaponConfig config;
        private readonly NWeaponItem item;

        public WeaponItem(NWeaponItem item)
        {
            this.item = item;
            config = ConfigMgr.tb.TbWeaponConfig[item.Cid];
        }

        public static int MaxLevel => ConfigMgr.tb.TbGlobalConfig.WeaponMaxLevel;

        #region Config

        public int Cid => config.Id;
        public override string Name => config.Name;
        public override string IconLoc => config.IconLoc;
        public string Rank => config.Rank;
        public string Desc => config.Desc;
        public string PassiveDesc => config.PassiveDesc;
        public List<AbilityConfig> PassiveAbilities => config.PassiveAbilities;

        #endregion


        public int Id => item.Id;

        public int Level => item.Level;
        public int Exp => item.Exp;
        public bool Locked => item.Locked;
        public int RefineLevel => item.RefineLevel;
        public NWeaponAttr BaseAttr => item.BaseAttr;
        public NWeaponAttr AdvancedAttr => item.AdvancedAttr;

        public int WearerId
        {
            get => item.WearerId;
            set
            {
                if (value == item.WearerId) return;
                item.WearerId = value;
                OnWearerIdChanged?.Invoke(item.WearerId);
            }
        }
        public event Action<int> OnWearerIdChanged;
    }
}