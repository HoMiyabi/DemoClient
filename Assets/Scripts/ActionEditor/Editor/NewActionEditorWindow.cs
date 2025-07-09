using System;
using UnityEditor;

namespace ActionEditor.Editor
{
    public class NewActionEditorWindow : EditorWindow
    {
        [MenuItem("Kirara/New Action Editor Window")]
        public static void GetWindow()
        {
            var window = GetWindow<NewActionEditorWindow>();
        }

        private void OnGUI()
        {
            // AnimationClipEditor
        }
    }
}