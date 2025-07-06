using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ActionEditor.Editor;
using UnityEditor;
using UnityEngine;

namespace Kirara.ActionEditor
{
    public class ActionTimelineWindow : EditorWindow
    {
        public ActionSO Action { get; set; }
        public ActionTrack Track { get; set; }
        public Color ClipColor { get; set; } = Color.green;
        public static float FrameRate = 60f;

        private float trackHeight = 20f;
        private float frameWidth = 10f;
        private float viewMinTime = 0f;
        private float ViewMinFrame => viewMinTime * frameRate;

        private int viewMinTrackIdx = 0;

        private float frameRate = 60f;
        private int framePerScroll = 2;

        private float trackSpacing = 1f;

        private GUINotify draggingNotify;
        private Vector2 mousePosInDraggingRect;

        private MyGridLayout layout;

        private Rect ControlBarRect => layout.Rect00;
        private Rect ScaleRect => layout.Rect01;
        private Rect TrackHeadRect => layout.Rect10;
        private Rect TracksRect => layout.Rect11;

        private static readonly Type[] actionNotifyTypes = GetSubclassTypes(typeof(ActionNotifySO));
        private static readonly Type[] actionNotifyStateTypes = GetSubclassTypes(typeof(ActionNotifyStateSO));

        private void OnEnable()
        {
            layout = new MyGridLayout(this, 150f, 20f, 1f, 1f);
        }

        public static ActionTimelineWindow GetWindow()
        {
            return GetWindow<ActionTimelineWindow>("动作时间轴");
        }

        private static Type[] GetSubclassTypes(Type baseType)
        {
            return Assembly.GetAssembly(baseType)
                .GetTypes()
                .Where(t => t.IsSubclassOf(baseType))
                .ToArray();
        }

        private void DrawControlBar()
        {
            using var scope = new GUI.GroupScope(ControlBarRect);
            using (new EditorGUI.DisabledScope(Action == null))
            {
                float width = 20f;
                var rect = new Rect(0, 0, width, ControlBarRect.height);
                if (GUI.Button(rect, "+", EditorStyles.toolbarButton))
                {
                    Action!.AddTrack();
                }
                rect.x += width;
                if (GUI.Button(rect, "<", EditorStyles.toolbarButton))
                {
                }
                rect.x += width;
                if (GUI.Button(rect, EditorGUIUtility.IconContent("PlayButton"), EditorStyles.toolbarButton))
                {
                    // time = 0.5f;
                    // var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
                    // var go = prefabStage.prefabContentsRoot;
                    // AnimationMode.StartAnimationMode();
                    // AnimationMode.BeginSampling();
                    // AnimationMode.SampleAnimationClip(go, Action.clip, time);
                    // AnimationMode.EndSampling();
                    // AnimationMode.StopAnimationMode();
                }
                rect.x += width;
                if (GUI.Button(rect, ">", EditorStyles.toolbarButton))
                {
                }
                rect.x += width;

                // for (int i = 1; i <= 4; i++)
                // {
                //     var lineRect = new Rect(i * width - 1, 0, 1, ControlBarRect.height);
                //     EditorGUI.DrawRect(lineRect, Color.black);
                // }

                // EditorGUI.BeginChangeCheck();
                // timeFrameInput = GUILayout.TextField(timeFrameInput);
                // if (EditorGUI.EndChangeCheck())
                // {
                //     if (float.TryParse(timeFrameInput, out float timeFrame))
                //     {
                //         time = timeFrame / FrameRate;
                //     }
                // }
            }
        }

        private static int NormalizeInt(float value)
        {
            if (value > 0) return 1;
            if (value == 0) return 0;
            return -1;
        }

        private void ScrollH(float scroll)
        {
            if (Action == null || Action.tracks.Count == 0) return;
            int scrollFrame = NormalizeInt(scroll) * framePerScroll;
            float viewMinFrame = viewMinTime * frameRate;
            viewMinTime = Mathf.Max(Mathf.Round(viewMinFrame + scrollFrame) / frameRate, 0f);
        }

        private void ScrollV(float scroll)
        {
            if (Action == null || Action.tracks.Count == 0) return;
            viewMinTrackIdx = Mathf.Clamp(viewMinTrackIdx + NormalizeInt(scroll), 0, Action.tracks.Count - 1);
        }

        private void HandleEvent()
        {
            var e = Event.current;

            switch (e.type)
            {
                case EventType.ScrollWheel:
                {
                    if (TracksRect.Contains(e.mousePosition) || ScaleRect.Contains(e.mousePosition))
                    {
                        e.Use();
                        float scroll = e.delta.y; // 向上为负，向下为正
                        ScrollH(scroll);

                        UpdateDragging(e.mousePosition);
                    }
                    else if (TrackHeadRect.Contains(e.mousePosition))
                    {
                        e.Use();
                        float scroll = e.delta.y;
                        ScrollV(scroll);
                    }

                    break;
                }
                case EventType.MouseDown:
                {
                    if (e.button == 0)
                    {
                        // 拖动轨道
                        // for (int i = 0; i < guiTrackClips.Count; i++)
                        // {
                        //     if (guiTrackClips[i].left.Contains(e.mousePosition))
                        //     {
                        //         e.Use();
                        //     }
                        //     else if (guiTrackClips[i].right.Contains(e.mousePosition))
                        //     {
                        //         e.Use();
                        //     }
                        //     else if (guiTrackClips[i].main.Contains(e.mousePosition))
                        //     {
                        //         e.Use();
                        //         draggingNotify = Action.tracks[i];
                        //         // mousePosInDraggingRect = e.mousePosition - guiTrackClips[i].position;
                        //         break;
                        //     }
                        // }
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
                        if (draggingNotify != null)
                        {
                            draggingNotify = null;
                            e.Use();
                        }
                    }
                    break;
                }
                case EventType.MouseDrag:
                {
                    if (draggingNotify != null)
                    {
                        e.Use();
                        UpdateDragging(e.mousePosition);
                    }
                    break;
                }
            }
        }

