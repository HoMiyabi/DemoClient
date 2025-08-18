using UnityEngine;

namespace Kirara.UI.Panel
{
    public class CombatPanel : BasePanel
    {
        private GameInput input;

        private void Awake()
        {
            input = new GameInput();
            AddInput();
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

            if (PlayerSystem.Instance != null)
            {
                PlayerSystem.Instance.gameObject.SetActive(true);
            }

            input.Enable();
        }

        public override void OnPause()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            if (PlayerSystem.Instance != null)
            {
                PlayerSystem.Instance.gameObject.SetActive(false);
            }

            input.Disable();
        }
    }
}