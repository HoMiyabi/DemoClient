using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kirara.TimelineAction
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

        public Deque<InputBufferItem> InputBuffer { get; private set; } = new(128);

        public struct InputBufferItem
        {
            public EActionCommand command;
            public EActionCommandPhase phase;
            public double time;
            public bool used;
        }

        private Animator Animator { get; set; }

        private bool playCalled = false;

        // 所有的通知
        private readonly List<ActionNotify> _notifies = new();
        private int _notifiesFront;

        // 所有的通知状态
        private readonly List<ActionNotifyState> _notifyStates = new();
        private int _notifyStatesFront;

        // 所有运行的通知状态
        private readonly List<ActionNotifyState> _runningNotifyStates = new();

        private Action _onFinish;

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
            if (actionName != null && ActionDict.TryGetValue(actionName, out action)) return true;

            action = null;
            Debug.LogWarning($"{name}没有动作: {actionName}");
            return false;
        }

        public bool IsActionExecutableInternal(string actionName)
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
            playCalled = true;
            _action = action;
            OverrideAction = null;

            ClearNotifies();
            ClearNotifyStates();
            // 切换的时候调用之前所有的end
            EndAndClearRunningNotifyStates();

            ActionUnpacker.Unpack(action, out _clip, _notifyStates, _notifies);

            Time = 0f;
            IsPlaying = true;
            _onFinish = onFinish;

            OnExecuteAction?.Invoke(action, actionName);
            OnSetActionParams?.Invoke(action.actionParams);

            Animator.CrossFadeInFixedTime(actionName, fadeDuration);
        }

        public void PlayAction(string actionName, float fadeDuration = 0f, Action onFinish = null)
        {
            if (!TryGetAction(actionName, out var action)) return;
            PlayActionInternal(action, actionName, fadeDuration, onFinish);
        }

        #region Input Transition

        public void InputCommand(EActionCommand command, EActionCommandPhase phase)
        {
            bool ok = false;
            if (OverrideAction)
            {
                ok = InputCommand(OverrideAction, command, phase);
            }
            else if (_action)
            {
                ok = InputCommand(_action, command, phase);
            }

            double time = UnityEngine.Time.timeAsDouble;
            const double dt = 1;
            if (!ok)
            {
                InputBuffer.PushBack(new InputBufferItem
                {
                    command = command,
                    phase = phase,
                    time = time
                });
            }
            while (InputBuffer.TryPeekFront(out var item) &&
                   item.time < time - dt)
            {
                InputBuffer.PopFront();
            }
        }

        private bool InputCommand(KiraraActionSO action, EActionCommand command, EActionCommandPhase phase)
        {
            var transition = GetExecutableCommandTransition(action, command, phase);
            if (transition != null)
            {
                // Debug.Log($"{name} 输入指令 {command} {phase} 跳转到 {transition.actionName}");
                PlayAction(transition.actionName, transition.fadeDuration);
                return true;
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
                        return true;
                    }
                }
            }

            if (string.IsNullOrEmpty(action.inheritActionTransition)) return false;

            var baseAction = ActionDict[action.inheritActionTransition];
            transition = GetExecutableCommandTransition(baseAction, command, phase);
            if (transition != null)
            {
                // Debug.Log($"{name} 输入指令 {command} {phase} 跳转到 {transition.actionName}");
                PlayAction(transition.actionName, transition.fadeDuration);
                return true;
            }
            return false;
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

            if (string.IsNullOrEmpty(_action.inheritActionTransition)) return;

            var baseAction = ActionDict[_action.inheritActionTransition];
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

        private void ClearNotifies()
        {
            _notifies.Clear();
            _notifiesFront = 0;
        }

        private void ClearNotifyStates()
        {
            _notifyStates.Clear();
            _notifyStatesFront = 0;
        }

        private void EndAndClearRunningNotifyStates()
        {
            foreach (var state in _runningNotifyStates)
            {
                state.NotifyEnd(this);
            }
            _runningNotifyStates.Clear();
        }

        public void Stop()
        {
            Debug.Log($"{name} Action Player Stop");
            _clip = null;
            Time = 0f;
            IsPlaying = false;
            ClearNotifies();
            ClearNotifyStates();
            EndAndClearRunningNotifyStates();
            // _animator.enabled = false;
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