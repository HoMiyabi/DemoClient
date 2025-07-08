using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kirara.UI
{
    [ExecuteAlways]
    public partial class KiraraRuntimeComponents : MonoBehaviour
    {
        [Serializable]
        public class Item
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
            if (index < 0 || index >= items.Count)
            {
                Debug.LogWarning($"索引越界, index: {index}, fieldName: {fieldName}, items.Count: {items.Count}");
                return null;
            }
            var item = items[index];
            if (item.fieldName != fieldName)
            {
                Debug.LogWarning($"字段名不匹配, index: {index}, fieldName: {fieldName}, item.fieldName: {item.fieldName}");
                return null;
            }
            var component = item.component;
            if (!component)
            {
                Debug.LogWarning($"组件为null, index: {index}, fieldName: {fieldName}");
                return null;
            }
            var com = component as T;
            if (!com)
            {
                Debug.LogWarning($"组件类型不匹配, index: {index}, fieldName: {fieldName}, 组件实际类型: {component.GetType()}");
                return null;
            }
            return com;
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        private static void Load()
        {
            UnityEditor.EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemOnGUI;
        }

        private static readonly string[] texts = {
            "★",
        };

        private static readonly Color[] colors = {
            Color.yellow,
            Color.white,
            Color.red,
            Color.magenta,
            Color.black,
            Color.cyan
        };

        private static readonly Vector2 rectOffset = new(-28, 0);

        private static void OnHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            var go = UnityEditor.EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (go == null)
            {
                return;
            }

            // Debug.Log(selectionRect);
            var r = new Rect(selectionRect);
            r.position += rectOffset;

            for (int i = 0; i < runtimeComponents.Count; i++)
            {
                if (runtimeComponents[i].items
                    .Select(x =>
                    {
                        if (x.component != null)
                        {
                            return x.component.gameObject;
                        }
                        Debug.LogWarning($"丢失引用 {runtimeComponents[i].name} 字段名: {x.fieldName}");
                        return null;
                    })
                    .Contains(go))
                {
                    GUI.Label(r, texts[(i / colors.Length) % texts.Length], new GUIStyle()
                    {
                        normal = new GUIStyleState()
                        {
                            textColor = colors[i % colors.Length]
                        }
                    });
                }
            }
        }

        private static readonly List<KiraraRuntimeComponents> runtimeComponents = new();

        private void OnEnable()
        {
            runtimeComponents.Add(this);
        }

        private void OnDisable()
        {
            runtimeComponents.Remove(this);
        }

#endif
    }
}