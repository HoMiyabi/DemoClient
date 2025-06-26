using System.Text;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using XNode;
using XNodeEditor;

namespace Kirara.Editor
{
    [CustomNodeEditor(typeof(StateNode))]
    public class StateNodeEditor : NodeEditor
    {
        private StateNode node;
        private MyNodeGraphEditor graphEditor;

        public override void OnHeaderGUI()
        {
            base.OnHeaderGUI();
        }

        private void EditClick()
        {

        }

        public override void OnBodyGUI()
        {
            node = (StateNode)target;
            graphEditor = (MyNodeGraphEditor)window.graphEditor;

            // base.OnBodyGUI();

            var tra = graphEditor.tra;
            PlayableDirector director = null;
            Animator animator = null;
            if (tra != null)
            {
                director = tra.GetComponent<PlayableDirector>();
                animator = tra.GetComponent<Animator>();
            }
            var action = node.action;

            if (director != null)
            {
                GUILayout.Label(director.time.ToString());
            }

            if (action != null)
            {
                GUILayout.Label(action.duration.ToString());
            }

            if (GUILayout.Button("编辑"))
            {
                if (director != null && animator != null && action != null)
                {
                    director.playableAsset = action;

                    foreach (var track in action.GetRootTracks())
                    {
                        if (track is AnimationTrack)
                        {
                            director.SetGenericBinding(track, animator);
                        }
                    }


                    var win = EditorWindow.GetWindow<TimelineEditorWindow>();
                    if (win != null)
                    {
                        win.SetTimeline(director);
                        win.Repaint();
                    }
                }
                else
                {
                    Debug.LogWarning(GetNullWarning(
                        director, nameof(PlayableDirector),
                        animator, nameof(Animator),
                        action, "Action"));
                }
            }

            // NodeEditorGUILayout.DynamicPortList(
            //     nameof(node.transitions),
            //     typeof(NodeTransition),
            //     serializedObject,
            //     NodePort.IO.Output,
            //     Node.ConnectionType.Override,
            //     Node.TypeConstraint.None,
            //     OnCreateReorderableList);
        }

        private static string GetNullWarning(params object[] objects)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < objects.Length; i += 2)
            {
                if (objects[i] == null && i + 1 < objects.Length)
                {
                    if (sb.Length != 0)
                    {
                        sb.Append(", ");
                    }
                    sb.Append(objects[i + 1]);
                    sb.Append(" == null");
                }
            }
            return sb.ToString();
        }

        private void OnCreateReorderableList(ReorderableList list)
        {
            // // 设置元素高度回调
            // list.elementHeightCallback = (index) =>
            // {
            //     // 每个属性字段的高度 + 行间距
            //     float lineHeight = EditorGUIUtility.singleLineHeight + 2;
            //     // 4个字段 + 底部额外间距
            //     return lineHeight * 4 + 4;
            // };

            // list.drawElementCallback = (rect, index, active, focused) =>
            // {
            //     XNode.NodePort port = node.GetPort(fieldName + " " + index);
            //     if (hasArrayData && arrayData.propertyType != SerializedPropertyType.String) {
            //         if (arrayData.arraySize <= index) {
            //             EditorGUI.LabelField(rect, "Array[" + index + "] data out of range");
            //             return;
            //         }
            //         SerializedProperty itemData = arrayData.GetArrayElementAtIndex(index);
            //         EditorGUI.PropertyField(rect, itemData, true);
            //     } else EditorGUI.LabelField(rect, port != null ? port.fieldName : "");
            //     if (port != null) {
            //         Vector2 pos = rect.position + (port.IsOutput ? new Vector2(rect.width + 6, 0) : new Vector2(-36, 0));
            //         NodeEditorGUILayout.PortField(pos, port);
            //     }
            //
            //     float height = EditorGUIUtility.singleLineHeight;
            //     float dy = EditorGUIUtility.singleLineHeight + 2;
            //
            //     // var ele = serializedObject
            //     //     .FindProperty(nameof(StateNode.transitions)).GetArrayElementAtIndex(index);
            //     // GUILayout.BeginArea(rect);
            //     EditorGUI.LabelField(rect, "123");
            //     // EditorGUILayout.PropertyField(ele);
            //     // GUILayout.EndArea();
            //
            //     // var bufferDuration = ele.FindPropertyRelative(nameof(NodeTransition.bufferDuration));
            //     // var beginTime = ele.FindPropertyRelative(nameof(NodeTransition.beginTime));
            //     // var endTime = ele.FindPropertyRelative(nameof(NodeTransition.endTime));
            //     // var command = ele.FindPropertyRelative(nameof(NodeTransition.command));
            //     // EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, height), bufferDuration);
            //     // rect.y += dy;
            //     // EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, height), beginTime);
            //     // rect.y += dy;
            //     // EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, height), endTime);
            //     // rect.y += dy;
            //     // EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, height), command);
            // };

            // list.drawHeaderCallback = rect => {
            //     EditorGUI.LabelField(rect, "123");
            // };
        }
    }
}