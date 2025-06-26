using UnityEditor;
using UnityEngine;
using XNodeEditor;

namespace Kirara.Editor
{
    [CustomNodeGraphEditor(typeof(MyNodeGraph))]
    public class MyNodeGraphEditor : NodeGraphEditor
    {
        public Transform tra;

        public override void OnGUI()
        {
            tra = (Transform)EditorGUILayout.ObjectField(
                tra,
                typeof(Transform),
                true,
                GUILayout.ExpandWidth(false));
        }

        public override void OnDropObjects(Object[] objects)
        {
        }
    }
}