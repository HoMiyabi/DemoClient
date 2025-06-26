#if UNITY_EDITOR
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
        private SerializedProperty items;

        public void OnEnable()
        {
            _target = target as KiraraRuntimeComponents;
            sb = new StringBuilder();
            items = serializedObject.FindProperty(nameof(_target.items));
        }

        public override void OnInspectorGUI()
        {
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
            base.OnInspectorGUI();
        }

        private string GenerateCSharpCode(string modifier)
        {
            sb.Clear();

            sb.AppendLine("#region View");

            foreach ((string fieldName, var com) in _target.items)
            {
                sb.AppendLine($"{modifier} {com.GetType().Name} {fieldName};");
            }

            sb.AppendLine("private void InitUI()");
            sb.AppendLine("{");

            if (_target.items.Count > 0)
            {
                sb.AppendLine($"    var c = GetComponent<{nameof(KiraraRuntimeComponents)}>();");
                sb.AppendLine($"    c.{nameof(_target.Init)}();");
            }

            foreach ((string fieldName, var com) in _target.items)
            {
                string typeName = com.GetType().Name;
                sb.AppendLine($"    {fieldName} = c.{nameof(_target.Q)}<{typeName}>(\"{fieldName}\");");
            }

            sb.AppendLine("}");
            sb.Append("#endregion");

            return sb.ToString();
        }
    }

}
}

#endif