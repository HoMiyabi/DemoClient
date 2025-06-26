using System;
using cfg.main;

using Manager;

namespace Kirara.Model
{
    public class MaterialItem : BaseItem
    {
        private readonly MaterialItemConfig config;
        private readonly NMaterialItem item;
        public MaterialItem(NMaterialItem item)
        {
            this.item = item;
            config = ConfigMgr.tb.TbMaterialItemConfig[item.Cid];
        }
        public int Cid => config.Id;
        public override string Name => config.Name;
        public override string IconLoc => config.IconLoc;

        public int Exp => config.Exp;


        public int Count
        {
            get => item.Count;
            set => item.Count = value;
        }
    }
}