using System;
using System.Collections.Generic;
using System.Linq;
using cfg.main;
using Kirara.TimelineAction;
using Manager;
using UnityEngine;

namespace Kirara
{
    public class ActionCtrl : MonoBehaviour
    {
        public KiraraActionListSO actionList;
        public Dictionary<string, KiraraActionSO> ActionDict { get; private set; }
        public ActionPlayer ActionPlayer { get; private set; }
        private ChCtrl ch;

        private KiraraActionSO _action;
        public EActionState State { get; set; }

        public readonly List<CancelWindowPlayableAsset> cancelWindowsAsset = new();

        private bool IsPressed(Dictionary<EActionCommand, bool> pressedDict, EActionCommand command)
        {
            return pressedDict.TryGetValue(command, out bool pressed) && pressed;
        }

        private void Awake()
        {
            ActionPlayer = GetComponent<ActionPlayer>();
            ch = GetComponent<ChCtrl>();
            Refresh();
        }

        public void Refresh()
        {
            ActionDict = actionList.actions?
                .ToDictionary(x => RemoveNamePrefix(x.name));
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
                        ExecuteAction(actionName, 0.15f);
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
                    ExecuteAction(cancelWin.cancelInfo.actionName, cancelWin.cancelInfo.fadeDuration);
                    return;
                }
            }
        }

        // 前缀为Action_Name_
        private string RemoveNamePrefix(string actionFullName)
        {
            if (!string.IsNullOrEmpty(actionList.namePrefix))
            {
                if (actionFullName.StartsWith(actionList.namePrefix))
                {
                    return actionFullName[actionList.namePrefix.Length..];
                }
                Debug.LogError($"无法移除前缀 全名：{actionFullName}，前缀：{actionList.namePrefix}");
            }
            Debug.Log($"{name} 前缀为空");
            return actionFullName;
        }

        public void Input(EActionCommand command, EActionCommandPhase phase)
        {
            if (_action == null) return;

            // 检查取消窗口转移
            foreach (var cancelWin in cancelWindowsAsset)
            {
                if (cancelWin.Check(command, phase, ActionPlayer.Time) &&
                    TryExecuteAction(cancelWin.cancelInfo.actionName, cancelWin.cancelInfo.fadeDuration))
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
                        ExecuteAction(actionName, 0.15f);
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
            int actionId = action.actionId;
            if (actionId == 0) return true;
            var config = ConfigMgr.tb.TbChActionNumericConfig[actionId];
            if (config.EnergyCost <= ch.Role.ae.GetAttr(EAttrType.CurrEnergy).Evaluate())
            {
                return true;
            }
            return false;
        }

        public void ExecuteActionFullName(string actionFullName, float fadeDuration = 0f, Action onFinish = null)
        {
            ExecuteAction(RemoveNamePrefix(actionFullName), fadeDuration, onFinish);
        }

        public bool TryExecuteAction(string actionName, float fadeDuration = 0f, Action onFinish = null)
        {
            if (!ActionDict.TryGetValue(actionName, out var action))
            {
                Debug.LogError($"{name} 没有动作 {actionName}");
                return false;
            }
            if (!IsActionExecutableInternal(action)) return false;
            ExecuteActionInternal(action, actionName, fadeDuration, onFinish);
            return true;
        }

        private void ExecuteActionInternal(KiraraActionSO action, string actionName, float fadeDuration = 0f, Action onFinish = null)
        {
            _action = action;

            State = action.actionState;
            ch.SetActionParams(action.actionParams);

            if (action.actionId != 0)
            {
                var config = ConfigMgr.tb.TbChActionNumericConfig[action.actionId];
                if (config != null)
                {
                    ch.Role.ae.GetAttr(EAttrType.CurrEnergy).BaseValue -= config.EnergyCost;
                }
            }

            string s1 = _action?.name ?? "null";
            string s2 = action.name ?? "null";
            if (s1 != s2)
            {
                // Debug.Log($"[{s1}] to [{s2}], fadeDuration = {fadeDuration}");
            }

            // 结束取消
            AddEndCancel(ref onFinish);

            ActionPlayer.Play(action, actionName, fadeDuration, onFinish);
        }

        public void ExecuteAction(string actionName, float fadeDuration = 0f, Action onFinish = null)
        {
            if (!ActionDict.TryGetValue(actionName, out var action))
            {
                Debug.LogError($"{name} 没有动作 {actionName}");
                return;
            }
            ExecuteActionInternal(action, actionName, fadeDuration, onFinish);
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
                    ExecuteAction(finishCancel.actionName, finishCancel.fadeDuration);
                };
            }
            else
            {
                Debug.LogWarning($"{name} {_action.name} 没有结束取消");
            }
        }
    }
}