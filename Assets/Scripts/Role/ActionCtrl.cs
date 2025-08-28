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
        public EActionState State { get; set; }

        public readonly List<CancelWindowPlayableAsset> cancelWindowsAsset = new();

        public Func<KiraraActionSO, bool> isActionExecutable;
        public Action<KiraraActionSO, string> onPlayAction;

        private bool IsPressed(Dictionary<EActionCommand, bool> pressedDict, EActionCommand command)
        {
            return pressedDict.TryGetValue(command, out bool pressed) && pressed;
        }

        private void Awake()
        {
            ActionPlayer = GetComponent<ActionPlayer>();
            Refresh();
        }

        public void Refresh()
        {
            ActionDict = actionList.ActionDict;
        }

        public void UpdatePressed(Dictionary<EActionCommand, bool> pressedDict)
        {
            // End只要Press就能Move，特判一下吧
            if (_action != null &&
                State == EActionState.End &&
                IsPressed(pressedDict, EActionCommand.Move))
            {
                string actionName = GetDefaultAction(EActionCommand.Move);
                if (!string.IsNullOrEmpty(actionName))
                {
                    var action = ActionDict[actionName];

                    if (ActionPriority.Get(action.actionState) > ActionPriority.Get(State))
                    {
                        Debug.Log(($"优先级转移 {action.name} > {_action.name}"));
                        PlayAction(actionName, 0.15f);
                    }
                }
            }

            // 按下Press阶段的触发
            foreach (var cancelWin in cancelWindowsAsset)
            {
                if (cancelWin.cancelInfo.phase == EActionCommandPhase.Press &&
                    IsPressed(pressedDict, cancelWin.cancelInfo.command) &&
                    cancelWin.Inside(ActionPlayer.Time))
                {
                    PlayAction(cancelWin.cancelInfo.actionName, cancelWin.cancelInfo.fadeDuration);
                    return;
                }
            }
        }

        public void Input(EActionCommand command, EActionCommandPhase phase)
        {
            if (_action == null) return;

            // 检查取消窗口转移
            foreach (var cancelWin in cancelWindowsAsset)
            {
                if (cancelWin.Check(command, phase, ActionPlayer.Time) &&
                    TryPlayAction(cancelWin.cancelInfo.actionName, cancelWin.cancelInfo.fadeDuration))
                {
                    return;
                }
            }

            // 优先级转移
            if (phase == EActionCommandPhase.Down)
            {
                string actionName = GetDefaultAction(command);
                if (!string.IsNullOrEmpty(actionName))
                {
                    var action = ActionDict[actionName];
                    if (ActionPriority.Get(action.actionState) > ActionPriority.Get(State))
                    {
                        // Debug.Log(($"优先级转移 [{action.name}] > [{_action.name}]"));
                        PlayAction(actionName, 0.15f);
                    }
                }
            }
        }

        private string GetDefaultAction(EActionCommand command)
        {
            if (command == EActionCommand.Move)
            {
                return ActionName.Walk_Start;
            }
            if (command == EActionCommand.Dodge)
            {
                return ActionName.Evade_Back;
            }
            if (command == EActionCommand.BaseAttack)
            {
                return ActionName.Attack_Normal_01;
            }
            if (command == EActionCommand.SpecialAttack)
            {
                if (IsActionExecutable(ActionName.Attack_Ex_Special)) return ActionName.Attack_Ex_Special;
                if (IsActionExecutable(ActionName.Attack_Special)) return ActionName.Attack_Special;
            }
            return null;
        }

        public bool IsActionExecutable(string actionName)
        {
            var action = ActionDict[actionName];
            return IsActionExecutableInternal(action);
        }

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
            State = action.actionState;
            onPlayAction?.Invoke(action, actionName);

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

            var finishCancel = _action.finishCancelInfo;
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
    }
}