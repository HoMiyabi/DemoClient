using Kirara.Model;

namespace Kirara.UI.Panel
{
    public class RoleDetailPanel : BasePanel
    {
        #region View
        private bool _isBound;
        private UnityEngine.UI.Button          UIBackBtn;
        private Kirara.UI.UITabController      UITabController;
        private Kirara.UI.UIRoleBasicStat      UICharacterBasicStat;
        private Kirara.UI.UIRoleEquipment UIRoleEquipment;
        public override void BindUI()
        {
            if (_isBound) return;
            _isBound = true;
            var b                = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            UIBackBtn            = b.Q<UnityEngine.UI.Button>(0, "UIBackBtn");
            UITabController      = b.Q<Kirara.UI.UITabController>(1, "UITabController");
            UICharacterBasicStat = b.Q<Kirara.UI.UIRoleBasicStat>(2, "UICharacterBasicStat");
            UIRoleEquipment      = b.Q<Kirara.UI.UIRoleEquipment>(3, "UIRoleEquipment");
        }
        #endregion

        private Role Role { get; set; }

        private void Start()
        {
            UIBackBtn.onClick.AddListener(() => UIMgr.Instance.PopPanel(this));
        }

        public void Set(Role role)
        {
            Role = role;

            UICharacterBasicStat.Set(role);
            UIRoleEquipment.Set(role);
        }
    }
}