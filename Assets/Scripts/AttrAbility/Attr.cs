using cfg.main;

namespace Kirara.AttrAbility
{
    public class Attr
    {
        public EAttrType type;

        public double baseValue;

        public AttrAbilitySet set;

        public Attr(EAttrType type, double baseValue)
        {
            this.type = type;
            this.baseValue = baseValue;
        }

        public double Evaluate()
        {
            double delta = 0;
            foreach (var ability in set.Abilities)
            {
                if (ability.attrs.ContainsKey(type))
                {
                    delta += ability.stackCount * ability.attrs.Get<EAttrType, double>(type);
                }
            }

            if ((int)type % 100 == 0)
            {
                // 为一级属性
                int i = (int)type;
                double bas = set[i + 1];
                double pct = set[i + 2];
                double fix = set[i + 3];
                return baseValue + delta + bas * (1f + pct) + fix;
            }
            // 为二级属性
            return baseValue + delta;
        }
    }
}