// using System.Linq;
// using UnityEditor;
// using UnityEngine;
//
// namespace Kirara.Editor
// {
//     public static class AnimationClipRenameAsFBX
//     {
//         [MenuItem("Assets/FBX里的AnimationClip改为和FBX同名")]
//         public static void RenameAsFBX()
//         {
//             foreach (var guid in Selection.assetGUIDs)
//             {
//                 var assetPath = AssetDatabase.GUIDToAssetPath(guid);
//                 var go = AssetDatabase.LoadAssetAtPath<GameObject>(guid);
//                 if (AssetImporter.GetAtPath(assetPath) is ModelImporter importer)
//                 {
//                     var clips = importer.clipAnimations;
//                     if (clips.Length == 1)
//                     {
//                         clips[0].takeName = go.name;
//                         clips[0].name = go.name;
//                         EditorUtility.SetDirty(importer);
//                         importer.SaveAndReimport();
//                     }
//                 }
//             }
//         }
//     }
// }