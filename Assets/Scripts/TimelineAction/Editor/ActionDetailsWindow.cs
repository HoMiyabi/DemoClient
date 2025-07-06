using UnityEditor;
using UnityEngine;

namespace Kirara.TimelineAction
{
    public class ActionDetailsWindow : EditorWindow
    {
        private UnityEditor.Editor editor;
        private KiraraActionSO _action;
        private Vector2 scrollPos;

        public static ActionDetailsWindow GetWindow()
        {
            var window = GetWindow<ActionDetailsWindow>("动作细节面板");
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

        private void UpdateEditor()
        {
            if (ActionListWindow.Instance == null) return;
            if (ActionListWindow.Instance.Action != _action)
            {
                _action = ActionListWindow.Instance.Action;
                ClearEditor();
                if (_action)
                {
                    editor = UnityEditor.Editor.CreateEditor(_action);
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

            if (!ActionListWindow.Instance.ActionList) return;

            var instance = ActionListWindow.Instance;
            string fullName = instance.ActionList.namePrefix + _action.finishCancelInfo.actionName;
            var next = instance.ActionList.actions.Find(a => a.name == fullName);
            if (next)
            {
                if (GUILayout.Button("切换动作"))
                {
                    instance.Action = next;
                }
            }
            else
            {
                EditorGUILayout.HelpBox("未找到动作", MessageType.Info);
            }

            if (GUILayout.Button("重置参数为状态默认"))
            {
                Undo.RecordObject(_action, "重置参数为状态默认");
                _action.actionParams = ActionParams.GetStateDefault(_action.actionState);
                EditorUtility.SetDirty(_action);
            }
        }

        private void DrawNull()
        {
            EditorGUILayout.HelpBox("未选择动作", MessageType.Info);
        }
    }
}