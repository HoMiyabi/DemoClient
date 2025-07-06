using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;

namespace Kirara.ActionEditor
{
    public class ActionListWindow : EditorWindow
    {
        private const float LABEL_WIDTH = 50f;

        private TextAsset text;
        private ActionListSO _actionList;
        private GameObject _prefab;
        private ActionSO _action;
        private ActionSO Action
        {
            get => _action;
            set
            {
                _action = value;
                DetailsWindow.Action = _action;
                TimelineWindow.Action = _action;
            }
        }

        private string searchQuery;
        private readonly List<(ActionSO action, long score)> actions = new();

        private Vector2 scrollPos;

        private ActionDetailsWindow _detailsWindow;
        private ActionDetailsWindow DetailsWindow
        {
            get
            {
                if (!_detailsWindow)
                {
                    _detailsWindow = ActionDetailsWindow.GetWindow();
                }
                return _detailsWindow;
            }
        }
        private ActionTimelineWindow _timelineWindow;
        private ActionTimelineWindow TimelineWindow
        {
            get
            {
                if (!_timelineWindow)
                {
                    _timelineWindow = ActionTimelineWindow.GetWindow();
                }
                return _timelineWindow;
            }
        }

        [MenuItem("Kirara/New动作列表")]
        public static void GetWindow()
        {
            var window = GetWindow<ActionListWindow>("动作列表");
            window.Init();
        }

        private void Init()
        {
            _ = DetailsWindow;
            _ = TimelineWindow;
        }

        private void OnGUI()
        {
            EditorGUIUtility.labelWidth = LABEL_WIDTH;

            EditorGUI.BeginChangeCheck();
            text = (TextAsset)EditorGUILayout.ObjectField("Json", text, typeof(TextAsset), false);
            if (EditorGUI.EndChangeCheck())
            {
                Debug.Log(text.GetType());
            }

            // 选择动作列表
            _actionList = (ActionListSO)EditorGUILayout.ObjectField(
                "动作列表", _actionList, typeof(ActionListSO), false);

            // 选择预制体
            _prefab = (GameObject)EditorGUILayout.ObjectField(
                "预制体", _prefab, typeof(GameObject), false);

            // 添加动作按钮
            using (new EditorGUI.DisabledGroupScope(!_actionList))
            {
                if (GUILayout.Button("+", GUILayout.ExpandWidth(false)))
                {
                    _actionList.AddAction();
                }
            }

            searchQuery = MyGUIUtils.ToolbarSearchField(searchQuery);

            using var s1 = new GUILayout.ScrollViewScope(scrollPos);
            scrollPos = s1.scrollPosition;

            if (!_actionList) return;

            actions.Clear();
            if (!string.IsNullOrEmpty(searchQuery))
            {
                foreach (var action in _actionList.actions)
                {
                    long score = 0;
                    if (FuzzySearch.FuzzyMatch(searchQuery, action.animName, ref score))
                    {
                        actions.Add((action, score));
                    }
                }
                actions.Sort((a, b) => b.score.CompareTo(a.score));
            }
            else
            {
                actions.AddRange(_actionList.actions.Select(a => (a, 0L)));
            }
            foreach (var (action, _) in actions)
            {
                var style = new GUIStyle(GUI.skin.button)
                {
                    alignment = TextAnchor.MiddleLeft,
                };
                MyGUIUtils.BeginHighlight(style, action == Action);
                if (GUILayout.Button(action.name, style))
                {
                    Action = action;
                }
                MyGUIUtils.EndHighlight();
            }
        }
    }
}