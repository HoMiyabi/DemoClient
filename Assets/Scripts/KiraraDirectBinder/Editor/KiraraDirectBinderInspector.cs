using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Assertions;

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
            };
            varNameFreq = new Dictionary<string, int>();
        }

        #region ReorderableList

        private Rect GetHorizontalItem(Rect rect, int index, int count, float spacing = 0f)
        {
            Assert.IsTrue(count >= 1);
            Assert.IsTrue(index >= 0 && index < count);

            rect.width = (rect.width - (count - 1) * spacing) / count;
            rect.x += index * (rect.width + spacing);
            return rect;
        }

        private void ReList_DrawHeader(Rect rect)
        {
            rect.width -= EditorGUIUtility.singleLineHeight;
            rect.x += EditorGUIUtility.singleLineHeight;
            var varNameRect = GetHorizontalItem(rect, 0, 2, 4);
            var componentRect = GetHorizontalItem(rect, 1, 2, 4);
            GUI.Label(varNameRect, "变量名");
            GUI.Label(componentRect, "组件");
        }

        private void ReList_DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var itemProp = itemsProp.GetArrayElementAtIndex(index);
            var fieldNameProp = itemProp.FindPropertyRelative("fieldName");
            var componentProp = itemProp.FindPropertyRelative("component");

            var contentRect = rect;
            contentRect.y += (contentRect.height - EditorGUIUtility.singleLineHeight) * 0.5f;
            contentRect.height = EditorGUIUtility.singleLineHeight;

            var varNameRect = GetHorizontalItem(contentRect, 0, 2, 4);
            var componentRect = GetHorizontalItem(contentRect, 1, 2, 4);

            DrawVarName(varNameRect, fieldNameProp);
            DrawComponent(componentRect, componentProp);
        }

        private void DrawVarName(Rect r, SerializedProperty prop)
        {
            var oldColor = GUI.color;
            if (!VarName.IsValid(prop.stringValue) || varNameFreq[prop.stringValue] >= 2)
            {
                GUI.color = ColorPalette.invalidVarNameColor;
            }
            EditorGUI.PropertyField(r, prop, GUIContent.none);
            GUI.color = oldColor;
        }

        private void DrawComponent(Rect r, SerializedProperty prop)
        {
            bool isNull = prop.objectReferenceValue == null;
            r.width -= EditorGUIUtility.singleLineHeight;

            var oldColor = GUI.color;
            if (isNull)
            {
                GUI.color = ColorPalette.nullReferenceColor;
            }
            EditorGUI.PropertyField(r, prop, GUIContent.none);
            GUI.color = oldColor;

            EditorGUI.BeginDisabledGroup(isNull);
            r.x += r.width;
            r.width = EditorGUIUtility.singleLineHeight;
            if (EditorGUI.DropdownButton(r, GUIContent.none, FocusType.Keyboard))
            {
                var currCom = (Component)prop.objectReferenceValue;
                var coms = currCom.GetComponents<Component>();
                var menu = new GenericMenu();
                foreach (var com in coms)
                {
                    menu.AddItem(new GUIContent(com.GetType().Name), com == currCom, () =>
                    {
                        prop.objectReferenceValue = com;
                        // GenericMenu的生命周期不一样，所以这里要手动调用
                        serializedObject.ApplyModifiedProperties();
                    });
                }
                menu.DropDown(r);
            }
            EditorGUI.EndDisabledGroup();
        }

        #endregion

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
                                AddItem(gameObject.transform);
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
            for (int i = 0; i < itemsProp.arraySize; i++)
            {
                var itemProp = itemsProp.GetArrayElementAtIndex(i);
                var varNameProp = itemProp.FindPropertyRelative("fieldName");
                string varName = varNameProp.stringValue;
                varNameFreq[varName] = varNameFreq.GetValueOrDefault(varName) + 1;
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawDragArea();

            if (GUILayout.Button("复制C#代码 public override"))
            {
                string code = CSharpGen.Run(_target, "public override");
                GUIUtility.systemCopyBuffer = code;
            }

            if (GUILayout.Button("复制C#代码 public"))
            {
                string code = CSharpGen.Run(_target, "public");
                GUIUtility.systemCopyBuffer = code;
            }

            if (GUILayout.Button("复制Lua代码"))
            {
                string code = LuaGen.Run(_target);
                GUIUtility.systemCopyBuffer = code;
            }

            UpdateVarNameFreq();
            reList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }
    }
}