using Kirara.Model;
using UnityEngine.UI;

namespace Kirara.UI.Panel
{
    public class RoleDetailPanel : BasePanel
    {
        #region View
        private Button               UIBackBtn;
        private UITabController      UITabController;
        private UIRoleBasicStat UICharacterBasicStat;
        private UICharacterEquipment UICharacterEquipment;
        private void InitUI()
        {
            var c                = GetComponent<KiraraRuntimeComponents>();
            UIBackBtn            = c.Q<Button>(0, "UIBackBtn");
            UITabController      = c.Q<UITabController>(1, "UITabController");
            UICharacterBasicStat = c.Q<UIRoleBasicStat>(2, "UICharacterBasicStat");
            UICharacterEquipment = c.Q<UICharacterEquipment>(3, "UICharacterEquipment");
        }
        #endregion

        private Role Role { get; set; }

        private void Awake()
        {
            InitUI();
            UIBackBtn.onClick.AddListener(() => UIMgr.Instance.PopPanel(this));
        }

        public void Set(Role role)
        {
            Role = role;

            UICharacterBasicStat.Set(role);
            UICharacterEquipment.Set(role);
        }
    }
}