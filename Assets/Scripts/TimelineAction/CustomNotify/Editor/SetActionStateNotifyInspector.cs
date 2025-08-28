using UnityEditor;
using UnityEngine;

namespace Kirara.TimelineAction
{
    [CustomEditor(typeof(SetActionStateNotify))]
    public class SetActionStateNotifyInspector : UnityEditor.Editor
    {
        private SetActionStateNotify _target;

        private void OnEnable()
        {
            _target = target as SetActionStateNotify;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("重置参数为状态默认"))
            {
                Undo.RecordObject(_target, "重置参数为状态默认");
                _target.actionParams = ActionParams.GetStateDefault(_target.actionState);
                EditorUtility.SetDirty(_target);
            }
        }
    }
}