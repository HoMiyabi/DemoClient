using UnityEditor;
using UnityEngine;

namespace Kirara.TimelineAction
{
    [CustomPropertyDrawer(typeof(SecondFrameAttribute))]
    public class SecondFrameDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attr = (SecondFrameAttribute)attribute;
            if (property.propertyType == SerializedPropertyType.Float)
            {
                var rect = new Rect(position.x, position.y, position.width / 3, position.height);
                EditorGUI.LabelField(rect, label);

                float width = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 18;
                {
                    rect.x += position.width / 3;
                    property.floatValue = EditorGUI.FloatField(rect, "秒", property.floatValue);

                    rect.x += position.width / 3;
                    EditorGUI.BeginChangeCheck();
                    float t = EditorGUI.FloatField(rect, "帧", property.floatValue * attr.FrameRate);
                    if (EditorGUI.EndChangeCheck())
                    {
                        property.floatValue = t / attr.FrameRate;
                    }
                }
                EditorGUIUtility.labelWidth = width;
            }
            else
            {
                EditorGUI.HelpBox(position, $"{label.text} 只支持float类型", MessageType.Info);
            }
        }
    }
}