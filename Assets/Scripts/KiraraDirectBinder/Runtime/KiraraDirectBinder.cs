using System;
using System.Collections.Generic;
using UnityEngine;

namespace KiraraDirectBinder
{
    [ExecuteAlways]
    public class KiraraDirectBinder : MonoBehaviour
    {
        [Serializable]
        public struct Item
        {
            public string fieldName;
            public Component component;

            public Item(string fieldName, Component component)
            {
                this.fieldName = fieldName;
                this.component = component;
            }

            public void Deconstruct(out string fieldName, out Component component)
            {
                fieldName = this.fieldName;
                component = this.component;
            }
        }

        public List<Item> items = new();

        public T Q<T>(int index, string fieldName) where T : Component
        {
            var component = Q(index, fieldName);
            var com = component as T;
            if (!com)
            {
                Debug.LogWarning($"组件类型不匹配, name: {name}, index: {index}, fieldName: {fieldName}, 组件实际类型: {component.GetType()}");
                return null;
            }
            return com;
        }

        public Component Q(int index, string fieldName)
        {
            if (index < 0 || index >= items.Count)
            {
                Debug.LogWarning($"索引越界, name: {name}, index: {index}, fieldName: {fieldName}, items.Count: {items.Count}");
                return null;
            }
            var item = items[index];
            if (item.fieldName != fieldName)
            {
                Debug.LogWarning($"字段名不匹配, name: {name}, index: {index}, fieldName: {fieldName}, item.fieldName: {item.fieldName}");
                return null;
            }
            var component = item.component;
            if (!component)
            {
                Debug.LogWarning($"组件为null, name: {name}, index: {index}, fieldName: {fieldName}");
                return null;
            }
            return component;
        }

#if UNITY_EDITOR
        private void OnEnable()
        {
            KiraraDirectBinderList.binders.Add(this);
        }

        private void OnDisable()
        {
            KiraraDirectBinderList.binders.Remove(this);
        }
#endif
    }
}