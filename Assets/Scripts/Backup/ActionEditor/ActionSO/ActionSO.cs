// using System;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEditor;
//
// namespace Kirara.ActionEditor
// {
//     // 动作
//     public class ActionSO : ScriptableObject
//     {
//         public AnimationClip clip;
//         public List<ActionTrackSO> tracks;
//
// #if UNITY_EDITOR
//         private string GetDisplayName(object obj)
//         {
//             return ObjectNames.NicifyVariableName(obj.GetType().Name);
//         }
//
//         public ActionTrackSO AddTrack(Type type)
//         {
//             // Undo.RecordObject(this, "添加轨道");
//             var track = CreateInstance(type) as ActionTrackSO;
//             if (track == null)
//             {
//                 Debug.LogError($"{type.Name} 不是一个有效的轨道类型");
//                 return null;
//             }
//             track.name = GetDisplayName(track);
//             tracks ??= new List<ActionTrackSO>();
//             tracks.Add(track);
//             AssetDatabase.AddObjectToAsset(track, this);
//             // EditorUtility.SetDirty(this);
//             AssetDatabase.SaveAssets();
//             // 子资产应该不用刷新
//
//             return track;
//         }
//
//         public bool RemoveTrack(ActionTrackSO track)
//         {
//             if (!tracks.Remove(track)) return false;
//
//             // Undo.RecordObject(this, $"删除轨道 {track.name}");
//             AssetDatabase.RemoveObjectFromAsset(track);
//             DestroyImmediate(track, true);
//             EditorUtility.SetDirty(this);
//             AssetDatabase.SaveAssets();
//
//             return true;
//         }
// #endif
//     }
// }