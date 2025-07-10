using XLua;

namespace Kirara.AttrEffect
{
    public interface ILuaAbility
    {
        AbilitySet abilitySet { get; set; }
        string name { get; set; }
        int stackCount { get; set; }
        int stackLimit { get; set; }
        void update(float dt);
        void onAttached();
        LuaTable attrs { get; set; }
        void setConfig(LuaTable config);
    }
}