// using System;
// using System.Reflection;
// using UnityEditor;
// using UnityEngine;
//
// namespace Kirara
// {
//     public static class MyEditorGUILayout
//     {
//         private static readonly
//             Func<string, GUILayoutOption[], string> ToolbarSearchFieldFunc = GetToolbarSearchFieldFunc();
//         private static Func<string, GUILayoutOption[], string> GetToolbarSearchFieldFunc()
//         {
//             var method = typeof(EditorGUILayout).GetMethod(
//                 "ToolbarSearchField",
//                 BindingFlags.NonPublic | BindingFlags.Static,
//                 null,
//                 new[] {typeof(string), typeof(GUILayoutOption[])},
//                 null);
//             if (method == null)
//             {
//                 Debug.LogError("EditorGUILayout.ToolbarSearchField not found");
//                 return null;
//             }
//             var del = method.CreateDelegate(typeof(Func<string, GUILayoutOption[], string>));
//             return (Func<string, GUILayoutOption[], string>)del;
//         }
//
//         public static string ToolbarSearchField(string text, params GUILayoutOption[] options)
//         {
//             return ToolbarSearchFieldFunc(text, options);
//         }
//     }
// }