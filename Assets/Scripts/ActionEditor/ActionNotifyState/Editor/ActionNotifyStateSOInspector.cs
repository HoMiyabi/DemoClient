using UnityEditor;
using UnityEngine;

namespace Kirara.ActionEditor
{
    [CustomEditor(typeof(ActionNotifyStateSO))]
    public class ActionNotifyStateSOInspector : UnityEditor.Editor
    {
        private ActionNotifyStateSO _target;
        private SerializedProperty nameProp;

        public void OnEnable()
        {
            _target = target as ActionNotifyStateSO;
            nameProp = serializedObject.FindProperty("m_Name");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(nameProp, new GUIContent("名字"));
            DrawStart();
            DrawDuration();

            serializedObject.ApplyModifiedProperties();
            // base.OnInspectorGUI();
        }

        private void DrawStart()
        {
            using var _ = new GUILayout.HorizontalScope();

            float width = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 15;

            GUILayout.Label("开始", GUILayout.Width(width));

            EditorGUI.BeginChangeCheck();
            float t = EditorGUILayout.FloatField("秒", _target.start);
            if (EditorGUI.EndChangeCheck())
            {
                _target.start = Mathf.Max(t, 0f);
                EditorUtility.SetDirty(_target);
            }

            EditorGUI.BeginChangeCheck();
            t = EditorGUILayout.FloatField("帧", _target.start * ActionTimelineWindow.FrameRate);
            if (EditorGUI.EndChangeCheck())
            {
                _target.start = Mathf.Max(t / ActionTimelineWindow.FrameRate, 0f);
                EditorUtility.SetDirty(_target);
            }

            EditorGUIUtility.labelWidth = width;
        }

        private void DrawDuration()
        {
            using var _ = new GUILayout.HorizontalScope();

            float width = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 15;

            GUILayout.Label("时长", GUILayout.Width(width));

            EditorGUI.BeginChangeCheck();
            float t = EditorGUILayout.FloatField("秒", _target.duration);
            if (EditorGUI.EndChangeCheck())
            {
                _target.duration = Mathf.Max(t, 0f);
                EditorUtility.SetDirty(_target);
            }

            EditorGUI.BeginChangeCheck();
            t = EditorGUILayout.FloatField("帧", t * ActionTimelineWindow.FrameRate);
            if (EditorGUI.EndChangeCheck())
            {
                _target.duration = Mathf.Max(t / ActionTimelineWindow.FrameRate, 0f);
                EditorUtility.SetDirty(_target);
            }

            EditorGUIUtility.labelWidth = width;
        }
    }
}