using UnityEngine;

namespace Kirara.UI
{
    public class UIRolesStatus : MonoBehaviour
    {
        #region View
        private UIBigRoleStatusBar   UIBigRoleStatusBar;
        private UISmallRoleStatusBar UISmallRoleStatusBar;
        private UISmallRoleStatusBar UISmallRoleStatusBar1;
        private void InitUI()
        {
            var c                 = GetComponent<KiraraDirectBinder>();
            UIBigRoleStatusBar    = c.Q<UIBigRoleStatusBar>(0, "UIBigRoleStatusBar");
            UISmallRoleStatusBar  = c.Q<UISmallRoleStatusBar>(1, "UISmallRoleStatusBar");
            UISmallRoleStatusBar1 = c.Q<UISmallRoleStatusBar>(2, "UISmallRoleStatusBar1");
        }
        #endregion

        private void Awake()
        {
            InitUI();
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