        private void Update()
        {
            // if (draggingTrack != null)
            // {
            // }
        }

        private void UpdateDragging(Vector2 mousePos)
        {
            if (draggingNotify == null) return;

            float offset = (mousePos - mousePosInDraggingRect - TracksRect.position).x;
            float roundFrame = Mathf.Round(offset / frameWidth + ViewMinFrame);
            // draggingNotify.start = Mathf.Max(roundFrame / frameRate, 0);
        }

        private void OnGUI()
        {
            HandleEvent();
            Draw();
        }

        private void Draw()
        {
            DrawControlBar();
            DrawScale();
            DrawTrackHead();
            DrawTracks();
            DrawGrid();
        }

        private void DrawGrid()
        {
            EditorGUI.DrawRect(layout.RectHSpacing, Color.black);
            EditorGUI.DrawRect(layout.RectVSpacing, Color.black);
        }

        private Vector2 scrollPos;

        private void DrawScale()
        {
            using var scope = new GUI.GroupScope(ScaleRect);

            float viewMinFrame = viewMinTime * frameRate;
            int frameNum = Mathf.FloorToInt(viewMinFrame);
            int drawNumPerFrame = 2;
            var rect = new Rect((frameNum - viewMinFrame) * frameWidth, 0, frameWidth * drawNumPerFrame, ScaleRect.height);
            int i = 0;
            const int max = 10000;
            while (i < 10000 && rect.x < ScaleRect.width)
            {
                if (frameNum % drawNumPerFrame == 0)
                {
                    float lineHeight = ScaleRect.height * 0.6f;
                    var scaleLineRect = new Rect(rect.x, rect.height - lineHeight, 1f, lineHeight);
                    EditorGUI.DrawRect(scaleLineRect, Color.black);
                    var style = new GUIStyle(GUI.skin.label)
                    {
                        fontSize = 9
                    };
                    GUI.Label(rect, frameNum.ToString(), style);
                }
                else
                {
                    float lineHeight = ScaleRect.height * 0.2f;
                    var scaleLineRect = new Rect(rect.x, rect.height - lineHeight, 1f, lineHeight);
                    EditorGUI.DrawRect(scaleLineRect, Color.gray);
                }
                rect.x += frameWidth;

                frameNum++;
                i++;
            }
            if (i == max)
            {
                Debug.LogWarning($"should not go here");
            }
        }

        private void DrawTrackHead()
        {
            using var scope = new GUI.GroupScope(TrackHeadRect);

            if (Action == null) return;

            var vLayout = new MyVLayout(TrackHeadRect.width, TrackHeadRect.height, trackHeight, trackSpacing);
            int i = 0;
            for (int idx = viewMinTrackIdx; idx < Action.tracks.Count; idx++, i++)
            {
                var track = Action.tracks[idx];

                if (!vLayout.TryRect(i, out var rect)) break;

                var style = new GUIStyle(EditorStyles.toolbarButton)
                {
                    alignment = TextAnchor.MiddleLeft,
                };
                MyGUIUtils.BeginHighlight(style, track == Track);
                if (GUI.Button(rect, track.name, style))
                {
                    Track = track;
                }
                MyGUIUtils.EndHighlight();
                EditorGUI.DrawRect(vLayout.Spacing(i), Color.gray);
            }
        }

        private void DrawTracks()
        {
            using var scope = new GUI.GroupScope(TracksRect);

            if (Action == null) return;

            // int i = 0;
            // for (int idx = viewMinTrackIdx; idx < Action.tracks.Count; idx++, i++)
            // {
            //     var track = Action.tracks[idx];
            //
            //     float clipStart = TimeToPos(track.start - viewMinTime);
            //     float clipWidth = TimeToPos(track.duration);
            //
            //     var clip = new GUITrackClip();
            //
            //     clip.main = new Rect(clipStart, TrackY(i), clipWidth, trackHeight);
            //     if (clip.main.y > TracksRect.height)
            //     {
            //         break;
            //     }
            //
            //     clip.left = clip.main.Width(4).CenterX(clip.main.x);
            //     clip.right = clip.main.Width(4).CenterX(clip.main.xMax);
            //
            //
            //     EditorGUIUtility.AddCursorRect(clip.left, MouseCursor.ResizeHorizontal);
            //     EditorGUIUtility.AddCursorRect(clip.right, MouseCursor.ResizeHorizontal);
            //
            //
            //     EditorGUI.DrawRect(clip.main, ClipColor);
            //     EditorGUI.DrawRect(new Rect(0, TrackSpacingY(i + 1), TracksRect.width, 1), Color.gray);
            //
            //     clip.main = GetAbsRect(clip.main, TracksRect);
            //     clip.left = GetAbsRect(clip.left, TracksRect);
            //     clip.right = GetAbsRect(clip.right, TracksRect);
            // }
        }

        private static Rect GetAbsRect(Rect rect, Rect parent)
        {
            return new Rect(rect.x + parent.x, rect.y + parent.y, rect.width, rect.height);
        }

        private float TimeToPos(float time)
        {
            return time * frameRate * frameWidth;
        }

        private float TrackY(int index)
        {
            return index * (trackHeight + trackSpacing);
        }

        private float TrackSpacingY(int index)
        {
            return index * (trackHeight + trackSpacing) - trackSpacing;
        }
    }
}