using UnityEngine;

namespace Kirara.UI
{
    public class UIRolesStatusBar : MonoBehaviour
    {
        #region View
        private bool _isBound;
        private Kirara.UI.UIBigRoleStatusBar   UIBigRoleStatusBar;
        private Kirara.UI.UISmallRoleStatusBar UISmallRoleStatusBar;
        private Kirara.UI.UISmallRoleStatusBar UISmallRoleStatusBar1;
        public void BindUI()
        {
            if (_isBound) return;
            _isBound = true;
            var c                 = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            UIBigRoleStatusBar    = c.Q<Kirara.UI.UIBigRoleStatusBar>(0, "UIBigRoleStatusBar");
            UISmallRoleStatusBar  = c.Q<Kirara.UI.UISmallRoleStatusBar>(1, "UISmallRoleStatusBar");
            UISmallRoleStatusBar1 = c.Q<Kirara.UI.UISmallRoleStatusBar>(2, "UISmallRoleStatusBar1");
        }
        #endregion

        private void Awake()
        {
            BindUI();
        }

        private void OnEnable()
        {
            PlayerSystem.Instance.OnFrontRoleChanged += UpdateView;
        }

        private void OnDisable()
        {
            PlayerSystem.Instance.OnFrontRoleChanged -= UpdateView;
        }

        private void UpdateView()
        {
            int chIdx = PlayerSystem.Instance.FrontRoleIdx;
            UIBigRoleStatusBar.Set(PlayerSystem.Instance.RoleCtrls[chIdx].Role);

            chIdx = PlayerSystem.Instance.GetNext(chIdx);
            UISmallRoleStatusBar.Set(PlayerSystem.Instance.RoleCtrls[chIdx].Role);

            chIdx = PlayerSystem.Instance.GetNext(chIdx);
            UISmallRoleStatusBar1.Set(PlayerSystem.Instance.RoleCtrls[chIdx].Role);
        }
    }
}