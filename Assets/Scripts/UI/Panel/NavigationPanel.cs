using System;
using UnityEngine.UI;

namespace Kirara.UI.Panel
{
    public class NavigationPanel : BasePanel
    {
        private Button SettingBtn;
        private Button InventoryBtn;
        private Button UIBackBtn;
        private Button FriendBtn;

        private void InitUI()
        {
            var c = GetComponent<KiraraRuntimeComponents>();
            c.Init();
            SettingBtn = c.Q<Button>("SettingBtn");
            InventoryBtn = c.Q<Button>("InventoryBtn");
            UIBackBtn = c.Q<Button>("UIBackBtn");
            FriendBtn = c.Q<Button>("FriendBtn");
        }

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