using System;
using System.Collections.Generic;
using XLua;

namespace Kirara.AttrEffect
{
    [CSharpCallLua, LuaCallCSharp]
    public class Ability
    {
        // 名字
        public string name;
        // 持续时间
        public double duration = double.PositiveInfinity;
        // 最大层数
        public int stackLimit = 1;

        public Action<Ability> init;
        public Action<Ability> onAdded;
        public Action onRemoved;
        public Action onAttackLanded;
        public Action onStackChanged;
        public Action<List<double>, double> stackRefreshPolicy;
        public Action<List<double>, double> overflowRefreshPolicy;
        public Dictionary<string, double> attrs;

        // 运行时数据
        // 能力集
        private AbilitySet abilitySet;
        // 当前层数
        public int stackCount = 1;
        // 每层的剩余时间
        public List<double> remainingTimes = new();

        public Ability()
        {
        }

        public Ability(string name)
        {
            this.name = name;
        }

        public void setTimer(string handle, float time)
        {
            abilitySet.SetTimer(handle, time);
        }

        public bool hasTimer(string handle)
        {
            return abilitySet.HasTimer(handle);
        }

        public void attachAbility(string abilityName)
        {
            abilitySet.AttachAbility(abilityName);
        }

        public static double Inject(string abilityName, string varName)
        {
            if (abilityName == "ShenHaiFangKe_1" && varName == "Rate")
            {
                return 0.114514;
            }
            return 0;
        }

        public void Update(float dt)
        {
            for (int i = 0; i < remainingTimes.Count;)
            {
                remainingTimes[i] -= dt;
                if (remainingTimes[i] <= 0)
                {
                    remainingTimes.RemoveAt(i);
                    stackCount--;
                }
                else
                {
                    i++;
                }
            }
        }

        public void OnAttached()
        {
            if (stackCount < stackLimit)
            {
                remainingTimes.Add(duration);
                stackCount++;
                stackRefreshPolicy?.Invoke(remainingTimes, duration);
            }
            else
            {
                overflowRefreshPolicy?.Invoke(remainingTimes, duration);
            }
        }

        private static void RefreshPolicy_DoNothing(List<double> remainingTimes, double duration)
        {
        }

        private static void RefreshPolicy_RefreshMin(List<double> remainingTimes, double duration)
        {
            int idx = 0;
            double min = remainingTimes[0];
            for (int i = 1; i < remainingTimes.Count; i++)
            {
                if (remainingTimes[i] < min)
                {
                    min = remainingTimes[i];
                    idx = i;
                }
            }
            remainingTimes[idx] = duration;
        }

        private static void RefreshPolicy_RefreshAll(List<double> remainingTimes, double duration)
        {
            for (int i = 0; i < remainingTimes.Count; i++)
            {
                remainingTimes[i] = duration;
            }
        }

        public List<Ability> GetConfig()
        {
            return null;
        }
    }
}