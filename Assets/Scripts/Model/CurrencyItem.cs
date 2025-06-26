using cfg.main;

using Manager;

namespace Kirara.Model
{
    public class CurrencyItem : BaseItem
    {
        private readonly CurrencyItemConfig config;
        private readonly NCurrencyItem item;

        public CurrencyItem(NCurrencyItem item)
        {
            this.item = item;
            config = ConfigMgr.tb.TbCurrencyItemConfig[item.Cid];
        }

        public int Cid => config.Id;
        public override string Name => config.Name;
        public override string IconLoc => config.IconLoc;

        public int Count
        {
            get => item.Count;
            set => item.Count = value;
        }
    }
}