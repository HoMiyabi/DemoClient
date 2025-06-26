/*using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Kirara.ActionEditor
{
    public static class MyGUIUtils
    {
        static MyGUIUtils()
        {
            InitToolbarSearchField();
        }

        private static Func<string, GUILayoutOption[], string> ToolbarSearchFieldFunc;

        private static void InitToolbarSearchField()
        {
            var method = typeof(EditorGUILayout).GetMethod(
                "ToolbarSearchField",
                BindingFlags.NonPublic | BindingFlags.Static,
                null,
                new[] {typeof(string), typeof(GUILayoutOption[])},
                null);
            if (method == null)
            {
                Debug.LogError("EditorGUILayout.ToolbarSearchField not found");
                return;
            }
            var del = method.CreateDelegate(typeof(Func<string, GUILayoutOption[], string>));
            ToolbarSearchFieldFunc = (Func<string, GUILayoutOption[], string>)del;
        }

        public static string ToolbarSearchField(string text, params GUILayoutOption[] options)
        {
            return ToolbarSearchFieldFunc(text, options);
        }


        private static Color color;
        public static void BeginHighlight(GUIStyle style, bool condition)
        {
            color = GUI.backgroundColor;
            if (condition)
            {
                GUI.backgroundColor = Color.cyan + new Color(0,0, 0.2f);
                style.fontStyle = FontStyle.Bold;
            }
        }

        public static void EndHighlight()
        {
            GUI.backgroundColor = color;
        }
    }
}*/