// using UnityEditor;
// using UnityEngine;
// using UnityEngine.Timeline;
//
// namespace Kirara.Timeline.Editor
// {
//     public class AniToTimeline : EditorWindow
//     {
//         public AnimationClip[] clips;
//         public string oldValue = "";
//         public string newValue = "";
//         public string temp;
//
//         private void OnGUI()
//         {
//             EditorGUILayout.LabelField($"选择了{clips.Length}项");
//             if (clips.Length == 0) return;
//
//             temp = EditorGUILayout.TextField("第0项的名称为", temp);
//
//             oldValue = EditorGUILayout.TextField("替换的旧值", oldValue);
//             newValue = EditorGUILayout.TextField("替换的新值", newValue);
//             if (GUILayout.Button("生成"))
//             {
//                 string absolutePath = EditorUtility.SaveFolderPanel("选择保存文件夹", Application.dataPath, "");
//
//                 if (string.IsNullOrEmpty(absolutePath)) return;
//                 string assetsPath = Application.dataPath;
//                 if (!absolutePath.StartsWith(assetsPath))
//                 {
//                     Debug.LogError("保存路径必须在Assets目录下");
//                     return;
//                 }
//                 string path = "Assets" + absolutePath.Substring(assetsPath.Length);
//
//                 foreach (var clip in clips)
//                 {
//                     var timelineAsset = CreateInstance<TimelineAsset>();
//                     var track = timelineAsset.CreateTrack<AnimationTrack>();
//                     var timelineClip = track.CreateClip<AnimationPlayableAsset>();
//                     var clipAsset = (AnimationPlayableAsset)timelineClip.asset;
//                     clipAsset.clip = clip;
//
//                     string fileName = clip.name;
//                     if (oldValue != "")
//                     {
//                         fileName = fileName.Replace(oldValue, newValue);
//                     }
//
//                     timelineAsset.name = fileName;
//                     AssetDatabase.CreateAsset(timelineAsset, $"{path}/{fileName}.playable");
//                 }
//                 AssetDatabase.SaveAssets();
//                 AssetDatabase.Refresh();
//             }
//         }
//
//         [MenuItem("Assets/动画生成Timeline")]
//         private static void ShowWindow()
//         {
//             var window = GetWindow<AniToTimeline>("动画生成Timeline");
//             window.clips = Selection.GetFiltered<AnimationClip>(SelectionMode.Unfiltered);
//             window.temp = window.clips[0].name;
//         }
//     }
// }