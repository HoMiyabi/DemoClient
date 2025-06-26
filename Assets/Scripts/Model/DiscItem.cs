using System;
using System.Collections.Generic;
using System.Linq;
using cfg.main;
using Google.Protobuf.Collections;
using Manager;

namespace Kirara.Model
{
    public class DiscItem : BaseItem
    {
        private readonly DiscConfig config;
        private readonly NDiscItem item;

        public DiscItem(NDiscItem item)
        {
            this.item = item;
            config = ConfigMgr.tb.TbDiscConfig[item.Cid];
            _subAttrs = item.SubAttrs.ToList();
        }

        public static int MaxLevel => ConfigMgr.tb.TbGlobalConfig.DiscMaxLevel;

        #region Config

        public int Cid => config.Id;
        public override string Name => config.Name;
        public override string IconLoc => config.IconLoc;
        public List<AbilityConfig> SetAbilities2 => config.SetAbilities2;
        public List<AbilityConfig> SetAbilities4 => config.SetAbilities4;
        public string SetEffect2Desc => config.SetEffect2Desc;
        public string SetEffect4Desc => config.SetEffect4Desc;
        public string Rank => config.Rank;
        public string Desc => config.Desc;

        #endregion

        public int Id => item.Id;
        public int Level
        {
            get => item.Level;
            set
            {
                if (item.Level == value) return;
                item.Level = value;
                OnLevelChanged?.Invoke();
            }
        }
        public event Action OnLevelChanged;

        public int Exp
        {
            get => item.Exp;
            set
            {
                if (item.Exp == value) return;
                item.Exp = value;
                OnExpChanged?.Invoke();
            }
        }
        public event Action OnExpChanged;

        public int WearerId
        {
            get => item.WearerId;
            set
            {
                if (item.WearerId == value) return;
                item.WearerId = value;
                OnWearerIdChanged?.Invoke();
            }
        }
        public event Action OnWearerIdChanged;

        public int Pos => item.Pos;
        public NDiscAttr MainAttr
        {
            get => item.MainAttr;
            set
            {
                if (item.MainAttr == value) return;
                item.MainAttr = value;
                OnMainAttrChanged?.Invoke();
            }
        }
        public event Action OnMainAttrChanged;

        private List<NDiscAttr> _subAttrs;
        public List<NDiscAttr> SubAttrs
        {
            get => _subAttrs;
            set
            {
                _subAttrs = value;
                OnSubAttrsChanged?.Invoke();
            }
        }
        public event Action OnSubAttrsChanged;

        public static int GetExp(int level)
        {
            var config = ConfigMgr.tb.TbDiscUpgradeExpConfig[level];
            return config.Exp;
        }
    }
}