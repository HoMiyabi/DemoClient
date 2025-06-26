// using UnityEditor;
// using UnityEngine;
//
// namespace Kirara.ActionEditor
// {
//     [CustomEditor(typeof(ActionSO))]
//     public class ActionSOInspector : UnityEditor.Editor
//     {
//         private SerializedProperty nameProp;
//         private SerializedProperty clipProp;
//
//         private void OnEnable()
//         {
//             nameProp = serializedObject.FindProperty("m_Name");
//             clipProp = serializedObject.FindProperty("clip");
//         }
//
//         public override void OnInspectorGUI()
//         {
//             serializedObject.Update();
//             EditorGUILayout.PropertyField(nameProp, new GUIContent("名字"));
//             EditorGUILayout.PropertyField(clipProp, new GUIContent("动画片段"));
//             serializedObject.ApplyModifiedProperties();
//         }
//     }
// }