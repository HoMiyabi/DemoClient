using UnityEditor;
using UnityEngine;

namespace Kirara.TimelineAction
{
    [CustomEditor(typeof(CommandTransitionNotifyState))]
    public class CancelWindowPlayableAssetInspector : UnityEditor.Editor
    {
        private CommandTransitionNotifyState _target;
        private void OnEnable()
        {
            _target = (CommandTransitionNotifyState)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (ActionListWindow.Instance.ActionList == null)
            {
                EditorGUILayout.HelpBox("未选择动作列表", MessageType.Info);
                return;
            }
            string fullName = ActionListWindow.Instance.ActionList.namePrefix + _target.commandTransition.actionName;
            if (ActionListWindow.Instance.ActionList != null)
            {
                foreach (var action in ActionListWindow.Instance.ActionList.actions)
                {
                    if (action.name == fullName)
                    {
                        if (GUILayout.Button("切换动作"))
                        {
                            ActionListWindow.Instance.Action = action;
                        }
                        return;
                    }
                }
            }
            EditorGUILayout.HelpBox("未找到动作", MessageType.Info);
        }
    }
}