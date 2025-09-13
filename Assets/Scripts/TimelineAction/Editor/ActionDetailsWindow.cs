using UnityEditor;
using UnityEngine;

namespace Kirara.TimelineAction.Editor
{
    public class ActionDetailsWindow : EditorWindow
    {
        private UnityEditor.Editor editor;
        private KiraraActionSO _action;
        public KiraraActionSO Action
        {
            get => _action;
            set
            {
                if (value == _action) return;

                _action = value;
                ClearEditor();
                if (_action)
                {
                    editor = UnityEditor.Editor.CreateEditor(_action);
                }
            }
        }
        private Vector2 scrollPos;

        public static ActionDetailsWindow GetWindow()
        {
            var window = GetWindow<ActionDetailsWindow>("动作细节");
            return window;
        }

        private void OnDestroy()
        {
            ClearEditor();
        }

        private void ClearEditor()
        {
            if (editor)
            {
                DestroyImmediate(editor);
                editor = null;
            }
        }

        private void OnGUI()
        {
            using var s = new GUILayout.ScrollViewScope(scrollPos);
            scrollPos = s.scrollPosition;
            if (editor)
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
            editor.OnInspectorGUI();
        }

        private void DrawNull()
        {
            EditorGUILayout.HelpBox("未选择动作", MessageType.Info);
        }
    }
}