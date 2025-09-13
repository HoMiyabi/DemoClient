// using UnityEditor;
// using UnityEngine;
//
// namespace Kirara.Timeline
// {
//     [CustomEditor(typeof(RandomAudioPlayableAsset))]
//     public class RandomAudioPlayableAssetEditor : UnityEditor.Editor
//     {
//         public override void OnInspectorGUI()
//         {
//             base.OnInspectorGUI();
//             var instance = (RandomAudioPlayableAsset)target;
//             if (GUILayout.Button("设置Duration为Max Length"))
//             {
//                 instance.timelineClip.duration = instance.audioClipsMaxLength;
//             }
//         }
//     }
// }