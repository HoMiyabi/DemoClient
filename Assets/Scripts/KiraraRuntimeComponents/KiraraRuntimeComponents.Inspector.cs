#if UNITY_EDITOR
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Kirara.UI
{
public partial class KiraraRuntimeComponents
{

    [CustomEditor(typeof(KiraraRuntimeComponents))]
    public class Inspector : Editor
    {
        private KiraraRuntimeComponents _target;
        private StringBuilder sb;
        private SerializedProperty itemsProp;
        private Component component;

        public void OnEnable()
        {
            _target = target as KiraraRuntimeComponents;
            sb = new StringBuilder();
            itemsProp = serializedObject.FindProperty(nameof(_target.items));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (GUILayout.Button("复制C#代码 private"))
            {
                string code = GenerateCSharpCode("private");
                GUIUtility.systemCopyBuffer = code;
            }
            if (GUILayout.Button("复制C#代码 public"))
            {
                string code = GenerateCSharpCode("public");
                GUIUtility.systemCopyBuffer = code;
            }

            component = (Component)EditorGUILayout.ObjectField("添加组件", component, typeof(Component), true);
            if (component)
            {
                itemsProp.InsertArrayElementAtIndex(itemsProp.arraySize);
                var itemProp = itemsProp.GetArrayElementAtIndex(itemsProp.arraySize - 1);
                itemProp.FindPropertyRelative("fieldName").stringValue = component.name;
                itemProp.FindPropertyRelative("component").objectReferenceValue = component;
                component = null;
            }

            EditorGUILayout.PropertyField(itemsProp);
            serializedObject.ApplyModifiedProperties();
        }

        private string GenerateCSharpCode(string modifier)
        {
            int typeNameMaxLen = _target.items
                .Select(x => x.component.GetType().Name.Length)
                .Max();
            int fieldNameMaxLen = _target.items
                .Select(x => x.fieldName.Length)
                .Max();

            sb.Clear();
            sb.AppendLine("#region View");

            foreach ((string fieldName, var com) in _target.items)
            {
                sb.AppendLine($"{modifier} {com.GetType().Name.PadRight(typeNameMaxLen)} {fieldName};");
            }

            sb.AppendLine("private void InitUI()");
            sb.AppendLine("{");

            if (_target.items.Count > 0)
            {
                int leftLen = Mathf.Max(5, fieldNameMaxLen);
                sb.AppendLine($"    {"var c".PadRight(leftLen)} = GetComponent<{nameof(KiraraRuntimeComponents)}>();");

                for (int i = 0; i < _target.items.Count; i++)
                {
                    (string fieldName, var com) = _target.items[i];
                    string typeName = com.GetType().Name;
                    sb.AppendLine($"    {fieldName.PadRight(leftLen)} = c.{nameof(_target.Q)}<{typeName}>({i}, \"{fieldName}\");");
                }
            }

            sb.AppendLine("}");
            sb.Append("#endregion");

            return sb.ToString();
        }
    }

}
}

#endif