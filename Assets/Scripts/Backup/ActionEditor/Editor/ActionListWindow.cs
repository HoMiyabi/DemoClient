/*using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;

namespace Kirara.ActionEditor
{
    public class ActionListWindow : EditorWindow
    {
        private const string TITLE = "动作列表";
        private const string ACTION_LIST = "动作列表";
        private const string PREFAB = "预制体";
        private const float LABEL_WIDTH = 50f;

        private ActionEditorBackend be;

        private string searchQuery;
        private List<(ActionSO, long)> actions;

        private Vector2 scrollPos;

        [MenuItem("Kirara/动作列表")]
        public static void GetWindow()
        {
            GetWindow<ActionListWindow>(TITLE);
        }

        private void OnEnable()
        {
            be = ActionEditorBackend.Instance;
            actions ??= new List<(ActionSO, long)>();
        }

        private void OnGUI()
        {
            EditorGUIUtility.labelWidth = LABEL_WIDTH;

            // 动作列表
            be.ActionList = (ActionListSO)EditorGUILayout.ObjectField(
                ACTION_LIST, be.ActionList,typeof(ActionListSO), false);

            // 预制体
            be.Prefab = (GameObject)EditorGUILayout.ObjectField(
                PREFAB, be.Prefab, typeof(GameObject), false);

            // 添加按钮
            using (new EditorGUI.DisabledGroupScope(be.ActionList == null))
            {
                if (GUILayout.Button("+", GUILayout.ExpandWidth(false)))
                {
                    be.ActionList.AddAction();
                }
            }

            // searchQuery = MyEditorGUILayout.ToolbarSearchField(searchQuery);

            using var s1 = new GUILayout.ScrollViewScope(scrollPos);
            scrollPos = s1.scrollPosition;

            if (be.ActionList == null || be.ActionList.actions == null) return;

            actions.Clear();
            if (!string.IsNullOrEmpty(searchQuery))
            {
                foreach (var action in be.ActionList.actions)
                {
                    long score = 0;
                    if (FuzzySearch.FuzzyMatch(searchQuery, action.name, ref score))
                    {
                        actions.Add((action, score));
                    }
                }
                actions.Sort((a, b) => b.Item2.CompareTo(a.Item2));
            }
            else
            {
                actions.AddRange(be.ActionList.actions.Select(a => (a, 0L)));
            }
            foreach (var (action, _) in actions)
            {
                var style = new GUIStyle(GUI.skin.button)
                {
                    alignment = TextAnchor.MiddleLeft,
                };
                MyGUIUtils.BeginHighlight(style, action == be.Action);
                if (GUILayout.Button(action.name, style))
                {
                    be.Action = action;
                }
                MyGUIUtils.EndHighlight();
            }
        }
    }
}*/