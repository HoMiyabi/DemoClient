// using System;
// using System.Collections.Generic;
// using System.Text;
// using System.Text.RegularExpressions;
// using UnityEditor;
// using UnityEngine;
//
// namespace Kirara.Editor
// {
//     public static class UIQuickBinder
//     {
//         // 忽略大小写
//         private static List<(string, Type)> tagType = new()
//         {
//             ("Tra", typeof(Transform)),
//             ("Btn", typeof(UnityEngine.UI.Button)),
//             ("Img", typeof(UnityEngine.UI.Image)),
//             ("Input", typeof(TMPro.TMP_InputField)),
//             ("Text", typeof(TMPro.TextMeshProUGUI)),
//             ("Dd", typeof(TMPro.TMP_Dropdown)),
//             ("LoopScroll", typeof(UnityEngine.UI.LoopScrollRect)),
//             ("Icon", typeof(UnityEngine.UI.Image))
//         };
//
//         private static readonly string ns = "Kirara.UI";
//
//         private static readonly StringBuilder pathSB = new();
//         private static readonly StringBuilder codeSB = new();
//         private static readonly Dictionary<string, (string path, string typeName)> dict = new();
//
//         [MenuItem("GameObject/生成UI绑定代码到剪切板", false, priority = 0)]
//         public static void Generate()
//         {
//             var transform = Selection.activeTransform;
//             if (transform == null)
//             {
//                 Debug.LogWarning("未选择物体");
//                 return;
//             }
//
//             pathSB.Clear();
//             codeSB.Clear();
//             dict.Clear();
//
//             foreach (Transform child in transform)
//             {
//                 Scan(child);
//             }
//             string code = GenerateCode();
//             GUIUtility.systemCopyBuffer = code;
//         }
//
//         private static void Scan(Transform transform)
//         {
//             string name = transform.name;
//             pathSB.Append(name);
//
//             foreach ((string tag, var type) in tagType)
//             {
//                 if ((name.EndsWith(tag, StringComparison.OrdinalIgnoreCase) ||
//                      Regex.IsMatch(name,
//                          $@"{Regex.Escape(tag)}\s+\(\d+\)$", RegexOptions.IgnoreCase)) &&
//                     transform.TryGetComponent(type, out _))
//                 {
//
//                     string fieldName = name.Replace('(', '_').Replace(')', '_').Replace(" ", "");
//                     if (dict.TryAdd(fieldName, (pathSB.ToString(), type.Name)))
//                     {
//                         Debug.Log($"1添加，原始名{name}");
//                     }
//                     else
//                     {
//                         Debug.LogWarning($"存在重名，原始名为{name}，字段名为{fieldName}");
//                     }
//                     goto endFind;
//                 }
//             }
//
//             int idx = name.IndexOf(':');
//             if (idx >= 0)
//             {
//                 dict.Add(name[..idx], (pathSB.ToString(), name[(idx + 1)..]));
//                 Debug.Log($"2添加，原始名{name}");
//                 goto end;
//             }
//
//             if (name.StartsWith("UI"))
//             {
//                 dict.Add(name, (pathSB.ToString(), name));
//                 Debug.Log($"3添加，原始名{name}");
//             }
//
// endFind:
//
//             if (!PrefabUtility.IsAnyPrefabInstanceRoot(transform.gameObject) && transform.childCount > 0)
//             {
//                 pathSB.Append('/');
//                 foreach (Transform child in transform)
//                 {
//                     Scan(child);
//                 }
//                 pathSB.Length -= 1;
//             }
//
//             end:
//
//             pathSB.Length -= name.Length;
//         }
//
//         private static string GenerateCode()
//         {
//             foreach ((string name, (string path, string typeName)) in dict)
//             {
//                 codeSB.Append($"private {typeName} {name};\n");
//             }
//
//             if (dict.Count > 0)
//             {
//                 codeSB.Append('\n');
//             }
//
//             codeSB.Append("private void InitUI()\n");
//             codeSB.Append("{\n");
//             foreach ((string name, (string path, string typeName)) in dict)
//             {
//                 string methodName;
//                 string newTypeName;
//                 if (typeName.EndsWith("[]"))
//                 {
//                     methodName = "GetComponentsInChildren";
//                     newTypeName = typeName[..^2];
//                 }
//                 else
//                 {
//                     methodName = "GetComponent";
//                     newTypeName = typeName;
//                 }
//                 codeSB.Append($"    {name} = transform.Find(\"{path}\").{methodName}<{newTypeName}>();\n");
//             }
//             codeSB.Append("}");
//
//             return codeSB.ToString();
//         }
//     }
// }