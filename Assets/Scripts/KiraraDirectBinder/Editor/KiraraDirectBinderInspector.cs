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

            reList = new ReorderableList(_target.items, typeof(KiraraDirectBinder.Item))
            {
                drawHeaderCallback = ReList_DrawHeader,
                drawElementCallback = ReList_DrawElement,
                onAddCallback = ReList_OnAdd,
                onRemoveCallback = ReList_OnRemove,
                onReorderCallbackWithDetails = ReList_OnReorder,
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

            // 如果引用为空，警告
            if (!componentProp.objectReferenceValue)
            {
                EditorGUI.DrawRect(rect, Color.yellow);
            }

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

        private void ReList_OnAdd(ReorderableList list)
        {
            itemsProp.InsertArrayElementAtIndex(list.index);
        }

        private void ReList_OnRemove(ReorderableList list)
        {
            itemsProp.DeleteArrayElementAtIndex(list.index);
        }

        private void ReList_OnReorder(ReorderableList list, int oldIndex, int newIndex)
        {
            itemsProp.MoveArrayElement(oldIndex, newIndex);
        }

        #endregion

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (GUILayout.Button("复制C#代码 override"))
            {
                string code = GenerateCSharpCode("override ");
                GUIUtility.systemCopyBuffer = code;
            }

            if (GUILayout.Button("复制C#代码"))
            {
                string code = GenerateCSharpCode("");
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

        private static string GetFullNameOrEmpty(Component component)
        {
            return component.GetType().FullName ?? string.Empty;
        }

        // 生成C#代码
        private string GenerateCSharpCode(string bindUIMethodModifier)
        {
            string varModifier = "private";

            // 类型全名的最大长度
            int typeNameMaxLen = _target.items
                .Select(x => GetFullNameOrEmpty(x.component).Length)
                .DefaultIfEmpty()
                .Max();

            // 变量名最大长度
            int fieldNameMaxLen = _target.items
                .Select(x => x.fieldName.Length)
                .DefaultIfEmpty()
                .Max();
            int equalLeftLen = Mathf.Max("var c".Length, fieldNameMaxLen);

            var sb = new StringBuilder();
            sb.AppendLine("#region View");

            if (_target.items.Count > 0)
            {
                sb.AppendLine($"private bool _isBound;");
            }
            foreach ((string fieldName, var com) in _target.items)
            {
                sb.AppendLine($"{varModifier} {GetFullNameOrEmpty(com).PadRight(typeNameMaxLen)} {fieldName};");
            }

            sb.AppendLine($"public {bindUIMethodModifier}void BindUI()");
            sb.AppendLine("{");

            if (_target.items.Count > 0)
            {
                sb.AppendLine("if (_isBound) return;");
                sb.AppendLine("_isBound = true;");
                sb.AppendLine($"    {"var c".PadRight(equalLeftLen)} = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();");

                for (int i = 0; i < _target.items.Count; i++)
                {
                    (string fieldName, var com) = _target.items[i];
                    string typeName = GetFullNameOrEmpty(com);
                    sb.AppendLine($"    {fieldName.PadRight(equalLeftLen)} = c.{nameof(_target.Q)}<{typeName}>({i}, \"{fieldName}\");");
                }
            }

            sb.AppendLine("}");
            sb.Append("#endregion");

            return sb.ToString();
        }
    }
}