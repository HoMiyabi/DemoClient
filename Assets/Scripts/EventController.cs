using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kirara
{
    public class EventController
    {
        private readonly Dictionary<string, Action<string>> namedDict = new();

        private readonly Dictionary<Type, Delegate> typeFnDict = new();

        public void AddListener(string eventName, Action<string> action)
        {
            if (action == null) return;

            if (!namedDict.TryAdd(eventName, action))
            {
                namedDict[eventName] += action;
            }
        }

        public void RemoveListener(string eventName, Action<string> action)
        {
            if (action == null) return;

            if (namedDict.TryGetValue(eventName, out var del))
            {
                namedDict[eventName] = del - action;
            }
        }

        public void TriggerEvent(string eventName, string arg)
        {
            if (namedDict.TryGetValue(eventName, out var action) && action != null)
            {
                action?.Invoke(arg);
            }
            else
            {
                Debug.LogWarning($"具名事件没有注册响应方法 Event Name={eventName}");
            }
        }

        public void AddListener<T>(Action<T> action)
        {
            if (action == null) return;

            var type = typeof(T);

            if (!typeFnDict.TryAdd(type, action))
            {
                typeFnDict[type] = Delegate.Combine(typeFnDict[type], action);
            }
        }

        public void RemoveListener<T>(Action<T> action)
        {
            if (action == null) return;

            var type = typeof(T);

            if (typeFnDict.TryGetValue(type, out var del))
            {
                typeFnDict[type] = Delegate.Remove(del, action);
            }
        }

        public void TriggerEvent(object arg)
        {
            var type = arg.GetType();

            if (typeFnDict.TryGetValue(type, out var del))
            {
                del?.DynamicInvoke(arg);
            }
            else
            {
                Debug.LogWarning($"泛型事件没有注册响应方法 Event Type={type}");
            }
        }
    }
}