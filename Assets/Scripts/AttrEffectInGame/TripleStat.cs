// using Kirara.AttrEffect;
//
// namespace Kirara
// {
//     public class TripleStat : Stat
//     {
//         public Attr baseAttr;
//         public Attr percentageAttr;
//         public Attr fixedAttr;
//
//         public TripleStat(string name, Attr baseAttr, Attr percentageAttr, Attr fixedAttr) :
//             base(name)
//         {
//             this.baseAttr = baseAttr;
//             this.percentageAttr = percentageAttr;
//             this.fixedAttr = fixedAttr;
//         }
//
//         public override float Evaluate()
//         {
//             return baseAttr.Evaluate() * (1 + percentageAttr.Evaluate()) + fixedAttr.Evaluate();
//         }
//     }
// }