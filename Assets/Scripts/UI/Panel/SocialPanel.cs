using UnityEngine.UI;

namespace Kirara.UI.Panel
{
    public class SocialPanel : BasePanel
    {
        #region View
        private Button UIBackBtn;
        private void InitUI()
        {
            var c     = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
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