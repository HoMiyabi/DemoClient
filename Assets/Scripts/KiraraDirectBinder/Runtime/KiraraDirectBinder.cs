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
            var com = component as T;
            if (!com)
            {
                Debug.LogWarning($"组件类型不匹配, name: {name}, index: {index}, fieldName: {fieldName}, 组件实际类型: {component.GetType()}");
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

        private static readonly string text = "★";

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
            if (!go)
            {
                return;
            }

            // Debug.Log(selectionRect);
            var r = new Rect(selectionRect);
            r.position += rectOffset;

            for (int i = 0; i < binders.Count; i++)
            {
                var binder = binders[i];
                foreach (var item in binder.items)
                {
                    if (item.component)
                    {
                        if (item.component.gameObject == go)
                        {
                            GUI.Label(r, text, new GUIStyle()
                            {
                                normal = new GUIStyleState()
                                {
                                    textColor = GetColor(i)
                                }
                            });
                        }
                    }
                }
            }

            // 绘制Binder
            var binderRect = new Rect(selectionRect);
            binderRect.position += new Vector2(binderRect.width - GUI.skin.label.CalcSize(new GUIContent(text)).x, 0);
            for (int i = 0; i < binders.Count; i++)
            {
                var binder = binders[i];
                if (binder.gameObject == go)
                {
                    GUI.Label(binderRect, text, new GUIStyle()
                    {
                        normal = new GUIStyleState()
                        {
                            textColor = GetColor(i)
                        }
                    });
                    if (binder.items.FindIndex(item => item.component == null) >= 0)
                    {
                        var v = GUI.skin.label.CalcSize(new GUIContent("丢失引用"));
                        binderRect.position -= new Vector2(v.x, 0);
                        binderRect.position += new Vector2(0, (binderRect.height - v.y) * 0.5f);
                        GUI.Label(binderRect, "丢失引用");
                    }
                }
            }
        }

        private static Color GetColor(int index)
        {
            return colors[index % colors.Length];
        }

        private static readonly List<KiraraDirectBinder> binders = new();

        private void OnEnable()
        {
            binders.Add(this);
        }

        private void OnDisable()
        {
            binders.Remove(this);
        }

#endif
    }
}