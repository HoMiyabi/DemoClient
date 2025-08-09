using System;

namespace Kirara.UI.Panel
{
    public class SocialPanel : BasePanel
    {
        #region View
        private bool _isBound;
        private UnityEngine.UI.Button UIBackBtn;
        public override void BindUI()
        {
            if (_isBound) return;
            _isBound = true;
            var c     = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            UIBackBtn = c.Q<UnityEngine.UI.Button>(0, "UIBackBtn");
        }
        #endregion

        private void Start()
        {
            UIBackBtn.onClick.AddListener(() => UIMgr.Instance.PopPanel(this));
        }
    }
}