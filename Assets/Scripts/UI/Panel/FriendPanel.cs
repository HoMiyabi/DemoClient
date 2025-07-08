using System;
using UnityEngine.UI;

namespace Kirara.UI.Panel
{
    public class FriendPanel : BasePanel
    {
        #region View
        private Button UIBackBtn;
        private void InitUI()
        {
            var c     = GetComponent<KiraraRuntimeComponents>();
            UIBackBtn = c.Q<Button>(0, "UIBackBtn");
        }
        #endregion

        private void Awake()
        {
            InitUI();

            UIBackBtn.onClick.AddListener(() => UIMgr.Instance.PopPanel(this));
        }
    }
}