using System;
using System.Collections.Generic;
using Kirara.TimelineAction;
using UnityEngine;

namespace Kirara
{
    public class ActionCtrl : MonoBehaviour
    {
        public KiraraActionListSO actionList;
        public Dictionary<string, KiraraActionSO> ActionDict { get; private set; }
        public ActionPlayer ActionPlayer { get; private set; }

        private KiraraActionSO _action;
        // public EActionState State { get; set; }

        public Func<KiraraActionSO, bool> isActionExecutable;
        public Action<KiraraActionSO, string> onPlayAction;
        public Action<ActionParams> onSetActionParams;
        public KiraraActionSO OverrideAction { get; set; }

        // private bool IsPressed(Dictionary<EActionCommand, bool> pressedDict, EActionCommand command)
        // {
        //     return pressedDict.TryGetValue(command, out bool pressed) && pressed;
        // }

        private void Awake()
        {
            ActionPlayer = GetComponent<ActionPlayer>();
            Refresh();
        }

        public void Refresh()
        {
            ActionDict = actionList.ActionDict;
        }

        // public void UpdatePressed(Dictionary<EActionCommand, bool> pressedDict)
        // {
        //     // End只要Press就能Move，特判一下吧
        //     if (_action != null &&
        //         State == EActionState.End &&
        //         IsPressed(pressedDict, EActionCommand.Move))
        //     {
        //         string actionName = GetDefaultAction(EActionCommand.Move);
        //         if (!string.IsNullOrEmpty(actionName))
        //         {
        //             var action = ActionDict[actionName];
        //
        //             if (ActionPriority.Get(action.actionState) > ActionPriority.Get(State))
        //             {
        //                 Debug.Log(($"优先级转移 {action.name} > {_action.name}"));
        //                 PlayAction(actionName, 0.15f);
        //             }
        //         }
        //     }
        //
        //     // 按下Press阶段的触发
        //     foreach (var cancelWin in commandTransitionNotifyStates)
        //     {
        //         if (cancelWin.commandTransition.phase == EActionCommandPhase.Press &&
        //             IsPressed(pressedDict, cancelWin.commandTransition.command) &&
        //             cancelWin.Inside(ActionPlayer.Time))
        //         {
        //             PlayAction(cancelWin.commandTransition.actionName, cancelWin.commandTransition.fadeDuration);
        //             return;
        //         }
        //     }
        // }

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
            if (ActionCommandTransition(action, command, phase)) return;

            // 检查指令跳转通知状态
            foreach (var state in ActionPlayer._runningNotifyStates)
            {
                if (state is CommandTransitionNotifyState transitionState)
                {
                    if (transitionState.Check(command, phase, ActionPlayer.Time) &&
                        TryPlayAction(transitionState.commandTransition.actionName,
                            transitionState.commandTransition.fadeDuration))
                    {
                        return;
                    }
                }
            }

            if (!string.IsNullOrEmpty(action.inheritTransitionActionName))
            {
                var baseAction = ActionDict[action.inheritTransitionActionName];
                ActionCommandTransition(baseAction, command, phase);
            }
        }

        private bool ActionCommandTransition(KiraraActionSO action, EActionCommand command, EActionCommandPhase phase)
        {
            // 检查动作的指令跳转
            if (action.commandTransitions != null)
            {
                foreach (var transition in action.commandTransitions)
                {
                    if (transition.command == command &&
                        transition.phase == phase &&
                        TryPlayAction(transition.actionName, transition.fadeDuration))
                    {
                        return true;
                    }
                }
            }

            return false;

            // // 优先级转移
            // if (phase == EActionCommandPhase.Down)
            // {
            //     string actionName = GetDefaultAction(command);
            //     if (!string.IsNullOrEmpty(actionName))
            //     {
            //         var action = ActionDict[actionName];
            //         if (ActionPriority.Get(action.actionState) > ActionPriority.Get(State))
            //         {
            //             // Debug.Log(($"优先级转移 [{action.name}] > [{_action.name}]"));
            //             PlayAction(actionName, 0.15f);
            //         }
            //     }
            // }
        }

