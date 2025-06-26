using System;
using UnityEditor;
using UnityEngine;

namespace KiraraTerminal
{
    public class TerminalWindow : EditorWindow
    {
        [MenuItem("Assets/Kirara Terminal %m")]
        public static void GetWindow()
        {
            GetWindow<TerminalWindow>("Kirara 终端");
        }

        private Shell shell = new();

        private void OnEnable()
        {
            minSize = new Vector2(600, 400);
            maxSize = new Vector2(600, 400);
            // Debug.Log($"OnEnable {shell == null}");
        }

        private void OnGUI()
        {
            ProcessKeyboardEvents();
            // Debug.Log($"OnGUI {shell == null}");
            shell.Current = GUILayout.TextField(shell.Current);
        }

        private void ProcessKeyboardEvents()
        {
            var evt = Event.current;
            if (evt.type != EventType.KeyDown) return;

            bool consume = true;
            switch (evt.keyCode)
            {
                case KeyCode.Escape:
                {
                    Close();
                    break;
                }
                case KeyCode.Return:
                {
                    shell.Execute();
                    break;
                }
                case KeyCode.UpArrow:
                {
                    shell.MovePrev();
                    break;
                }
                case KeyCode.DownArrow:
                {
                    shell.MoveNext();
                    break;
                }
                default:
                {
                    consume = false;
                    break;
                }
            }
            if (consume)
                evt.Use();
        }
    }
}