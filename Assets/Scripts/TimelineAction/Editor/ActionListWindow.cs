using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using SearchField = UnityEditor.IMGUI.Controls.SearchField;

namespace Kirara.TimelineAction
{
    public class ActionListWindow : EditorWindow
    {
        public static ActionListWindow Instance { get; private set; }

        private KiraraActionListSO _actionList;
        public KiraraActionListSO ActionList {get => _actionList; set => SetActionList(value); }

        private KiraraActionSO _action;
        public KiraraActionSO Action { get => _action; set => SetAction(value); }

        private GameObject _go;
        private GameObject GO { get => _go; set => SetGO(value); }

        private Animator _animator;
        private ActionCtrl _actionCtrl;
        private PlayableDirector _director;
        private RuntimeAnimatorController _rtAnimCtrl;

        private SearchField searchField;
        private string searchText;
        private List<(KiraraActionSO, long)> actions;

        private Vector2 scrollPos;

        [MenuItem("Kirara/动作列表")]
        public static void GetWindow()
        {
            GetWindow<ActionListWindow>("动作列表");
            ActionDetailsWindow.GetWindow();
        }

        private void OnEnable()
        {
            actions ??= new List<(KiraraActionSO, long)>();
            if (Instance != null)
            {
                Debug.LogWarning("ActionListWindow already exists!");
            }
            Instance = this;
            searchField = new SearchField();
        }

        private void SetActionList(KiraraActionListSO actionList, bool updateGO = true)
        {
            if (actionList == _actionList) return;

            _actionList = actionList;
            if (updateGO)
            {
                UpdateGO();
            }
            Action = ActionList ? ActionList.actions?.FirstOrDefault() : null;
        }

        private void SetAction(KiraraActionSO action)
        {
            _action = action;

            // 更新Timeline窗口
            var timelineWindow = TimelineWindowHelper.GetWindow();
            if (_director)
            {
                _director.playableAsset = _action;

                foreach (var track in _action.GetRootTracks())
                {
                    if (track is AnimationTrack)
                    {
                        _director.SetGenericBinding(track, _animator);
                        break;
                    }
                }
                timelineWindow.SetTimeline(_director);
            }
            else
            {
                timelineWindow.SetTimeline(_action);
            }
            timelineWindow.Repaint();

            // 更新动作细节窗口
            var detailsWindow = ActionDetailsWindow.GetWindow();
            detailsWindow.Action = action;
            detailsWindow.Repaint();
        }

        private void UpdateGO()
        {
            var actionCtrls = FindObjectsByType<ActionCtrl>(FindObjectsSortMode.None);
            foreach (var ctrl in actionCtrls)
            {
                if (ctrl.actionList == ActionList)
                {
                    GO = ctrl.gameObject;
                    return;
                }
            }
            GO = null;
        }

        private void SetGO(GameObject go)
        {
            if (go == _go) return;

            _go = go;
            if (_go)
            {
                _actionCtrl = _go.GetComponent<ActionCtrl>();
                _animator = _go.GetComponent<Animator>();
                _director = _go.GetComponent<PlayableDirector>();
                SetActionList(_actionCtrl ? _actionCtrl.actionList : null, false);
            }
            else
            {
                _animator = null;
                _actionCtrl = null;
                _director = null;
                SetActionList(null, false);
            }
        }

        private void OnGUI()
        {
            float oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 80f;

            // 动作列表
            ActionList = (KiraraActionListSO)EditorGUILayout.ObjectField(
                "动作列表", ActionList, typeof(KiraraActionListSO), false);

            // GameObject
            GO = (GameObject)EditorGUILayout.ObjectField(
                "GameObject", GO, typeof(GameObject), true);

            // 名字前缀
            if (ActionList)
            {
                EditorGUI.BeginChangeCheck();
                string newNamePrefix = EditorGUILayout.TextField("名字前缀", ActionList.namePrefix);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(ActionList, "修改名字前缀");
                    ActionList.namePrefix = newNamePrefix;
                    EditorUtility.SetDirty(ActionList);
                }
            }

            EditorGUIUtility.labelWidth = oldLabelWidth;

            // 控制栏
            EditorGUI.BeginDisabledGroup(ActionList == null || GO == null);
            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button(MyEditorGUIIcon.PlayButton, GUILayout.ExpandWidth(false)))
                {
                    if (_animator.runtimeAnimatorController == null)
                    {
                        _animator.runtimeAnimatorController = _rtAnimCtrl;
                    }

                    _actionCtrl.Refresh();
                    _actionCtrl.PlayActionFullName(Action.name);
                }
                if (GUILayout.Button("Animator", GUILayout.ExpandWidth(false)))
                {
                    if (_animator.runtimeAnimatorController == null)
                    {
                        _animator.runtimeAnimatorController = _rtAnimCtrl;
                    }
                }
                if (GUILayout.Button("Timeline", GUILayout.ExpandWidth(false)))
                {
                    _rtAnimCtrl = _animator.runtimeAnimatorController;
                    _animator.runtimeAnimatorController = null;
                }
                if (GUILayout.Button("导出Json", GUILayout.ExpandWidth(false)))
                {
                    Debug.Log(ActionList.ToJson());
                }
            }
            EditorGUI.EndDisabledGroup();

            searchText = searchField.OnGUI(searchText);

            using var s1 = new GUILayout.ScrollViewScope(scrollPos);
            scrollPos = s1.scrollPosition;

            if (ActionList == null || ActionList.actions == null) return;

            actions.Clear();
            if (!string.IsNullOrEmpty(searchText))
            {
                foreach (var action in ActionList.actions)
                {
                    long score = 0;
                    if (FuzzySearch.FuzzyMatch(searchText, action.name, ref score))
                    {
                        actions.Add((action, score));
                    }
                }
                actions.Sort((a, b) => b.Item2.CompareTo(a.Item2));
            }
            else
            {
                actions.AddRange(ActionList.actions.Select(a => (a, 0L)));
            }

            foreach (var (action, _) in actions)
            {
                var style = new GUIStyle(GUI.skin.button)
                {
                    alignment = TextAnchor.MiddleLeft,
                };

                using (new EditorGUILayout.HorizontalScope())
                {
                    MyGUIUtils.BeginHighlight(style, action == Action);
                    if (GUILayout.Button(action.name, style))
                    {
                        Action = action;
                    }
                    MyGUIUtils.EndHighlight();

                    if (GUILayout.Button("...", GUILayout.ExpandWidth(false)))
                    {
                        var menu = new GenericMenu();
                        menu.AddItem(new GUIContent("复制名"), false, () =>
                        {
                            string s = action.name;
                            if (s.StartsWith(ActionList.namePrefix))
                            {
                                s = s[ActionList.namePrefix.Length..];
                            }
                            GUIUtility.systemCopyBuffer = s;
                        });
                        menu.AddItem(new GUIContent("Project中高亮"), false, () =>
                        {
                            EditorGUIUtility.PingObject(action);
                        });
                        menu.ShowAsContext();
                    }
                }
            }
        }
    }
}