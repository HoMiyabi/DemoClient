using XLua;

namespace Kirara.AttrAbility
{
    [CSharpCallLua]
    public interface ILuaAbility
    {
        AttrAbilitySet set { get; set; }
        string name { get; set; }
        double duration { get; set; }
        int stackCount { get; set; }
        int stackLimit { get; set; }

        void update(float dt);
        void onAttached();
        LuaTable attrs { get; set; }
        void setConfig(LuaTable config);
        double getMinRemainingTime();
    }
}