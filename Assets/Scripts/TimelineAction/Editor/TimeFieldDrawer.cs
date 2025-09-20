using UnityEditor;
using UnityEngine;

namespace Kirara.TimelineAction.Editor
{
    [CustomPropertyDrawer(typeof(TimeFieldAttribute))]
    public class TimeFieldDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attr = (TimeFieldAttribute)attribute;
            if (property.propertyType == SerializedPropertyType.Float)
            {
                var rect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
                EditorGUI.LabelField(rect, label);

                float oldLabelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 16;

                rect.x += rect.width;
                rect.width = (position.width - rect.width) * 0.5f;
                EditorGUI.PropertyField(rect, property, new GUIContent("秒"));

                rect.x += rect.width;
                EditorGUI.BeginChangeCheck();
                float t = EditorGUI.FloatField(rect, "帧", property.floatValue * attr.FrameRate);
                if (EditorGUI.EndChangeCheck())
                {
                    property.floatValue = t / attr.FrameRate;
                }

                EditorGUIUtility.labelWidth = oldLabelWidth;
            }
            else
            {
                EditorGUI.HelpBox(position, $"{label.text} 只支持float类型", MessageType.Info);
            }
        }
    }
}