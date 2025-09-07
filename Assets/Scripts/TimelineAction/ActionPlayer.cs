using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

namespace Kirara.TimelineAction
{
    public class ActionPlayer : MonoBehaviour
    {
        public float Time { get; private set; }
        public bool IsPlaying { get; private set; }
        public float Speed
        {
            get => _animator.speed;
            set => _animator.speed = value;
        }

        private Animator _animator;
        private AnimationClip _clip;
        private KiraraActionSO _action;

        // 所有运行的通知状态
        public readonly List<ActionNotifyState> _runningNotifyStates = new();

        // 所有的通知状态
        public List<ActionNotifyState> NotifyStates { get; } = new();
        private int _notifyStatesFront;

        // 所有的通知
        private readonly List<ActionNotify> _notifies = new();
        private int _notifiesFront;

        private Action _onFinish;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void Stop()
        {
            Debug.Log($"{name} Action Player Stop");
            _clip = null;
            Time = 0f;
            IsPlaying = false;
            _runningNotifyStates.Clear();
            ClearNotifyStates();
            ClearNotifies();
            // _animator.enabled = false;
        }

        private void ClearNotifyStates()
        {
            NotifyStates.Clear();
            _notifyStatesFront = 0;
        }

        private void ClearNotifies()
        {
            _notifies.Clear();
            _notifiesFront = 0;
        }

        private bool playCalled = false;
        public void Play(KiraraActionSO action, string stateName, float fadeDuration = 0f, Action onFinish = null)
        {
            playCalled = true;

            // 切换的时候调用之前所有的end
            foreach (var state in _runningNotifyStates)
            {
                state.NotifyEnd(this);
            }
            _runningNotifyStates.Clear();

            ClearNotifyStates();
            ClearNotifies();

            _action = action;
            UnpackAction(action, out _clip, NotifyStates, _notifies);

            Time = 0f;
            IsPlaying = true;
            _onFinish = onFinish;

            _animator.CrossFadeInFixedTime(stateName, fadeDuration);
        }

        private void ProcessNotifies()
        {
            playCalled = false;
            // 处理 Notify State Begin
            while (_notifyStatesFront < NotifyStates.Count && NotifyStates[_notifyStatesFront].start <= Time)
            {
                var state = NotifyStates[_notifyStatesFront];
                _runningNotifyStates.Add(state);
                state.NotifyBegin(this);
                if (playCalled)
                {
                    return;
                }
                _notifyStatesFront++;
            }

            // 处理 Notify
            while (_notifiesFront < _notifies.Count && _notifies[_notifiesFront].time <= Time)
            {
                _notifies[_notifiesFront].Notify(this);
                if (playCalled)
                {
                    return;
                }
                _notifiesFront++;
            }

            // 处理 Notify State End
            for (int i = 0; i < _runningNotifyStates.Count;)
            {
                var state = _runningNotifyStates[i];
                if (state.start + state.length <= Time)
                {
                    _runningNotifyStates.RemoveAt(i);
                    state.NotifyEnd(this);
                    if (playCalled)
                    {
                        return;
                    }
                }
                else
                {
                    i++;
                }
            }
        }

        private void Update()
        {
            if (IsPlaying)
            {
                Time += UnityEngine.Time.deltaTime * Speed;

                ProcessNotifies();
                if (playCalled)
                {
                    return;
                }

                if (Time > _clip.length)
                {
                    if (_action.isLoop)
                    {
                        if (_runningNotifyStates.Count > 0)
                        {
                            Debug.LogWarning($"循环动作到结尾 但还在通知状态中");
                            foreach (var state in _runningNotifyStates)
                            {
                                state.NotifyEnd(this);
                            }
                        }
                        _runningNotifyStates.Clear();

                        Time -= _clip.length;
                        _notifyStatesFront = 0;
                        _notifiesFront = 0;

                        ProcessNotifies();
                        if (playCalled)
                        {
                            return;
                        }
                    }
                    else
                    {
                        playCalled = false;
                        _onFinish?.Invoke();
                        if (playCalled)
                        {
                            return;
                        }
                        Stop();
                    }
                }

                foreach (var state in _runningNotifyStates)
                {
                    state.NotifyTick(this, Time);
                }
            }
        }

        private static void MarkersToNotifies(IEnumerable<IMarker> markers, List<ActionNotify> notifies)
        {
            foreach (var marker in markers)
            {
                if (marker is ActionNotify notify)
                {
                    notifies.Add(notify);
                }
                else
                {
                    Debug.LogWarning($"仅支持 ActionNotifyMarker {marker.GetType()}");
                }
            }
        }

        private static void TimelineClipsToNotifyStates(
            IEnumerable<TimelineClip> clips, List<ActionNotifyState> notifyStates)
        {
            foreach (var clip in clips)
            {
                if (clip.asset is ActionNotifyState notifyState)
                {
                    notifyState.start = (float)clip.start;
                    notifyState.length = (float)clip.duration;
                    notifyStates.Add(notifyState);
                }
                else
                {
                    Debug.LogWarning($"仅支持ActionNotifyState {clip.asset.GetType()}");
                }
            }
        }

        private static void UnpackAction(
            KiraraActionSO action,
            out AnimationClip animClip,
            List<ActionNotifyState> notifyStates,
            List<ActionNotify> notifies)
        {
            animClip = null;

            foreach (var track in action.GetOutputTracks())
            {
                MarkersToNotifies(track.GetMarkers(), notifies);
                if (track is AnimationTrack animTrack)
                {
                    foreach (var clip in animTrack.GetClips())
                    {
                        if (animClip == null)
                        {
                            animClip = clip.animationClip;
                            if (clip.start != 0.0)
                            {
                                Debug.LogWarning($"仅支持从0时刻开始播放 {clip.displayName}");
                            }
                        }
                        else
                        {
                            Debug.LogWarning($"仅支持一个动画片段 {clip.displayName}");
                        }
                    }
                }
                else
                {
                    TimelineClipsToNotifyStates(track.GetClips(), notifyStates);
                }
            }

            notifyStates.Sort((l, r) => l.start.CompareTo(r.start));
            notifies.Sort((l, r) => l.time.CompareTo(r.time));
        }
    }
}