using System.Collections.Generic;
using Manager;
using UnityEngine;
using XLua;

namespace Kirara.AttrEffect
{
    [LuaCallCSharp, CSharpCallLua]
    public class AbilitySet
    {
        public Dictionary<string, ILuaAbility> Abilities { get; private set; } = new();
        private List<(string handle, float time)> Timers { get; set; } = new();

        private static readonly Dictionary<string, LuaTable> configAbilities =
            LuaMgr.Instance.LuaEnv.Global.Get<Dictionary<string, LuaTable>>("configAbilities");

        private delegate ILuaAbility newAbilityDel();

        private static readonly newAbilityDel newAbility =
            LuaMgr.Instance.LuaEnv.Global.GetInPath<newAbilityDel>("Ability.new");

        public void Update(float dt)
        {
            UpdateTimers(dt);
            UpdateAbilities(dt);
        }

        private void UpdateTimers(float dt)
        {
            for (int i = 0; i < Timers.Count;)
            {
                (string handle, float time) = Timers[i];
                time -= dt;
                if (time > 0)
                {
                    Timers[i] = (handle, time);
                    i++;
                }
                else
                {
                    Timers.RemoveAt(i);
                }
            }
        }

        private void UpdateAbilities(float dt)
        {
            foreach (var ability in Abilities.Values)
            {
                ability.update(dt);
            }
        }

        public bool RemoveAbility(string abilityName)
        {
            return Abilities.Remove(abilityName, out var ability);
        }

        public void SetTimer(string handle, float time)
        {
            int i = Timers.FindIndex(x => x.handle == handle);
            if (i >= 0)
            {
                Timers[i] = (handle, time);
            }
            else
            {
                Timers.Add((handle, time));
            }
        }

        public bool HasTimer(string handle)
        {
            return Timers.FindIndex(x => x.handle == handle) >= 0;
        }

        public void AttachAbility(string name, Dictionary<string, double> attrs)
        {
            if (Abilities.ContainsKey(name))
            {
                Debug.LogWarning($"Ability已存在 name: {name}");
                return;
            }
            var ability = newAbility();
            ability.abilitySet = this;
            foreach (var kv in attrs)
            {
                ability.attrs.Set(kv.Key, kv.Value);
            }
            Abilities.Add(name, ability);
            ability.onAttached();
        }

        public void AttachAbility(string name)
        {
            if (Abilities.TryGetValue(name, out var ability))
            {
                ability.onAttached();
            }
            else
            {
                ability = newAbility();
                ability.abilitySet = this;
                ability.setConfig(configAbilities[name]);
                ability.onAttached();
            }
        }

        public static double Inject(string abilityName, string varName)
        {
            if (abilityName == "ShenHaiFangKe_1" && varName == "Rate")
            {
                return 0.114514;
            }
            return 0;
        }
    }
}