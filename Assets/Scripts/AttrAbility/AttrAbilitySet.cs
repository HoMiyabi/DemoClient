using System.Collections.Generic;
using cfg.main;
using Manager;
using UnityEngine;
using XLua;

namespace Kirara.AttrAbility
{
    [LuaCallCSharp]
    public class AttrAbilitySet
    {
        #region Attr

        private Dictionary<int, Attr> AttrDict { get; set; } = new();

        public double this[EAttrType type]
        {
            get => this[(int)type];
            set => this[(int)type] = value;
        }

        public double this[int type]
        {
            get
            {
                if (AttrDict.TryGetValue(type, out var attr))
                {
                    return attr.Evaluate();
                }
                return 0;
            }
            set
            {
                if (AttrDict.TryGetValue(type, out var attr))
                {
                    attr.baseValue = value;
                }
                else
                {
                    AttrDict[type] = new Attr((EAttrType)type, value)
                    {
                        set = this
                    };
                }
            }
        }

        public Attr GetAttr(EAttrType type)
        {
            return AttrDict[(int)type];
        }

        public Attr GetAttr(int type)
        {
            return AttrDict[type];
        }

        #endregion

        #region Ability

        public List<ILuaAbility> Abilities { get; private set; } = new();
        private List<(string handle, float time)> Timers { get; set; } = new();

        [CSharpCallLua]
        private delegate void InitAbilityDel(ILuaAbility ability);

        private static readonly Dictionary<string, InitAbilityDel> configAbilities =
            LuaMgr.Instance.LuaEnv.Global.Get<Dictionary<string, InitAbilityDel>>("configAbilities");

        [CSharpCallLua]
        private delegate ILuaAbility NewAbilityDel(AttrAbilitySet set);

        private static readonly NewAbilityDel newAbility =
            LuaMgr.Instance.LuaEnv.Global.GetInPath<NewAbilityDel>("Ability.new");

        public void Update(float dt)
        {
            UpdateTimers(dt);
            UpdateAbilities(dt);
            RemoveZeroStackAbilities();
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
            Debug.Log($"UpdateAbilities, Abilities.Count: {Abilities.Count}");
            foreach (var ability in Abilities)
            {
                ability.update(dt);
            }
        }

        private void RemoveZeroStackAbilities()
        {
            for (int i = 0; i < Abilities.Count;)
            {
                if (Abilities[i].stackCount <= 0)
                {
                    if (Abilities[i].stackCount < 0)
                    {
                        Debug.LogWarning($"Ability stack count < 0 name:{Abilities[i].name}");
                    }
                    Abilities.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }

        public void RemoveAbility(string abilityName)
        {
            Abilities.RemoveAll(x => x.name == abilityName);
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
            if (Abilities.Find(x => x.name == name) != null)
            {
                Debug.LogWarning($"Ability已存在 name: {name}");
                return;
            }
            var ability = newAbility(this);
            ability.name = name;
            foreach (var kv in attrs)
            {
                ability.attrs.Set(kv.Key, kv.Value);
            }
            Abilities.Add(ability);
            ability.onAttached();
        }

        public void AttachAbility(string name)
        {
            var ability = Abilities.Find(x => x.name == name);
            if (ability == null)
            {
                ability = newAbility(this);
                ability.name = name;
                var init = configAbilities[name];
                init(ability);
                Abilities.Add(ability);
                // ability.attrs.ForEach<string, double>((key, value) =>
                // {
                //     if (Enum.TryParse<EAttrType>(key, out var type))
                //     {
                //         GetAttr(type).abilities.Add(ability);
                //     }
                //     else
                //     {
                //         Debug.LogError($"{name} has invalid attr {key}");
                //     }
                // });
            }
            ability.onAttached();
        }

        public static double Inject(string abilityName, string varName)
        {
            if (abilityName == "ShenHaiFangKe_1" && varName == "Rate")
            {
                return 0.114514;
            }
            return 0;
        }

        #endregion
    }
}