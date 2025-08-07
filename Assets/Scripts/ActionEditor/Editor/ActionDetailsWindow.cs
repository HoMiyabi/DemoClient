using System;
using UnityEditor;
using UnityEngine;

namespace Kirara.ActionEditor
{
    public class ActionDetailsWindow : EditorWindow
    {
        private ActionSO _action;
        public ActionSO Action
        {
            get => _action;
            set
            {
                if (_action == value) return;

                _action = value;
                Clear();
                _editor = UnityEditor.Editor.CreateEditor(_action);
            }
        }
        private UnityEditor.Editor _editor;

        private Vector2 scrollPos;

        public static ActionDetailsWindow GetWindow()
        {
            return GetWindow<ActionDetailsWindow>("动作细节面板");
        }

        private void Clear()
        {
            if (_editor)
            {
                DestroyImmediate(_editor);
                _editor = null;
            }
        }

        private void OnDestroy()
        {
            Clear();
        }

        private void Update()
        {
            Repaint();
        }

        private void OnGUI()
        {
            if (_editor)
            {
                DrawAction();
            }
            else
            {
                DrawNull();
            }
        }

        private void DrawAction()
        {
            using var s1 = new EditorGUILayout.ScrollViewScope(scrollPos);
            scrollPos = s1.scrollPosition;
            _editor.OnInspectorGUI();
        }

        private void DrawNull()
        {
            EditorGUILayout.HelpBox("未选择动作", MessageType.Info);
        }
    }
}