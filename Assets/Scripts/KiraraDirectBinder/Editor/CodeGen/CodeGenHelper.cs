using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KiraraDirectBinder.Editor
{
    internal static class CodeGenHelper
    {
        public static int GetTypeFullNameMaxLen(List<KiraraDirectBinder.Item> items)
        {
            return items
                .Select(x => GetFullNameOrEmpty(x.component).Length)
                .DefaultIfEmpty()
                .Max();
        }

        public static int GetVarNameMaxLen(List<KiraraDirectBinder.Item> items)
        {
            return items
                .Select(x => x.fieldName.Length)
                .DefaultIfEmpty()
                .Max();
        }

        public static string GetFullNameOrEmpty(Component component)
        {
            return component.GetType().FullName ?? string.Empty;
        }
    }
}