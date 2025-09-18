using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace KiraraDirectBinder.Editor
{
    public static class KiraraDirectBinderHierarchy
    {
        [InitializeOnLoadMethod]
        private static void Load()
        {
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemOnGUI;
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

        private static readonly HashSet<string> varNameSet = new();

        private static void OnHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            var go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (!go)
            {
                return;
            }

            // Debug.Log(selectionRect);
            var r = new Rect(selectionRect);
            r.position += rectOffset;

            for (int i = 0; i < KiraraDirectBinderList.binders.Count; i++)
            {
                var binder = KiraraDirectBinderList.binders[i];
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
            binderRect.x += binderRect.width - GUI.skin.label.CalcSize(new GUIContent(text)).x;
            for (int i = 0; i < KiraraDirectBinderList.binders.Count; i++)
            {
                var binder = KiraraDirectBinderList.binders[i];
                if (binder.gameObject == go)
                {
                    DrawBinderHierarchyWindowItem(i, binderRect);
                }
            }
        }

        private static void DrawBinderHierarchyWindowItem(int index, Rect rect)
        {
            var binder = KiraraDirectBinderList.binders[index];
            GUI.Label(rect, text, new GUIStyle()
            {
                normal = new GUIStyleState()
                {
                    textColor = GetColor(index)
                }
            });

            string tip;
            bool nullReference = binder.items.FindIndex(item => item.component == null) >= 0;
            if (nullReference)
            {
                tip = "丢失引用";
                var v = GUI.skin.label.CalcSize(new GUIContent(tip));
                rect.x -= v.x;
                rect.width = v.x;

                EditorGUI.DrawRect(rect, ColorPalette.nullReferenceColor);
                GUI.Label(rect, tip, new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleLeft,
                });
            }

            bool invalidVarName = false;
            varNameSet.Clear();
            foreach (var item in binder.items)
            {
                if (!VarName.IsValid(item.fieldName) || !varNameSet.Add(item.fieldName))
                {
                    invalidVarName = true;
                    break;
                }
            }

            if (invalidVarName)
            {
                tip = "变量名有误";
                var v = GUI.skin.label.CalcSize(new GUIContent(tip));
                rect.x -= v.x;
                rect.width = v.x;

                EditorGUI.DrawRect(rect, ColorPalette.invalidVarNameColor);
                GUI.Label(rect, tip, new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleLeft
                });
            }
        }

        private static Color GetColor(int index)
        {
            return colors[index % colors.Length];
        }
    }
}