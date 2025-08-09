using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace KiraraDirectBinder.Editor
{
    public class KiraraDirectBinderWindow : EditorWindow
    {
        private KiraraDirectBinder _binder;
        private KiraraDirectBinder Binder
        {
            get => _binder;
            set
            {
                if (value == _binder) return;

                _binder = value;
                Set();
            }
        }

        private Transform _target;
        private Transform Target
        {
            get => _target;
            set
            {
                if (value == _target) return;

                _target = value;
                Set();
            }
        }

        private Component[] components;
        private string[] names;
        private List<int> idxInBinder = new();

        [MenuItem("GameObject/Kirara Direct Binder Window _c", false, priority = 0)]
        public static void OpenWindow()
        {
            var tra = Selection.activeTransform;

            if (tra == null) return;

            var window = GetWindow<KiraraDirectBinderWindow>(true, "Kirara Direct Binder Window");
            window._target = tra;
            window._binder = FindBinder(tra);
            window.Set();
        }

        private void Set()
        {
            UpdateComponents();
            UpdateComponentNames();

            UpdateWindowPosition();
        }

        private void UpdateComponentNames()
        {
            if (!Target || !Binder)
            {
                names = Array.Empty<string>();
                return;
            }

            names = new string[components.Length];
            for (int i = 0; i < components.Length; i++)
            {
                int comIdxInBinder = Binder.items
                    .FindIndex(x => x.component == components[i]);
                if (comIdxInBinder != -1)
                {
                    names[i] = Binder.items[comIdxInBinder].fieldName;
                }
                else
                {
                    names[i] = components[i].name.Replace(" ", "").Replace("(", "").Replace(")", "");
                }
            }
        }

        private void UpdateWindowPosition()
        {
            var windowRect = position;

            // Pos
            var mouseInScreen = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
            var offset = new Vector2(-15, -75);
            windowRect.position = mouseInScreen + offset;

            position = windowRect;
        }

        private static int MatchScore(Component com, string target)
        {
            var type = com.GetType();
            string name = type.Name;
            if (target.EndsWith(name))
            {
                return 3;
            }

            string pattern = $@"{Regex.Escape(name)} \(\d+\)$";
            if (Regex.IsMatch(target, pattern))
            {
                return 2;
            }

            if (KiraraDirectBinderAlias.alias.TryGetValue(type, out var aliasNames)
                && aliasNames.Any(target.EndsWith))
            {
                return 1;
            }
            return 0;
        }

        /// <summary>
        /// 把匹配的组件放在前面
        /// </summary>
        /// <returns></returns>
        private void UpdateComponents()
        {
            if (!Target || !Binder)
            {
                components = Array.Empty<Component>();
                return;
            }

            string _name = Target.name;
            components = Target.GetComponents<Component>();
            Array.Sort(components, (x, y) =>
                MatchScore(y, _name) - MatchScore(x, _name));
        }

        /// <summary>
        /// 优先在父路径找最近的，其次自己上查找 <see cref="KiraraDirectBinder"/>
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        private static KiraraDirectBinder FindBinder(Transform transform)
        {
            if (transform == null) return null;
            var p = transform.parent;
            KiraraDirectBinder binder;

            while (p)
            {
                if (p.TryGetComponent(out binder))
                {
                    return binder;
                }
                p = p.parent;
            }
            if (transform.TryGetComponent(out binder))
            {
                return binder;
            }
            return null;
        }

        private void DrawBinderAndTarget()
        {
            float width = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 60f;

            Binder = (KiraraDirectBinder)EditorGUILayout.ObjectField(
                "Binder:", Binder, typeof(KiraraDirectBinder), true);

            Target = (Transform)EditorGUILayout.ObjectField(
                "选中对象:", Target, typeof(Transform), true);

            EditorGUIUtility.labelWidth = width;
        }

        private void DrawAddButton(int i)
        {
            if (GUILayout.Button("+", GUILayout.ExpandWidth(false)))
            {
                Undo.RecordObject(_binder, $"添加{nameof(Binder)}" +
                                           $".{nameof(Binder.items)}" +
                                           $"字段{names[i]} " +
                                           $"类型{components[i].GetType().Name}");
                Binder.items.Add(new KiraraDirectBinder.Item(names[i], components[i]));
                EditorUtility.SetDirty(Binder);
                Close();
            }
        }

        private void DrawRemoveButton(int i, int idxInBinder)
        {
            if (GUILayout.Button("-", GUILayout.ExpandWidth(false)))
            {
                Undo.RecordObject(_binder, $"删除{nameof(_binder)}" +
                                           $".{nameof(_binder.items)}" +
                                           $"字段{_binder.items[idxInBinder].fieldName} " +
                                           $"类型{components[i].GetType().Name}");
                _binder.items.RemoveAt(idxInBinder);
                EditorUtility.SetDirty(Binder);
                Close();
            }
        }

        private void DrawRenameButton(int i, int idxInBinder)
        {
            if (GUILayout.Button("✓", GUILayout.ExpandWidth(false)))
            {
                Undo.RecordObject(_binder, $"修改{nameof(_binder)}" +
                                           $".{nameof(_binder.items)}" +
                                           $"字段新名字{names[i]} " +
                                           $"类型{components[i].GetType().Name}");
                Binder.items[idxInBinder] = new KiraraDirectBinder.Item(names[i], components[i]);
                EditorUtility.SetDirty(Binder);
            }
        }

        private void UpdateIdxInBinder()
        {
            idxInBinder.Clear();
            if (!Binder) return;
            foreach (var c in components)
            {
                int idx = Binder.items.FindIndex(x => x.component == c);
                idxInBinder.Add(idx);
            }
        }

        private static readonly float saveButtonWidth = EditorStyles.miniButton.CalcSize(new GUIContent("✓")).x;

        private void OnGUI()
        {
            DrawBinderAndTarget();

            UpdateIdxInBinder();

            float col2 = 0f;
            for (int i = 0; i < components.Length; i++)
            {
                float width = EditorStyles.textField.CalcSize(new GUIContent(names[i])).x;
                if (idxInBinder[i] != -1)
                {
                    width += saveButtonWidth;
                }
                col2 = Mathf.Max(col2, width);
            }

            float maxTypeWidth = components.Length > 0 ? components
                .Select(x =>
                    GUI.skin.label.CalcSize(new GUIContent(x.GetType().Name)).x)
                .Max() : 0;

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("X", GUILayout.ExpandWidth(false)))
            {
                Close();
            }
            GUILayout.Label("变量名", GUILayout.MinWidth(col2));
            GUILayout.Label("组件类型", GUILayout.Width(maxTypeWidth));
            GUILayout.EndHorizontal();

            for (int i = 0; i < components.Length; i++)
            {
                var com = components[i];

                int idxInBinder = Binder.items
                    .FindIndex(x => x.component == components[i]);

                GUILayout.BeginHorizontal();

                if (idxInBinder != -1)
                {
                    // 组件在Binder里，可以删除
                    DrawRemoveButton(i, idxInBinder);
                }
                else
                {
                    // 组件不在Binder里，可以添加
                    DrawAddButton(i);
                }

                float w = col2;
                if (idxInBinder != -1)
                {
                    w -= saveButtonWidth;
                }
                names[i] = GUILayout.TextField(names[i], GUILayout.MinWidth(w));

                if (idxInBinder != -1)
                {
                    // 组件在Binder里，可以重命名
                    DrawRenameButton(i, idxInBinder);
                }

                GUILayout.Label(com.GetType().Name, GUILayout.Width(maxTypeWidth));

                GUILayout.EndHorizontal();
            }
        }
    }
}