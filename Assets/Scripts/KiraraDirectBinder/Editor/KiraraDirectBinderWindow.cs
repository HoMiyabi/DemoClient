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
                UpdateData();
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
                UpdateData();
            }
        }

        private struct EditItem
        {
            public Component component;
            public int binderIdx;
            public string varName;
        }

        private readonly List<EditItem> _editItems = new();
        private readonly List<Component> _components = new();

        [MenuItem("GameObject/Kirara Direct Binder Window _c", false, priority = 0)]
        public static void OpenWindow()
        {
            var tra = Selection.activeTransform;

            if (tra == null) return;

            var window = GetWindow<KiraraDirectBinderWindow>(true, "Kirara Direct Binder Window");
            window._target = tra;
            window._binder = FindBinder(tra);
            window.UpdateData();
            if (window._binder)
            {
                EditorGUIUtility.PingObject(window._binder);
            }
        }

        private void UpdateData()
        {
            UpdateEditItems();
            UpdateWindowPosition();
        }

        private void UpdateEditItems()
        {
            _editItems.Clear();

            if (!Target || !Binder)
            {
                return;
            }

            string targetName = Target.name;
            string targetVarName = VarName.ReplaceToValid(targetName);

            Target.GetComponents(_components);
            foreach (var component in _components)
            {
                int binderIdx = Binder.items.FindIndex(x => x.component == component);

                string varName = binderIdx != -1 ?
                    Binder.items[binderIdx].fieldName :
                    targetVarName;

                _editItems.Add(new EditItem
                {
                    component = component,
                    binderIdx = binderIdx,
                    varName = varName
                });
            }

            _editItems.Sort((x, y) =>
                MatchScore(y.component, targetName) - MatchScore(x.component, targetName));
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
            if (target.EndsWith(name) || name.EndsWith(target))
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
            EditorGUIUtility.labelWidth = 50f;

            Binder = (KiraraDirectBinder)EditorGUILayout.ObjectField(
                "Binder", Binder, typeof(KiraraDirectBinder), true);

            Target = (Transform)EditorGUILayout.ObjectField(
                "目标对象", Target, typeof(Transform), true);

            EditorGUIUtility.labelWidth = width;
        }

        private void DrawRemoveButton(in EditItem item)
        {
            if (GUILayout.Button("-", GUILayout.ExpandWidth(false)))
            {
                Undo.RecordObject(Binder, "KiraraDirectBinder删除Item");
                Binder.items.RemoveAt(item.binderIdx);
                EditorUtility.SetDirty(Binder);
                // 如果这里不Close可能要更新窗口数据
                Close();
            }
        }

        private void DrawAddButton(in EditItem item)
        {
            if (GUILayout.Button("+", GUILayout.ExpandWidth(false)))
            {
                Undo.RecordObject(Binder, "KiraraDirectBinder添加Item");
                Binder.items.Add(new KiraraDirectBinder.Item(item.varName, item.component));
                EditorUtility.SetDirty(Binder);
                // 如果这里不Close可能要更新窗口数据
                Close();
            }
        }

        private void DrawRenameSaveButton(in EditItem item)
        {
            if (GUILayout.Button("✓", GUILayout.ExpandWidth(false)))
            {
                Undo.RecordObject(_binder, "KiraraDirectBinder重命名Item");
                Binder.items[item.binderIdx] = new KiraraDirectBinder.Item(item.varName, item.component);
                EditorUtility.SetDirty(Binder);
            }
        }

        private static readonly float saveButtonWidth = EditorStyles.miniButton.CalcSize(new GUIContent("✓")).x;

        private void OnGUI()
        {
            DrawBinderAndTarget();

            float col2 = 0f;

            foreach (var item in _editItems)
            {
                float width = EditorStyles.textField.CalcSize(new GUIContent(item.varName)).x;
                if (item.binderIdx != -1)
                {
                    width += saveButtonWidth;
                }
                col2 = Mathf.Max(col2, width);
            }

            float maxTypeWidth = _editItems
                .Select(x =>
                    GUI.skin.label.CalcSize(new GUIContent(x.component.GetType().Name)).x)
                .DefaultIfEmpty()
                .Max();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("X", GUILayout.ExpandWidth(false)))
            {
                Close();
            }
            GUILayout.Label("变量名", GUILayout.MinWidth(col2));
            GUILayout.Label("组件类型", GUILayout.Width(maxTypeWidth));
            GUILayout.EndHorizontal();

            for (int i = 0; i < _editItems.Count; i++)
            {
                var item = _editItems[i];
                GUILayout.BeginHorizontal();
                if (item.binderIdx != -1)
                {
                    // 组件在Binder里，可以删除
                    DrawRemoveButton(item);
                }
                else
                {
                    // 组件不在Binder里，可以添加
                    DrawAddButton(item);
                }

                float w = col2;
                if (item.binderIdx != -1)
                {
                    w -= saveButtonWidth;
                }

                EditorGUI.BeginChangeCheck();
                item.varName = GUILayout.TextField(item.varName, GUILayout.MinWidth(w));
                if (EditorGUI.EndChangeCheck())
                {
                    // Undo.RecordObject(Binder, "KiraraDirectBinder修改Item");
                    // Binder.items[i] = new KiraraDirectBinder.Item(item.varName, item.component);
                    // EditorUtility.SetDirty(Binder);
                    _editItems[i] = item;
                }

                if (item.binderIdx != -1)
                {
                    // 组件在Binder里，可以重命名保存到Binder
                    DrawRenameSaveButton(item);
                }

                GUILayout.Label(item.component.GetType().Name, GUILayout.Width(maxTypeWidth));

                GUILayout.EndHorizontal();
            }
        }
    }
}