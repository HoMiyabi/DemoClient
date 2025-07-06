/*using System;
using UnityEditor;
using UnityEngine;

namespace Kirara.ActionEditor
{
    public class TrackDetailsWindow : EditorWindow
    {
        private const string TITLE = "轨道细节面板";
        private ActionEditorBackend backend;
        private UnityEditor.Editor editor;
        private ActionTrackSO track;

        [MenuItem("Kirara/轨道细节面板")]
        public static void GetWindow()
        {
            GetWindow<TrackDetailsWindow>(TITLE);
        }

        private void OnEnable()
        {
            backend = ActionEditorBackend.Instance;
        }

        private void OnDestroy()
        {
            ClearEditor();
        }

        private void ClearEditor()
        {
            if (editor != null)
            {
                DestroyImmediate(editor);
                editor = null;
            }
        }

        private void UpdateEditor()
        {
            if (backend.Track != track)
            {
                track = backend.Track;
                ClearEditor();
                if (track != null)
                {
                    editor = UnityEditor.Editor.CreateEditor(track);
                }
            }
        }

        private void Update()
        {
            UpdateEditor();
            Repaint();
        }

        private void OnGUI()
        {
            if (editor != null)
            {
                editor.OnInspectorGUI();
            }
            else
            {
                var style = new GUIStyle(EditorStyles.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                };
                GUILayout.Label("未选择轨道", style);
            }
        }
    }
}*/