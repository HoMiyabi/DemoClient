using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Kirara.TimelineAction
{
    [CustomEditor(typeof(KiraraActionSO))]
    public class KiraraActionSOInspector : UnityEditor.Editor
    {
        private SerializedProperty m_DurationModeProp;
        private SerializedProperty actionIdProp;
        private SerializedProperty isLoopProp;
        private SerializedProperty actionArgsProp;
        private SerializedProperty finishTransitionProp;
        private SerializedProperty inheritActionTransitionProp;
        private SerializedProperty commandTransitionsProp;
        private SerializedProperty signalTransitionsProp;

        private void OnEnable()
        {
            m_DurationModeProp = serializedObject.FindProperty("m_DurationMode");
            isLoopProp = serializedObject.FindProperty(nameof(KiraraActionSO.isLoop));
            actionIdProp = serializedObject.FindProperty(nameof(KiraraActionSO.actionId));
            actionArgsProp = serializedObject.FindProperty(nameof(KiraraActionSO.actionArgs));
            finishTransitionProp = serializedObject.FindProperty(nameof(KiraraActionSO.finishTransition));
            inheritActionTransitionProp = serializedObject.FindProperty(nameof(KiraraActionSO.inheritActionTransition));
            commandTransitionsProp = serializedObject.FindProperty(nameof(KiraraActionSO.commandTransitions));
            signalTransitionsProp = serializedObject.FindProperty(nameof(KiraraActionSO.signalTransitions));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_DurationModeProp);
            EditorGUILayout.PropertyField(actionIdProp);
            EditorGUILayout.PropertyField(isLoopProp, new GUIContent("循环"));
            EditorGUILayout.PropertyField(actionArgsProp, new GUIContent("动作参数"));
            EditorGUILayout.PropertyField(inheritActionTransitionProp, new GUIContent("继承此动作的跳转"));
            EditorGUILayout.PropertyField(finishTransitionProp, new GUIContent("结束跳转"));
            EditorGUILayout.PropertyField(commandTransitionsProp, new GUIContent("指令跳转"));
            EditorGUILayout.PropertyField(signalTransitionsProp, new GUIContent("信号跳转"));

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("跳转到本动作的动作:");
            var instance = ActionListWindow.Instance;
            if (instance && instance.ActionList &&
                instance.ActionList.actions != null && instance.Action == target)
            {
                string shortName = target.name;
                if (shortName.StartsWith(instance.ActionList.namePrefix))
                {
                    shortName = shortName[instance.ActionList.namePrefix.Length..];
                }
                EditorGUI.indentLevel++;

                EditorGUILayout.LabelField("结束跳转:");
                EditorGUI.indentLevel++;
                foreach (var action in instance.ActionList.actions)
                {
                    if (action == null) continue;
                    if (action.finishTransition.actionName == shortName)
                    {
                        DrawLabelAndOpen(instance, action.name, action);
                    }
                }
                EditorGUI.indentLevel--;

                EditorGUILayout.LabelField("指令跳转:");
                EditorGUI.indentLevel++;
                foreach (var action in instance.ActionList.actions)
                {
                    if (action == null) continue;
                    foreach (var transition in action.commandTransitions)
                    {
                        if (transition.actionName == shortName)
                        {
                            DrawLabelAndOpen(instance, $"{action.name}: {transition.command}, {transition.phase}", action);
                        }
                    }
                }
                EditorGUI.indentLevel--;

                EditorGUILayout.LabelField("信号跳转:");
                EditorGUI.indentLevel++;
                foreach (var action in instance.ActionList.actions)
                {
                    if (action == null) continue;

                    foreach (var transition in action.signalTransitions)
                    {
                        if (transition.actionName == shortName)
                        {
                            DrawLabelAndOpen(instance, $"{action.name}: {transition.signalName}", action);
                        }
                    }
                }
                EditorGUI.indentLevel--;

                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawLabelAndOpen(ActionListWindow instance, string label, KiraraActionSO action)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label);
            if (GUILayout.Button("打开", GUILayout.ExpandWidth(false)))
            {
                instance.Action = action;
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}