using cfg.main;

namespace Kirara.AttrEffect
{
    public class Modifier
    {
        public Effect effect;
        public EAttrType AttrType { get; private set; }
        public float DeltaValue { get; private set; }

        public Modifier(EAttrType attrType, float deltaValue)
        {
            AttrType = attrType;
            DeltaValue = deltaValue;
        }

        public virtual void Apply(Attr attr)
        {
            attr.deltaValue += DeltaValue * effect.StackCount;
        }
    }
}