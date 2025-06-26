/*using UnityEditor;
using UnityEngine;

namespace Kirara.ActionEditor
{
    public class ActionDetailsWindow : EditorWindow
    {
        private const string TITLE = "动作细节面板";
        private ActionEditorBackend backend;
        private UnityEditor.Editor editor;
        private ActionSO action;

        [MenuItem("Kirara/动作细节面板")]
        public static void GetWindow()
        {
            GetWindow<ActionDetailsWindow>(TITLE);
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
            if (backend.Action != action)
            {
                action = backend.Action;
                ClearEditor();
                if (action != null)
                {
                    editor = UnityEditor.Editor.CreateEditor(action);
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
                GUILayout.Label("未选择动作", style);
            }
        }
    }
}*/