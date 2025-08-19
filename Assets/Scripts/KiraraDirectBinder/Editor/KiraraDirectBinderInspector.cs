using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace KiraraDirectBinder.Editor
{
    [CustomEditor(typeof(KiraraDirectBinder))]
    public class KiraraDirectBinderInspector : UnityEditor.Editor
    {
        private KiraraDirectBinder _target;
        private SerializedProperty itemsProp;
        private ReorderableList reList;
        private Dictionary<string, int> varNameFreq;

        public void OnEnable()
        {
            _target = (KiraraDirectBinder)target;
            itemsProp = serializedObject.FindProperty(nameof(_target.items));

            reList = new ReorderableList(serializedObject, itemsProp)
            {
                drawHeaderCallback = ReList_DrawHeader,
                drawElementCallback = ReList_DrawElement,
                drawElementBackgroundCallback = ReList_DrawElementBackground,
            };
            varNameFreq = new Dictionary<string, int>();
        }

        #region ReorderableList

        private Rect GetVarNameRect(Rect rect)
        {
            float hSpacing = 4f;

            var r1 = rect;
            r1.width = r1.width / 2 - hSpacing / 2;
            return r1;
        }

        private Rect GetComponentRect(Rect rect)
        {
            float hSpacing = 4f;

            var r1 = rect;
            r1.width = r1.width / 2 - hSpacing / 2;
            r1.x += r1.width + hSpacing;
            return r1;
        }

        private void ReList_DrawHeader(Rect rect)
        {
            GUI.Label(rect, "组件");
        }

        private void ReList_DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var itemProp = itemsProp.GetArrayElementAtIndex(index);
            var fieldNameProp = itemProp.FindPropertyRelative("fieldName");
            var componentProp = itemProp.FindPropertyRelative("component");

            rect.y += 2;
            rect.height = EditorGUIUtility.singleLineHeight;

            var varNameRect = GetVarNameRect(rect);
            var componentRect = GetComponentRect(rect);

            float defaultWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = varNameRect.width / 5;
            EditorGUI.PropertyField(varNameRect, fieldNameProp, new GUIContent("变量名"));
            EditorGUI.PropertyField(componentRect, componentProp, new GUIContent("组件"));
            EditorGUIUtility.labelWidth = defaultWidth;
        }

        private void ReList_DrawElementBackground(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (index < 0)
            {
                ReorderableList.defaultBehaviours.DrawElementBackground(rect, index, isActive, isFocused, reList.draggable);
                return;
            }

            var itemProp = itemsProp.GetArrayElementAtIndex(index);
            var fieldNameProp = itemProp.FindPropertyRelative("fieldName");
            var componentProp = itemProp.FindPropertyRelative("component");

            var varNameRect = GetVarNameRect(rect);
            var componentRect = GetComponentRect(rect);

            // 变量名是否重复警告
            if (varNameFreq[fieldNameProp.stringValue] <= 1)
            {
                ReorderableList.defaultBehaviours.DrawElementBackground(varNameRect, index, isActive, isFocused, reList.draggable);
            }
            else
            {
                DrawRect(varNameRect, isActive || isFocused, Color.yellow * 0.85f, Color.yellow);
            }

            if (componentProp.objectReferenceValue)
            {
                ReorderableList.defaultBehaviours.DrawElementBackground(componentRect, index, isActive, isFocused, reList.draggable);
            }
            else
            {
                var color = Color.red + new Color(-0.3f, 0.5f, 0.5f);
                DrawRect(componentRect, isActive || isFocused, color * 0.8f, color);
            }
        }

        #endregion

        private void DrawRect(Rect rect, bool highlight, Color normalColor, Color highlightColor)
        {
            EditorGUI.DrawRect(rect, highlight ? highlightColor : normalColor);
        }

        private void DrawDragArea()
        {
            var area = GUILayoutUtility.GetRect(0f, 30f,
                GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            var tips = new GUIContent("拖拽到此处添加");
            EditorGUI.DrawRect(area, Color.yellow * 0.7f + new Color(0, 0, 0.3f, 0));
            EditorGUI.LabelField(area, tips, new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter
            });

            var e = Event.current;
            switch (e.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!area.Contains(e.mousePosition)) return;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    if (e.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();
                        foreach (var obj in DragAndDrop.objectReferences)
                        {
                            if (obj is Component component)
                            {
                                AddItem(component);
                            }
                            else if (obj is GameObject gameObject)
                            {
                                if (gameObject.TryGetComponent<RectTransform>(out var rectTransform))
                                {
                                    AddItem(rectTransform);
                                }
                            }
                        }
                    }
                    e.Use();
                    break;
            }
        }

        private void AddItem(Component component)
        {
            itemsProp.InsertArrayElementAtIndex(itemsProp.arraySize);
            var itemProp = itemsProp.GetArrayElementAtIndex(itemsProp.arraySize - 1);
            itemProp.FindPropertyRelative("fieldName").stringValue = component.name;
            itemProp.FindPropertyRelative("component").objectReferenceValue = component;
        }

        private void UpdateVarNameFreq()
        {
            varNameFreq.Clear();
            foreach (var item in _target.items)
            {
                varNameFreq[item.fieldName] = varNameFreq.GetValueOrDefault(item.fieldName) + 1;
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawDragArea();

            if (GUILayout.Button("复制C#代码 override"))
            {
                string code = CodeGenerator.GenCSharpCode(_target, "override ");
                GUIUtility.systemCopyBuffer = code;
            }

            if (GUILayout.Button("复制C#代码"))
            {
                string code = CodeGenerator.GenCSharpCode(_target, "");
                GUIUtility.systemCopyBuffer = code;
            }

            if (GUILayout.Button("复制Lua代码"))
            {
                string code = CodeGenerator.GenLuaCode(_target);
                GUIUtility.systemCopyBuffer = code;
            }

            UpdateVarNameFreq();
            reList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }
    }
}