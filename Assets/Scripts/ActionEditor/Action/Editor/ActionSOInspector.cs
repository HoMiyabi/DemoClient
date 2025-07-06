using UnityEditor;
using UnityEngine;

namespace Kirara.ActionEditor
{
    [CustomEditor(typeof(ActionSO))]
    public class ActionSOInspector : Editor
    {
        private SerializedProperty nameProp;
        private SerializedProperty aniNameProp;

        private void OnEnable()
        {
            nameProp = serializedObject.FindProperty("m_Name");
            aniNameProp = serializedObject.FindProperty("animName");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(nameProp, new GUIContent("名字"));
            EditorGUILayout.PropertyField(aniNameProp, new GUIContent("动画名"));
            serializedObject.ApplyModifiedProperties();
        }
    }
}