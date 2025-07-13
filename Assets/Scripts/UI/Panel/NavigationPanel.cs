using System;
using UnityEngine.UI;

namespace Kirara.UI.Panel
{
    public class NavigationPanel : BasePanel
    {
        #region View
        private Button SettingBtn;
        private Button InventoryBtn;
        private Button UIBackBtn;
        private Button FriendBtn;
        private void InitUI()
        {
            var c        = GetComponent<KiraraDirectBinder>();
            SettingBtn   = c.Q<Button>(0, "SettingBtn");
            InventoryBtn = c.Q<Button>(1, "InventoryBtn");
            UIBackBtn    = c.Q<Button>(2, "UIBackBtn");
            FriendBtn    = c.Q<Button>(3, "FriendBtn");
        }
        #endregion

        private void Awake()
        {
            InitUI();

            UIBackBtn.onClick.AddListener(() => UIMgr.Instance.PopPanel(this));

            SettingBtn.onClick.AddListener(() => UIMgr.Instance.PushPanel<SettingsPanel>());
            InventoryBtn.onClick.AddListener(() => UIMgr.Instance.PushPanel<InventoryPanel>());
            FriendBtn.onClick.AddListener(() => UIMgr.Instance.PushPanel<FriendPanel>());
        }
    }
}