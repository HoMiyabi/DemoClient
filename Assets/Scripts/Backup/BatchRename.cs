// using System;
// using UnityEditor;
// using UnityEngine;
//
// namespace Kirara.Editor
// {
//     public class BatchRename : EditorWindow
//     {
//         private string oldValue;
//         private string newValue;
//
//         [MenuItem("Assets/批量重命名")]
//         public static void ShowWindow()
//         {
//             GetWindow<BatchRename>().Show();
//         }
//
//         private void OnGUI()
//         {
//             oldValue = EditorGUILayout.TextField("替换旧值", oldValue);
//             newValue = EditorGUILayout.TextField("替换新值", newValue);
//
//             if (GUILayout.Button("替换"))
//             {
//                 foreach (var asset in Selection.objects)
//                 {
//                     var path = AssetDatabase.GetAssetPath(asset);
//                     AssetDatabase.RenameAsset(path, asset.name.Replace(oldValue, newValue));
//                 }
//                 AssetDatabase.SaveAssets();
//                 AssetDatabase.Refresh();
//             }
//         }
//     }
// }