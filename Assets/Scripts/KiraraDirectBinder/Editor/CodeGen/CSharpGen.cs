using System.Text;
using UnityEngine;

namespace KiraraDirectBinder.Editor
{
    internal static class CSharpGen
    {
        public static string Run(KiraraDirectBinder binder, string bindUIMethodModifier)
        {
            string varModifier = "private";

            // 类型全名的最大长度
            int typeFullNameMaxLen = CodeGenHelper.GetTypeFullNameMaxLen(binder.items);

            // 变量名最大长度
            int varNameMaxLen = CodeGenHelper.GetVarNameMaxLen(binder.items);
            int equalLeftLen = Mathf.Max("var b".Length, varNameMaxLen);

            var sb = new StringBuilder();
            sb.AppendLine("#region View");

            if (binder.items.Count > 0)
            {
                sb.AppendLine($"private bool _isBound;");
            }
            foreach ((string fieldName, var com) in binder.items)
            {
                sb.AppendLine($"{varModifier} {CodeGenHelper.GetFullNameOrEmpty(com).PadRight(typeFullNameMaxLen)} {fieldName};");
            }

            sb.AppendLine($"{bindUIMethodModifier} void BindUI()");
            sb.AppendLine("{");

            if (binder.items.Count > 0)
            {
                sb.AppendLine("if (_isBound) return;");
                sb.AppendLine("_isBound = true;");
                sb.AppendLine($"    {"var b".PadRight(equalLeftLen)} = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();");

                for (int i = 0; i < binder.items.Count; i++)
                {
                    (string fieldName, var com) = binder.items[i];
                    string typeName = CodeGenHelper.GetFullNameOrEmpty(com);
                    sb.AppendLine($"    {fieldName.PadRight(equalLeftLen)} = b.Q<{typeName}>({i}, \"{fieldName}\");");
                }
            }

            sb.AppendLine("}");
            sb.Append("#endregion");

            return sb.ToString();
        }
    }
}