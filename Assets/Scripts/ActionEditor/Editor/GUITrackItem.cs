using Kirara.ActionEditor;
using UnityEditor;
using UnityEngine;

namespace ActionEditor.Editor
{
    public static class GUITrackItem
    {
        public static bool Draw(ActionTrackSO track, float height, Color color)
        {
            var rect = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.toolbar,
                GUILayout.ExpandWidth(true), GUILayout.Height(height));

            EditorGUI.DrawRect(rect, color);

            var e = Event.current;
            switch (e.type)
            {
                case EventType.MouseDown when e.button == 0 && rect.Contains(e.mousePosition):
                {
                    e.Use();
                    return true;
                }
                case EventType.ContextClick when rect.Contains(e.mousePosition):
                {
                    e.Use();
                    var menu = new GenericMenu();
                    if (ActionTimelineWindow.actionNotifyTypes.Length == 0)
                    {
                        menu.AddItem(new GUIContent("添加通知/无"), false, null);
                    }
                    foreach (var type in ActionTimelineWindow.actionNotifyTypes)
                    {
                        menu.AddItem(new GUIContent($"添加通知/{type.Name}"), false, () => track.AddActionNotify(type));
                    }
                    if (ActionTimelineWindow.actionNotifyStateTypes.Length == 0)
                    {
                        menu.AddItem(new GUIContent("添加通知状态/无"), false, null);
                    }
                    foreach (var type in ActionTimelineWindow.actionNotifyStateTypes)
                    {
                        menu.AddItem(new GUIContent($"添加通知状态/{type.Name}"), false, () => track.AddActionNotifyState(type));
                    }
                    menu.ShowAsContext();
                    break;
                }
            }
            return false;
        }
    }
}