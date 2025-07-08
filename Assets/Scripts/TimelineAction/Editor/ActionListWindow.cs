using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

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

        private Animator animator;
        private ActionCtrl1 actionCtrl;
        private PlayableDirector director;
        private RuntimeAnimatorController rtAnimCtrl;

        private string searchQuery;
        private List<(KiraraActionSO, long)> actions;

        private Vector2 scrollPos;

        [MenuItem("Kirara/动作列表")]
        public static void GetWindow()
        {
            var window = GetWindow<ActionListWindow>("动作列表");
            var detailsWindow = ActionDetailsWindow.GetWindow();
        }

        private void OnEnable()
        {
            actions ??= new List<(KiraraActionSO, long)>();
            if (Instance != null)
            {
                Debug.LogWarning("ActionListWindow already exists!");
            }
            Instance = this;
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

            var window = TimelineWindowHelper.GetWindow();
            if (director)
            {
                director.playableAsset = _action;

                foreach (var track in _action.GetRootTracks())
                {
                    if (track is AnimationTrack)
                    {
                        director.SetGenericBinding(track, animator);
                        break;
                    }
                }
                window.SetTimeline(director);
            }
            else
            {
                window.SetTimeline(_action);
            }

            window.Repaint();
        }

        private void UpdateGO()
        {
            GO = null;
            var actionCtrls = FindObjectsByType<ActionCtrl1>(FindObjectsSortMode.None);
            foreach (var ctrl in actionCtrls)
            {
                if (ctrl.actionList == ActionList)
                {
                    GO = ctrl.gameObject;
                    break;
                }
            }
        }

        private void SetGO(GameObject go)
        {
            if (go == _go) return;

            _go = go;
            if (_go)
            {
                actionCtrl = _go.GetComponent<ActionCtrl1>();
                animator = _go.GetComponent<Animator>();
                director = _go.GetComponent<PlayableDirector>();
                SetActionList(actionCtrl ? actionCtrl.actionList : null, false);
            }
            else
            {
                animator = null;
                actionCtrl = null;
                director = null;
                SetActionList(null, false);
            }
        }

        private void OnGUI()
        {
            const float LabelWidth = 50f;
            EditorGUIUtility.labelWidth = LabelWidth;

            // 动作列表
            EditorGUI.BeginChangeCheck();
            ActionList = (KiraraActionListSO)EditorGUILayout.ObjectField(
                "动作列表", ActionList, typeof(KiraraActionListSO), false);

            // GameObject
            GO = (GameObject)EditorGUILayout.ObjectField(
                "GameObject", GO, typeof(GameObject), true);

            // 名字前缀
            if (ActionList)
            {
                EditorGUI.BeginChangeCheck();
                ActionList.namePrefix = EditorGUILayout.TextField("名字前缀", ActionList.namePrefix);
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(ActionList);
                }
            }

            // 控制栏
            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button(MyEditorGUIIcon.PlayButton, GUILayout.ExpandWidth(false)))
                {
                    if (animator.runtimeAnimatorController == null)
                    {
                        animator.runtimeAnimatorController = rtAnimCtrl;
                    }

                    actionCtrl.Refresh();
                    actionCtrl.PlayActionFullName(Action.name);
                }
                if (GUILayout.Button("Animator", GUILayout.ExpandWidth(false)))
                {
                    if (animator.runtimeAnimatorController == null)
                    {
                        animator.runtimeAnimatorController = rtAnimCtrl;
                    }
                }
                if (GUILayout.Button("Timeline", GUILayout.ExpandWidth(false)))
                {
                    rtAnimCtrl = animator.runtimeAnimatorController;
                    animator.runtimeAnimatorController = null;
                }
                if (GUILayout.Button("导出Json", GUILayout.ExpandWidth(false)))
                {
                    Debug.Log(ActionList.ToJson());
                }
            }

            searchQuery = MyEditorGUILayout.ToolbarSearchField(searchQuery);

            using var s1 = new GUILayout.ScrollViewScope(scrollPos);
            scrollPos = s1.scrollPosition;

            if (ActionList == null || ActionList.actions == null) return;

            actions.Clear();
            if (!string.IsNullOrEmpty(searchQuery))
            {
                foreach (var action in ActionList.actions)
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