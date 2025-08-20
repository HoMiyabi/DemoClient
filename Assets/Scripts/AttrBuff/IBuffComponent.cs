using XLua;

namespace Kirara.AttrBuff
{
    [CSharpCallLua]
    public interface IBuffComponent
    {
        AttrBuffSet set { get; set; }
        string name { get; set; }
        double duration { get; set; }
        int stackLimit { get; set; }
        double attachInterval { get; set; }
        LuaTable attrs { get; set; }
        int stackCount { get; set; }
        LuaTable remainingTimes { get; set; }

        void Update(float dt);
        void Attached();
        double GetMinRemainingTime();

        void OnAttackHit(AttackHitContext ctx);
        void OnActionStart(ActionStartContext ctx);
    }
}