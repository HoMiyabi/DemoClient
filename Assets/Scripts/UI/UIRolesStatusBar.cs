using System;
using cfg.main;
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
        private TMPro.TextMeshProUGUI          CurrHpText;
        private TMPro.TextMeshProUGUI          MaxHpText;
        public void BindUI()
        {
            if (_isBound) return;
            _isBound = true;
            var c                 = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            UIBigRoleStatusBar    = c.Q<Kirara.UI.UIBigRoleStatusBar>(0, "UIBigRoleStatusBar");
            UISmallRoleStatusBar  = c.Q<Kirara.UI.UISmallRoleStatusBar>(1, "UISmallRoleStatusBar");
            UISmallRoleStatusBar1 = c.Q<Kirara.UI.UISmallRoleStatusBar>(2, "UISmallRoleStatusBar1");
            CurrHpText            = c.Q<TMPro.TextMeshProUGUI>(3, "CurrHpText");
            MaxHpText             = c.Q<TMPro.TextMeshProUGUI>(4, "MaxHpText");
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

        private void Update()
        {
            var role = PlayerSystem.Instance.FrontRoleCtrl.Role;
            double currHp = role.Set[EAttrType.CurrHp];
            double maxHp = role.Set[EAttrType.Hp];
            CurrHpText.text = currHp.ToString("F0");
            MaxHpText.text = maxHp.ToString("F0");
        }
    }
}