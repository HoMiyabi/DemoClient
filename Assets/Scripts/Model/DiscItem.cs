using System;
using System.Collections.Generic;
using System.Linq;
using cfg.main;
using Manager;

namespace Kirara.Model
{
    public class DiscItem : BaseItem
    {
        public static int MaxLevel => ConfigMgr.tb.TbGlobalConfig.DiscMaxLevel;
        public DiscConfig Config { get; private set; }

        public string Id { get; private set; }
        public int Cid { get; private set; }

        private int _level;
        public int Level
        {
            get => _level;
            set
            {
                if (_level == value) return;
                _level = value;
                OnLevelChanged?.Invoke();
            }
        }
        public event Action OnLevelChanged;

        private int _exp;
        public int Exp
        {
            get => _exp;
            set
            {
                if (_exp == value) return;
                _exp = value;
                OnExpChanged?.Invoke();
            }
        }
        public event Action OnExpChanged;

        private string _roleId;
        public string RoleId
        {
            get => _roleId;
            set
            {
                if (_roleId == value) return;
                _roleId = value;
                OnRoleIdChanged?.Invoke();
            }
        }
        public event Action OnRoleIdChanged;

        public bool Locked { get; set; }

        public int Pos { get; set; }

        private NDiscAttr _mainAttr;
        public NDiscAttr MainAttr
        {
            get => _mainAttr;
            set
            {
                if (_mainAttr == value) return;
                _mainAttr = value;
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

        public override string Name => Config.Name;
        public override string IconLoc => Config.IconLoc;

        public static int GetExp(int level)
        {
            var config = ConfigMgr.tb.TbDiscUpgradeExpConfig[level];
            return config.Exp;
        }

        public DiscItem(NDiscItem item)
        {
            Config = ConfigMgr.tb.TbDiscConfig[item.Cid];

            Id = item.Id;
            Cid = item.Cid;
            Level = item.Level;
            Exp = item.Exp;
            RoleId = item.RoleId;
            Locked = item.Locked;
            Pos = item.Pos;
            MainAttr = item.MainAttr;
            SubAttrs = item.SubAttrs.ToList();
        }
    }
}