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

        public Component addComponent;

        public List<Item> items = new();

        public Dictionary<string, Component> dict;

        public void Init()
        {
            dict = items.ToDictionary(x => x.fieldName, x => x.component);
        }

        public T Q<T>(string fieldName) where T : Component
        {
            if (!dict.TryGetValue(fieldName, out var component))
            {
                Debug.LogError($"{name}找不到组件{fieldName}，是否未更新代码？");
                return null;
            }
            if (component == null)
            {
                Debug.LogWarning($"{component.name}组件{fieldName}为空");
                return null;
            }
            var com = component as T;
            if (com == null)
            {
                Debug.LogError($"{component.name}组件{fieldName}类型不为{typeof(T).Name}，请检查代码");
                return null;
            }
            return com;
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            if (addComponent != null)
            {
                items.Add(new Item(addComponent.name, addComponent));
                addComponent = null;
            }
        }

        [UnityEditor.InitializeOnLoadMethod]
        private static void Load()
        {
            UnityEditor.EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemOnGUI;
        }

        private static readonly string[] texts = new[]
        {
            "★",
        };

        private static readonly Color[] colors = new[]
        {
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