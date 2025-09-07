using System;
using System.Collections.Generic;
using Kirara.TimelineAction;
using UnityEngine;

namespace Kirara
{
    public class ActionCtrl : MonoBehaviour
    {
        public KiraraActionListSO actionList;
        private Dictionary<string, KiraraActionSO> ActionDict { get; set; }

        private KiraraActionSO _action;
        public KiraraActionSO OverrideAction { get; set; }

        public Func<KiraraActionSO, bool> IsActionExecutable { get; set; }
        public Action<KiraraActionSO, string> OnExecuteAction { get; set; }
        public Action<ActionParams> OnSetActionParams { get; set; }

        public float Time { get; private set; }
        public bool IsPlaying { get; private set; }
        public float Speed
        {
            get => Animator.speed;
            set => Animator.speed = value;
        }

        public bool EnableFinishTransition { get; set; } = true;

        private Animator Animator { get; set; }

        private void Awake()
        {
            Animator = GetComponent<Animator>();
            Refresh();
        }

        public void Refresh()
        {
            ActionDict = actionList.ActionDict;
        }

        public bool TryGetAction(string actionName, out KiraraActionSO action)
        {
            if (ActionDict.TryGetValue(actionName, out action)) return true;

            Debug.LogWarning($"{name} 没有动作 {actionName}");
            return false;
        }

        private bool IsActionExecutableInternal(string actionName)
        {
            if (!TryGetAction(actionName, out var action)) return false;

            if (IsActionExecutable == null) return true;

            return IsActionExecutable(action);
        }

        public void PlayActionFullName(string actionFullName, float fadeDuration = 0f, Action onFinish = null)
        {
            PlayAction(actionList.RemoveNamePrefix(actionFullName), fadeDuration, onFinish);
        }

        private void PlayActionInternal(KiraraActionSO action, string actionName, float fadeDuration = 0f, Action onFinish = null)
        {
            _action = action;
            OverrideAction = null;

            // 结束转移
            AddEndTransition(ref onFinish);

            Play(action, actionName, fadeDuration, onFinish);

            OnExecuteAction?.Invoke(action, actionName);
            OnSetActionParams?.Invoke(action.actionParams);
        }

        public void PlayAction(string actionName, float fadeDuration = 0f, Action onFinish = null)
        {
            if (!TryGetAction(actionName, out var action)) return;
            PlayActionInternal(action, actionName, fadeDuration, onFinish);
        }

        private void AddEndTransition(ref Action onFinish)
        {
            // 结束取消
            if (_action.isLoop) return;

            var finishTransition = _action.finishTransition;
            if (!string.IsNullOrEmpty(finishTransition.actionName))
            {
                onFinish += () =>
                {
                    PlayAction(finishTransition.actionName, finishTransition.fadeDuration);
                };
            }
            else
            {
                Debug.LogWarning($"{name} {_action.name} 没有结束取消");
            }
        }

        #region Input Transition

        public void InputCommand(EActionCommand command, EActionCommandPhase phase)
        {
            if (OverrideAction)
            {
                InputCommand(OverrideAction, command, phase);
            }
            else if (_action)
            {
                InputCommand(_action, command, phase);
            }
        }

        private void InputCommand(KiraraActionSO action, EActionCommand command, EActionCommandPhase phase)
        {
            var transition = GetExecutableCommandTransition(action, command, phase);
            if (transition != null)
            {
                // Debug.Log($"{name} 输入指令 {command} {phase} 跳转到 {transition.actionName}");
                PlayAction(transition.actionName, transition.fadeDuration);
                return;
            }

            // 检查指令跳转通知状态
            foreach (var state in _runningNotifyStates)
            {
                if (state is CommandTransitionNotifyState transitionState)
                {
                    var commandTransition = transitionState.commandTransition;
                    if (commandTransition.Check(command, phase) &&
                        IsActionExecutableInternal(commandTransition.actionName))
                    {
                        PlayAction(commandTransition.actionName, commandTransition.fadeDuration);
                        return;
                    }
                }
            }

            if (string.IsNullOrEmpty(action.inheritTransitionActionName)) return;
            var baseAction = ActionDict[action.inheritTransitionActionName];
            transition = GetExecutableCommandTransition(baseAction, command, phase);
            if (transition != null)
            {
                // Debug.Log($"{name} 输入指令 {command} {phase} 跳转到 {transition.actionName}");
                PlayAction(transition.actionName, transition.fadeDuration);
            }
        }

