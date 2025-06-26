using System;
using System.Collections.Generic;
using UnityEngine;

namespace Manager
{
    public static class ObserveMgr
    {
        private static readonly Dictionary<string, Delegate> dict = new();
        private static readonly Dictionary<object, List<(string, Delegate)>> tagCallbacks = new();

        private static bool CheckNull(string name, Delegate action)
        {
            if (name == null)
            {
                Debug.LogWarning("Name should not be null.");
                return false;
            }

            if (action == null)
            {
                Debug.LogWarning("Action should not be null.");
                return false;
            }
            return true;
        }

        private static void AddTagCallback(object tag, string name, Delegate action)
        {
            if (tagCallbacks.TryGetValue(tag, out var list))
            {
                list.Add((name, action));
            }
            else
            {
                tagCallbacks.Add(tag, new List<(string, Delegate)> { (name, action) });
            }
        }

        public static void Add<T>(object tag, string name, Action<T> action) where T : class
        {
            if (tag == null)
            {
                Debug.LogWarning("Tag should not be null.");
                return;
            }
            if (!CheckNull(name, action)) return;

            AddInternal(name, action);
            AddTagCallback(tag, name, action);
        }

        private static void AddInternal(string name, Delegate action)
        {
            if (dict.TryGetValue(name, out var del))
            {
                dict[name] = Delegate.Combine(del, action);
            }
            else
            {
                dict.Add(name, action);
            }
        }

        private static void RemoveInternal(string name, Delegate action)
        {
            if (dict.TryGetValue(name, out var del))
            {
                var d = Delegate.Remove(del, action);
                if (d != null)
                {
                    dict[name] = d;
                }
                else
                {
                    dict.Remove(name);
                }
            }
        }

        public static void Remove(object tag)
        {
            if (tag == null)
            {
                Debug.LogWarning("Tag should not be null.");
                return;
            }

            if (tagCallbacks.Remove(tag, out var list))
            {
                foreach ((string name, var del) in list)
                {
                    RemoveInternal(name, del);
                }
            }
        }

        public static void NotifySet<T>(string name, T obj) where T : class
        {
            if (dict.TryGetValue(name, out var del))
            {
                var action = del as Action<T>;
                action?.Invoke(obj);
            }
        }
    }
}