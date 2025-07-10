using System.Collections.Generic;
using cfg.main;

namespace Kirara.AttrEffect
{
    public class AttrSet
    {
        private readonly Dictionary<int, Attr> dict = new();

        public double this[EAttrType type]
        {
            get => this[(int)type];
            set => this[(int)type] = value;
        }

        public double this[int type]
        {
            get
            {
                if (dict.TryGetValue(type, out var attr))
                {
                    return attr.Evaluate();
                }
                return 0;
            }
            set
            {
                if (dict.TryGetValue(type, out var attr))
                {
                    attr.baseValue = value;
                }
                else
                {
                    dict[type] = new Attr(type, value);
                }
            }
        }

        public Attr GetAttr(EAttrType type)
        {
            return dict[(int)type];
        }
    }
}