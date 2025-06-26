using System;
using System.Collections.Generic;
using Kirara;
using Kirara.Quest;

namespace Manager
{
    public static class EventMgr
    {
        #region 通用部分

        private static Dictionary<string, Delegate> eventDict = new();

        public static void AddListener(string eventName, Action<int> action)
        {
            if (eventDict.TryGetValue(eventName, out var del))
            {
                eventDict[eventName] = Delegate.Combine(del, action);
            }
            else
            {
                eventDict.Add(eventName, action);
            }
        }

        public static void RemoveListener(string eventName, Action<int> action)
        {
            if (eventDict.TryGetValue(eventName, out var del))
            {
                del = Delegate.Remove(del, action);
                if (del != null)
                {
                    eventDict[eventName] = del;
                }
                else
                {
                    eventDict.Remove(eventName);
                }
            }
        }

        public static void TriggerEvent(string eventName, int arg)
        {
            if (eventDict.TryGetValue(eventName, out var del))
            {
                var action = (Action<int>)del;
                action(arg);
            }
        }

        #endregion
    }
}