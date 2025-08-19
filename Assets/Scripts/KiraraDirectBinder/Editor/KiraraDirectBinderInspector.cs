using System.Linq;
using System.Text;
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
        }

        #region ReorderableList

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

            float hSpacing = 4f;

            var rect1 = rect;
            rect1.width = rect1.width / 2 - hSpacing / 2;
            var rect2 = rect1;
            rect2.x += rect1.width + hSpacing;

            float defaultWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = rect1.width / 5;
            EditorGUI.PropertyField(rect1, fieldNameProp, new GUIContent("变量名"));
            EditorGUI.PropertyField(rect2, componentProp, new GUIContent("组件"));
            EditorGUIUtility.labelWidth = defaultWidth;
        }

        private void ReList_DrawElementBackground(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (index < 0)
            {
                ReorderableList.defaultBehaviours.DrawElementBackground(rect, index, isActive, isFocused, reList.draggable);
                return;
            }

            // 如果引用为空，警告
            var itemProp = itemsProp.GetArrayElementAtIndex(index);
            var componentProp = itemProp.FindPropertyRelative("component");
            if (componentProp.objectReferenceValue)
            {
                ReorderableList.defaultBehaviours.DrawElementBackground(rect, index, isActive, isFocused, reList.draggable);
            }
            else
            {
                if (isActive || isFocused)
                {
                    EditorGUI.DrawRect(rect, Color.yellow);
                }
                else
                {
                    EditorGUI.DrawRect(rect, Color.yellow * 0.9f);
                }
            }
        }

        #endregion

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

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

            var component = (Component)EditorGUILayout.ObjectField("添加组件", null, typeof(Component), true);
            if (component)
            {
                itemsProp.InsertArrayElementAtIndex(itemsProp.arraySize);
                var itemProp = itemsProp.GetArrayElementAtIndex(itemsProp.arraySize - 1);
                itemProp.FindPropertyRelative("fieldName").stringValue = component.name;
                itemProp.FindPropertyRelative("component").objectReferenceValue = component;
            }

            reList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }
    }
}