using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KiraraDirectBinder.Editor
{
    internal static class CodeGenerator
    {
        private static int GetTypeFullNameMaxLen(KiraraDirectBinder binder)
        {
            return binder.items
                .Select(x => GetFullNameOrEmpty(x.component).Length)
                .DefaultIfEmpty()
                .Max();
        }

        private static int GetVarNameMaxLen(KiraraDirectBinder binder)
        {
            return binder.items
                .Select(x => x.fieldName.Length)
                .DefaultIfEmpty()
                .Max();
        }

        private static string GetFullNameOrEmpty(Component component)
        {
            return component.GetType().FullName ?? string.Empty;
        }

        private static void AssertNoDuplicateNames(KiraraDirectBinder binder)
        {
            var set = new HashSet<string>();
            foreach (var item in binder.items)
            {
                if (!set.Add(item.fieldName))
                {
                    Debug.LogWarning($"重复的变量名: {item.fieldName}");
                    return;
                }
            }
        }

        public static string GenLuaCode(KiraraDirectBinder binder)
        {
            AssertNoDuplicateNames(binder);
            // 变量名最大长度
            int varNameMaxLen = GetVarNameMaxLen(binder);
            int equalLeftLen = Mathf.Max("local b".Length, 5 + varNameMaxLen);
            var sb = new StringBuilder();

            if (binder.items.Count > 0)
            {
                sb.AppendLine("if self._isBound then");
                sb.AppendLine("    return");
                sb.AppendLine("end");
                sb.AppendLine("self._isBound = true");
                sb.AppendLine($"{"local b".PadRight(equalLeftLen)} = self.com:GetComponent(typeof(CS.KiraraDirectBinder.KiraraDirectBinder))");

                for (int i = 0; i < binder.items.Count; i++)
                {
                    (string fieldName, var com) = binder.items[i];
                    // string typeName = GetFullNameOrEmpty(com);
                    sb.Append($"{$"self.{fieldName}".PadRight(equalLeftLen)} = b:Q({i}, \"{fieldName}\")");
                    if (i < binder.items.Count - 1)
                    {
                        sb.AppendLine();
                    }
                }
            }

            return sb.ToString();
        }

        // 生成C#代码
        public static string GenCSharpCode(KiraraDirectBinder binder, string bindUIMethodModifier)
        {
            AssertNoDuplicateNames(binder);
            string varModifier = "private";

            // 类型全名的最大长度
            int typeFullNameMaxLen = GetTypeFullNameMaxLen(binder);

            // 变量名最大长度
            int varNameMaxLen = GetVarNameMaxLen(binder);
            int equalLeftLen = Mathf.Max("var b".Length, varNameMaxLen);

            var sb = new StringBuilder();
            sb.AppendLine("#region View");

            if (binder.items.Count > 0)
            {
                sb.AppendLine($"private bool _isBound;");
            }
            foreach ((string fieldName, var com) in binder.items)
            {
                sb.AppendLine($"{varModifier} {GetFullNameOrEmpty(com).PadRight(typeFullNameMaxLen)} {fieldName};");
            }

            sb.AppendLine($"public {bindUIMethodModifier}void BindUI()");
            sb.AppendLine("{");

            if (binder.items.Count > 0)
            {
                sb.AppendLine("if (_isBound) return;");
                sb.AppendLine("_isBound = true;");
                sb.AppendLine($"    {"var b".PadRight(equalLeftLen)} = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();");

                for (int i = 0; i < binder.items.Count; i++)
                {
                    (string fieldName, var com) = binder.items[i];
                    string typeName = GetFullNameOrEmpty(com);
                    sb.AppendLine($"    {fieldName.PadRight(equalLeftLen)} = b.Q<{typeName}>({i}, \"{fieldName}\");");
                }
            }

            sb.AppendLine("}");
            sb.Append("#endregion");

            return sb.ToString();
        }
    }
}