        private CommandTransitionInfo GetExecutableCommandTransition(
            KiraraActionSO action, EActionCommand command, EActionCommandPhase phase)
        {
            if (action.commandTransitions != null)
            {
                foreach (var transition in action.commandTransitions)
                {
                    if (transition.Check(command, phase) &&
                        IsActionExecutableInternal(transition.actionName))
                    {
                        return transition;
                    }
                }
            }
            return null;
        }

        public void InputSignal(string signalName)
        {
            if (OverrideAction)
            {
                InputSignal(OverrideAction, signalName);
            }
            else if (_action)
            {
                InputSignal(_action, signalName);
            }
        }

        private void InputSignal(KiraraActionSO action, string signalName)
        {
            var transition = GetExecutableSignalTransition(action, signalName);
            if (transition != null)
            {
                PlayAction(transition.actionName, transition.fadeDuration);
            }

            if (string.IsNullOrEmpty(_action.inheritTransitionActionName)) return;

            var baseAction = ActionDict[_action.inheritTransitionActionName];
            transition = GetExecutableSignalTransition(baseAction, signalName);
            if (transition != null)
            {
                PlayAction(transition.actionName, transition.fadeDuration);
            }
        }

        private SignalTransitionInfo GetExecutableSignalTransition(KiraraActionSO action, string signalName)
        {
            if (action.signalTransitions != null)
            {
                foreach (var signalTransition in action.signalTransitions)
                {
                    if (signalTransition.signalName == signalName &&
                        IsActionExecutableInternal(signalTransition.actionName))
                    {
                        return signalTransition;
                    }
                }
            }
            return null;
        }

        #endregion

        #region Action Player
        private AnimationClip _clip;

        // 所有运行的通知状态
        public readonly List<ActionNotifyState> _runningNotifyStates = new();

        // 所有的通知状态
        private readonly List<ActionNotifyState> _notifyStates = new();
        private int _notifyStatesFront;

        // 所有的通知
        private readonly List<ActionNotify> _notifies = new();
        private int _notifiesFront;

        private Action _onFinish;

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
            _notifyStates.Clear();
            _notifyStatesFront = 0;
        }

        private void ClearNotifies()
        {
            _notifies.Clear();
            _notifiesFront = 0;
        }

        private void EndAndClearRunningNotifyStates()
        {
            foreach (var state in _runningNotifyStates)
            {
                state.NotifyEnd(this);
            }
            _runningNotifyStates.Clear();
        }

        private bool playCalled = false;
        private void Play(KiraraActionSO action, string stateName, float fadeDuration = 0f, Action onFinish = null)
        {
            playCalled = true;

            // 切换的时候调用之前所有的end
            EndAndClearRunningNotifyStates();

            ClearNotifyStates();
            ClearNotifies();

            _action = action;
            ActionUnpacker.Unpack(action, out _clip, _notifyStates, _notifies);

            Time = 0f;
            IsPlaying = true;
            _onFinish = onFinish;

            Animator.CrossFadeInFixedTime(stateName, fadeDuration);
        }

        private void ProcessNotifies()
        {
            playCalled = false;
            // 处理 Notify State Begin
            while (_notifyStatesFront < _notifyStates.Count && _notifyStates[_notifyStatesFront].start <= Time)
            {
                var state = _notifyStates[_notifyStatesFront];
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
                        if (EnableFinishTransition)
                        {
                            var finishTransition = _action.finishTransition;
                            if (!string.IsNullOrEmpty(finishTransition.actionName))
                            {
                                PlayAction(finishTransition.actionName, finishTransition.fadeDuration);
                            }
                            else
                            {
                                Stop();
                            }
                        }
                        else
                        {
                            Stop();
                        }
                    }
                }

                foreach (var state in _runningNotifyStates)
                {
                    state.NotifyTick(this, Time);
                }
            }
        }

        #endregion
    }
}