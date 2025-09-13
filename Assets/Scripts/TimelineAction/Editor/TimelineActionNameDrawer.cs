using UnityEditor;
using UnityEngine;

namespace Kirara.TimelineAction.Editor
{
    [CustomPropertyDrawer(typeof(TimelineActionNameAttribute))]
    public class TimelineActionNameDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                var instance = ActionListWindow.Instance;
                KiraraActionSO next = null;
                if (instance != null && instance.ActionList != null)
                {
                    string fullName = instance.ActionList.namePrefix + property.stringValue;
                    next = instance.ActionList.actions.Find(a => a.name == fullName);
                }

                float width = GUI.skin.button.CalcSize(new GUIContent("打开")).x;
                position.width -= width;
                EditorGUI.PropertyField(position, property, label);

                position.x += position.width;
                position.width = width;
                EditorGUI.BeginDisabledGroup(next == null);
                if (GUI.Button(position, "打开"))
                {
                    // 不延迟可能会切换Editor导致NullReferenceException: SerializedObject of SerializedProperty has been Disposed.
                    EditorApplication.delayCall += () => instance.Action = next;
                }
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                EditorGUI.HelpBox(position, $"{label.text} 只支持string类型", MessageType.Info);
            }
        }
    }
}