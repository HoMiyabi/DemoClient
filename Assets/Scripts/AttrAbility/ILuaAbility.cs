using XLua;

namespace Kirara.AttrAbility
{
    [CSharpCallLua]
    public interface ILuaAbility
    {
        AttrAbilitySet set { get; set; }
        string name { get; set; }
        double duration { get; set; }
        int stackLimit { get; set; }
        double attachInterval { get; set; }
        LuaTable attrs { get; set; }
        int stackCount { get; set; }
        LuaTable remainingTimes { get; set; }

        void update(float dt);
        void onAttached();
        double getMinRemainingTime();
    }
}