        // private string GetDefaultAction(EActionCommand command)
        // {
        //     if (command == EActionCommand.Move)
        //     {
        //         return ActionName.Walk_Start;
        //     }
        //     if (command == EActionCommand.Dodge)
        //     {
        //         return ActionName.Evade_Back;
        //     }
        //     if (command == EActionCommand.BaseAttack)
        //     {
        //         return ActionName.Attack_Normal_01;
        //     }
        //     if (command == EActionCommand.SpecialAttack)
        //     {
        //         if (IsActionExecutable(ActionName.Attack_Ex_Special)) return ActionName.Attack_Ex_Special;
        //         if (IsActionExecutable(ActionName.Attack_Special)) return ActionName.Attack_Special;
        //     }
        //     return null;
        // }

        // public bool IsActionExecutable(string actionName)
        // {
        //     var action = ActionDict[actionName];
        //     return IsActionExecutableInternal(action);
        // }

        private bool IsActionExecutableInternal(KiraraActionSO action)
        {
            if (isActionExecutable != null)
            {
                return isActionExecutable(action);
            }
            return true;
        }

        public void PlayActionFullName(string actionFullName, float fadeDuration = 0f, Action onFinish = null)
        {
            PlayAction(actionList.RemoveNamePrefix(actionFullName), fadeDuration, onFinish);
        }

        public bool TryPlayAction(string actionName, float fadeDuration = 0f, Action onFinish = null)
        {
            if (!ActionDict.TryGetValue(actionName, out var action))
            {
                Debug.LogError($"{name} 没有动作 {actionName}");
                return false;
            }
            if (!IsActionExecutableInternal(action)) return false;
            PlayActionInternal(action, actionName, fadeDuration, onFinish);
            return true;
        }

        private void PlayActionInternal(KiraraActionSO action, string actionName, float fadeDuration = 0f, Action onFinish = null)
        {
            _action = action;
            OverrideAction = null;
            // State = action.actionState;
            onPlayAction?.Invoke(action, actionName);
            onSetActionParams?.Invoke(action.actionParams);

            // string s1 = _action?.name ?? "null";
            // string s2 = action.name ?? "null";
            // if (s1 != s2)
            // {
            //     // Debug.Log($"[{s1}] to [{s2}], fadeDuration = {fadeDuration}");
            // }

            // 结束取消
            AddEndCancel(ref onFinish);

            ActionPlayer.Play(action, actionName, fadeDuration, onFinish);
        }

        public void PlayAction(string actionName, float fadeDuration = 0f, Action onFinish = null)
        {
            if (!ActionDict.TryGetValue(actionName, out var action))
            {
                Debug.LogError($"{name} 没有动作 {actionName}");
                return;
            }
            PlayActionInternal(action, actionName, fadeDuration, onFinish);
        }

        private void AddEndCancel(ref Action onFinish)
        {
            // 结束取消
            if (_action.isLoop) return;

            var finishCancel = _action.finishTransition;
            if (!string.IsNullOrEmpty(finishCancel.actionName))
            {
                // Debug.Log($"{name} 添加结束转移到{_action.finishNextActionName}");
                onFinish += () =>
                {
                    PlayAction(finishCancel.actionName, finishCancel.fadeDuration);
                };
            }
            else
            {
                Debug.LogWarning($"{name} {_action.name} 没有结束取消");
            }
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
            if (ActionSignalTransition(action, signalName)) return;

            if (string.IsNullOrEmpty(_action.inheritTransitionActionName)) return;

            var baseAction = ActionDict[_action.inheritTransitionActionName];
            ActionSignalTransition(baseAction, signalName);
        }

        private bool ActionSignalTransition(KiraraActionSO action, string signalName)
        {
            if (action.signalTransitions != null)
            {
                foreach (var signalTransition in action.signalTransitions)
                {
                    if (signalTransition.signalName == signalName &&
                        TryPlayAction(signalTransition.actionName, signalTransition.fadeDuration))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}