using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Kirara.TimelineAction
{
    public class ActionDetailsWindow : EditorWindow
    {
        private const string TITLE = "动作细节面板";
        private UnityEditor.Editor editor;
        private KiraraActionSO action;
        private Vector2 scrollPos;

        public static ActionDetailsWindow GetWindow()
        {
            var window = GetWindow<ActionDetailsWindow>(TITLE);
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
            if (ActionListWindow.Instance.Action != action)
            {
                action = ActionListWindow.Instance.Action;
                ClearEditor();
                if (action)
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
            using var s = new GUILayout.ScrollViewScope(scrollPos);
            scrollPos = s.scrollPosition;
            if (!editor)
            {
                EditorGUILayout.HelpBox("未选择动作", MessageType.Info);
                return;
            }

            editor.OnInspectorGUI();

            if (ActionListWindow.Instance.ActionList == null) return;

            var instance = ActionListWindow.Instance;
            string fullName = instance.ActionList.namePrefix + action.finishCancelInfo.actionName;
            var next = instance.ActionList.actions.Find(a => a.name == fullName);
            if (next != null)
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
                Undo.RecordObject(action, "重置参数为状态默认");
                action.actionParams = ActionParams.GetStateDefault(action.actionState);
                EditorUtility.SetDirty(action);
            }
        }
    }
}