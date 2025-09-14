using System.Text;
using UnityEngine;

namespace KiraraDirectBinder.Editor
{
    internal static class LuaGen
    {
        public static string Run(KiraraDirectBinder binder)
        {
            // 变量名最大长度
            int varNameMaxLen = CodeGenHelper.GetVarNameMaxLen(binder.items);
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
    }
}