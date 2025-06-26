using System;
using UnityEngine.UI;

namespace Kirara.UI.Panel
{
    public class FriendPanel : BasePanel
    {
        private Button UIBackBtn;

        private void InitUI()
        {
            var c = GetComponent<KiraraRuntimeComponents>();
            c.Init();
            UIBackBtn = c.Q<Button>("UIBackBtn");
        }

        private void Awake()
        {
            InitUI();

            UIBackBtn.onClick.AddListener(() => UIMgr.Instance.PopPanel(this));
        }
    }
}