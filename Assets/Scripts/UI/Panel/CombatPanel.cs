using System;
using UnityEngine;

namespace Kirara.UI.Panel
{
    public class CombatPanel : BasePanel
    {
        private GameInput input;

        protected override void Awake()
        {
            input = new GameInput();
            AddInput();
        }

        private void OnDestroy()
        {
            input.Dispose();
        }

        private void AddInput()
        {
            input.Combat.Esc.started += _ =>
            {
                // 导航面板
                UIMgr.Instance.PushPanel("NavigationPanel");
                // UIMgr.Instance.PushPanel<NavigationPanel>();
            };
            input.Combat.CharacterPanel.started += _ =>
            {
                // 选择角色面板
                UIMgr.Instance.PushPanel<RoleSelectPanel>();
            };
            input.Combat.OpenQuestPanel.started += _ =>
            {
                // 任务面板
                UIMgr.Instance.PushPanel<QuestPanel>();
            };
        }

        public override void OnResume()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            PlayerSystem.Instance.EnableInput = true;

            input.Enable();
        }

        public override void OnPause()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            PlayerSystem.Instance.EnableInput = false;

            input.Disable();
        }
    }
}