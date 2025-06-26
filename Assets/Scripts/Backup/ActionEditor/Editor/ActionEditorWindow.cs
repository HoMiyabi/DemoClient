/*using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Kirara.ActionEditor
{
    public class ActionEditorWindow : EditorWindow
    {
        private const string STitle = "Kirara 动作编辑器";
        private const string SPrefab = "预制体";
        private const string SActionList = "动作列表";
        private const string SActionDetails = "动作细节面板";
        private const string STrackDetails = "轨道细节面板";

        private const float LabelWidth = 50;

        [MenuItem("Kirara/动作编辑器")]
        private static void GetWindow()
        {
            GetWindow<ActionEditorWindow>(STitle);
        }

        private UnityEditor.Editor actionSOEditor;
        private UnityEditor.Editor trackSOEditor;

        private ActionListSO _actionList;
        private ActionListSO ActionList
        {
            get => _actionList;
            set
            {
                if (_actionList == value) return;

                _actionList = value;
                Action = _actionList != null ? MyFirstOrDefault(_actionList.actions) : null;
            }
        }

        private ActionSO _action;
        private ActionSO Action
        {
            get => _action;
            set
            {
                if (_action == value) return;

                _action = value;
                UpdateEditor(ref actionSOEditor, _action);
                Track = _action != null ? MyFirstOrDefault(_action.tracks) : null;
            }
        }

        private ActionTrackSO _track;
        private ActionTrackSO Track
        {
            get => _track;
            set
            {
                if (_track == value) return;

                _track = value;
                UpdateEditor(ref trackSOEditor, _track);
            }
        }

        private List<Rect> trackClipWinRect = new();
        private ActionTrackSO draggingTrack;
        private Vector2 mousePosInDraggingRect;

        private string timeFrameInput;
        private int minFrame;

        private int scrollSpeed = 2;

        // 网格布局
        private float width1 = 200;
        private float height1 = 300;

        public const int FrameRate = 60;

        private float frameWidth = 20;
        private const float TickHeight = 20;

        private GameObject prefab;

        private static T MyFirstOrDefault<T>(List<T> list)
        {
            if (list != null && list.Count > 0)
            {
                return list[0];
            }
            return default;
        }

        // 选择动作区域
        private void DrawSelectActionArea()
        {
            using var s1 = new GUILayout.VerticalScope(GUILayout.Width(width1));

            ActionList = (ActionListSO)EditorGUILayout.ObjectField(
                SActionList, ActionList, typeof(ActionListSO), false);

            prefab = (GameObject)EditorGUILayout.ObjectField(
                SPrefab, prefab, typeof(GameObject), false);

            if (GUILayout.Button("打开Prefab Scene"))
            {
                string path = AssetDatabase.GetAssetPath(prefab);
                PrefabStageUtility.OpenPrefab(path);
            }

            if (ActionList == null || ActionList.actions == null) return;

            foreach (var action in ActionList.actions)
            {
                var style = new GUIStyle(GUI.skin.button)
                {
                    alignment = TextAnchor.MiddleLeft,
                };
                var color = GUI.backgroundColor;
                if (action == Action)
                {
                    GUI.backgroundColor = Color.cyan + new Color(0,0, 0.2f);
                    style.fontStyle = FontStyle.Bold;
                }
                if (GUILayout.Button(action.name, style))
                {
                    Action = action;
                }
                GUI.backgroundColor = color;
            }
        }

        private void DrawDetails()
        {
            using var s1 = new GUILayout.HorizontalScope();
            DrawActionSODetails();
            DrawTrackSODetails();
        }

        // 动作细节面板区域
        private void DrawActionSODetails()
        {
            using var s1 = new GUILayout.VerticalScope();

            var style = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
            };
            GUILayout.Label(SActionDetails, style);

            if (actionSOEditor != null)
            {
                actionSOEditor.OnInspectorGUI();
            }
        }

        // 轨道细节面板区域
        private void DrawTrackSODetails()
        {
            using var s1 = new GUILayout.VerticalScope();

            var style = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
            };
            GUILayout.Label(STrackDetails, style);

            if (trackSOEditor != null)
            {
                trackSOEditor.OnInspectorGUI();
            }
        }

        private void RemoveTrack(ActionTrackSO track)
        {
            if (Track == track)
            {
                int idx = Action.tracks.IndexOf(track);
                if (idx + 1 < Action.tracks.Count)
                {
                    Track = Action.tracks[idx + 1];
                }
                else if (idx - 1 >= 0)
                {
                    Track = Action.tracks[idx - 1];
                }
                else
                {
                    Track = null;
                }
            }
            Action?.RemoveTrack(track);
        }

        private void AddTrack()
        {
            if (Action != null)
            {
                var track = Action.AddTrack(typeof(ActionTrackSO));
                Track = track;
            }
        }

        // 轨道头部区域
        private void DrawTrackHeaderArea()
        {
            using var s1 = new GUILayout.VerticalScope(GUILayout.Width(width1));

            // 控制按钮
            DrawControlBar();

            if (Action == null || Action.tracks == null) return;

            for (int i = 0; i < Action.tracks.Count; i++)
            {
                var track = Action.tracks[i];
                using var s2 = new GUILayout.HorizontalScope();
                var style = new GUIStyle(GUI.skin.button)
                {
                    alignment = TextAnchor.MiddleLeft,
                };
                var color = GUI.backgroundColor;
                if (track == Track)
                {
                    GUI.backgroundColor = Color.cyan + new Color(0, 0, 0.2f);
                    style.fontStyle = FontStyle.Bold;
                }
                if (GUILayout.Button(track.name, style))
                {
                    Track = track;
                }
                GUI.backgroundColor = color;

                if (GUILayout.Button("...", GUILayout.ExpandWidth(false)))
                {
                    var menu = new GenericMenu();
                    menu.AddItem(new GUIContent("删除"), false, () =>
                    {
                        RemoveTrack(track);
                    });
                    menu.ShowAsContext();
                }
            }
        }

        private void Update()
        {

        }

        private void DrawControlBar()
        {
            using var _ = new GUILayout.HorizontalScope();
            if (GUILayout.Button("+", GUILayout.ExpandWidth(false)))
            {
                AddTrack();
            }
            if (GUILayout.Button("<", GUILayout.ExpandWidth(false)))
            {
            }
            if (GUILayout.Button("||", GUILayout.ExpandWidth(false)))
            {
                time = 0.5f;
                var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
                var go = prefabStage.prefabContentsRoot;
                // AnimationMode.StartAnimationMode();
                // AnimationMode.BeginSampling();
                // AnimationMode.SampleAnimationClip(go, Action.clip, time);
                // AnimationMode.EndSampling();
                // AnimationMode.StopAnimationMode();
            }
            if (GUILayout.Button(">", GUILayout.ExpandWidth(false)))
            {
            }
            EditorGUI.BeginChangeCheck();
            timeFrameInput = GUILayout.TextField(timeFrameInput);
            if (EditorGUI.EndChangeCheck())
            {
                if (float.TryParse(timeFrameInput, out float timeFrame))
                {
                    time = timeFrame / FrameRate;
                }
            }
        }

        private static Rect GetAbsRect(Rect rect, Rect parent)
        {
            return new Rect(rect.x + parent.x, rect.y + parent.y, rect.width, rect.height);
        }

        private float trackHeight = 18;
        private float trackPadding = 3;
        private float trackSpacing = 4;

        private float time;

        private Rect tickTracksRect;

        // 绘制时间刻度和轨道
        private void DrawTickTracks()
        {
            tickTracksRect = GUILayoutUtility.GetRect(100, 1000);
            using var g1 = new GUI.GroupScope(tickTracksRect);

            using (var g2 = new GUI.GroupScope(new Rect(0, 0, tickTracksRect.width, TickHeight)))
            {
                int i = 0;
                int frameIdx = minFrame;
                int maxLabelCount = 1000;
                while (i < maxLabelCount)
                {
                    var labelRect = new Rect(i * frameWidth, 0, frameWidth, TickHeight);
                    var labelPosSS = GUIUtility.GUIToScreenPoint(labelRect.position);
                    if (!position.Contains(labelPosSS))
                    {
                        break;
                    }
                    var sepLineRect = new Rect(labelRect.x, labelRect.y, 1, TickHeight);
                    EditorGUI.DrawRect(sepLineRect, Color.black);
                    var style = new GUIStyle(GUI.skin.label)
                    {
                        alignment = TextAnchor.MiddleCenter,
                    };
                    GUI.Label(labelRect, frameIdx.ToString(), style);
                    i++;
                    frameIdx++;
                }
                if (i == maxLabelCount)
                {
                    Debug.LogWarning($"should not go here");
                }
            }

            using (var g3 = new GUI.GroupScope(new Rect(0, TickHeight + 4, tickTracksRect.width, 1000)))
            {
                trackClipWinRect.Clear();
                if (Action != null && Action.tracks != null)
                {
                    for (int i = 0; i < Action.tracks.Count; i++)
                    {
                        var track = Action.tracks[i];
                        float clipStart = FrameToPos(track.start * FrameRate - minFrame);
                        float clipWidth = FrameToPos(track.duration * FrameRate);
                        var clipRect = new Rect(clipStart, i * (trackHeight + trackSpacing) + trackPadding,
                            clipWidth, trackHeight);
                        trackClipWinRect.Add(GetAbsRect(clipRect, tickTracksRect));
                        EditorGUI.DrawRect(clipRect, track.ClipColor);
                    }
                }
            }

            // 时间线
            var color = Color.yellow;
            var headColor = color;
            headColor.a = 0.8f;
            var bodyColor = color;
            EditorGUI.DrawRect(new Rect(time * FrameRate * frameWidth - 5, 0, 10, 20), headColor);
            EditorGUI.DrawRect(new Rect(time * FrameRate * frameWidth, 20, 1, 100), bodyColor);
        }

        private float FrameToPos(float frame)
        {
            return frame * 20f;
        }

        private void UpdateEditor(ref UnityEditor.Editor editor, UnityEngine.Object target)
        {
            if (editor != null)
            {
                DestroyImmediate(editor);
                editor = null;
            }
            if (target != null)
            {
                editor = UnityEditor.Editor.CreateEditor(target);
            }
            Repaint();
        }

        private void OnDestroy()
        {
            if (actionSOEditor != null)
            {
                DestroyImmediate(actionSOEditor);
                actionSOEditor = null;
            }
        }

        private void Draw()
        {
            EditorGUIUtility.labelWidth = LabelWidth;
            using (var s1 = new GUILayout.VerticalScope())
            {
                using (var s2 = new GUILayout.HorizontalScope(GUILayout.Height(height1)))
                {
                    DrawSelectActionArea();
                    DrawDetails();
                }
                using (var s3 = new GUILayout.HorizontalScope())
                {
                    DrawTrackHeaderArea();
                    DrawTickTracks();
                }
            }
        }

        private void HandleEvent()
        {
            var e = Event.current;

            switch (e.type)
            {
                case EventType.ScrollWheel:
                {
                    e.Use();
                    float scroll = e.delta.y; // 向上为负，向下为正
                    minFrame = Mathf.Clamp(minFrame + NormalizeInt(scroll) * scrollSpeed, 0, 100);

                    if (draggingTrack != null)
                    {
                        UpdateDragging(e.mousePosition);
                    }

                    break;
                }
                case EventType.MouseDown:
                {
                    if (e.button == 0)
                    {
                        // 拖动轨道
                        for (int i = 0; i < trackClipWinRect.Count; i++)
                        {
                            if (trackClipWinRect[i].Contains(e.mousePosition))
                            {
                                draggingTrack = Action.tracks[i];
                                mousePosInDraggingRect = e.mousePosition - trackClipWinRect[i].position;
                                e.Use();
                                break;
                            }
                        }
                        // 点击刻度
                        // if (tracksRect.Contains(e.mousePosition))
                        // {
                        //     int frame = NormalizeInt((e.mousePosition - tracksRect.position).x / frameWidth);
                        //     minFrame = Mathf.Clamp(frame, 0, 100);
                        //     e.Use();
                        // }
                    }
                    break;
                }
                case EventType.MouseUp:
                {
                    if (e.button == 0)
                    {
                        if (draggingTrack != null)
                        {
                            draggingTrack = null;
                            e.Use();
                        }
                    }
                    break;
                }
                case EventType.MouseDrag:
                {
                    if (draggingTrack != null)
                    {
                        UpdateDragging(e.mousePosition);
                        e.Use();
                    }
                    break;
                }
            }
        }

        private void UpdateDragging(Vector2 mousePosition)
        {
            float offset = (mousePosition - mousePosInDraggingRect - tickTracksRect.position).x;
            float roundFrame = Mathf.Round(offset / frameWidth + minFrame);
            draggingTrack.start = Mathf.Max(roundFrame / FrameRate, 0);
            EditorUtility.SetDirty(draggingTrack);
        }

        private static int NormalizeInt(float value)
        {
            if (value > 0) return 1;
            if (value == 0) return 0;
            return -1;
        }

        private void OnGUI()
        {
            Draw();
            HandleEvent();
        }
    }
}*/