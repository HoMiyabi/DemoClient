using cfg.main;
using Kirara.Model;
using UnityEngine;

namespace Kirara.UI
{
    public class UIRoleBasicStat : MonoBehaviour
    {
        #region View
        private bool _isBound;
        private Kirara.UI.UIStatBar   HPStatBar;
        private Kirara.UI.UIStatBar   ATKStatBar;
        private Kirara.UI.UIStatBar   DEFStatBar;
        private Kirara.UI.UIStatBar   ImpactStatBar;
        private Kirara.UI.UIStatBar   CRIT_RateStatBar;
        private Kirara.UI.UIStatBar   CRIT_DMGStatBar;
        private Kirara.UI.UIStatBar   AnomalyMasteryStatBar;
        private Kirara.UI.UIStatBar   AnomalyProficiencyStatBar;
        private Kirara.UI.UIStatBar   PEN_RatioStatBar;
        private Kirara.UI.UIStatBar   EnergyRegenStatBar;
        private TMPro.TextMeshProUGUI NameText;
        public void BindUI()
        {
            if (_isBound) return;
            _isBound = true;
            var b                     = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            HPStatBar                 = b.Q<Kirara.UI.UIStatBar>(0, "HPStatBar");
            ATKStatBar                = b.Q<Kirara.UI.UIStatBar>(1, "ATKStatBar");
            DEFStatBar                = b.Q<Kirara.UI.UIStatBar>(2, "DEFStatBar");
            ImpactStatBar             = b.Q<Kirara.UI.UIStatBar>(3, "ImpactStatBar");
            CRIT_RateStatBar          = b.Q<Kirara.UI.UIStatBar>(4, "CRIT_RateStatBar");
            CRIT_DMGStatBar           = b.Q<Kirara.UI.UIStatBar>(5, "CRIT_DMGStatBar");
            AnomalyMasteryStatBar     = b.Q<Kirara.UI.UIStatBar>(6, "AnomalyMasteryStatBar");
            AnomalyProficiencyStatBar = b.Q<Kirara.UI.UIStatBar>(7, "AnomalyProficiencyStatBar");
            PEN_RatioStatBar          = b.Q<Kirara.UI.UIStatBar>(8, "PEN_RatioStatBar");
            EnergyRegenStatBar        = b.Q<Kirara.UI.UIStatBar>(9, "EnergyRegenStatBar");
            NameText                  = b.Q<TMPro.TextMeshProUGUI>(10, "NameText");
        }
        #endregion

        private Role Role { get; set; }

        public void Set(Role role)
        {
            BindUI();
            Role = role;
            NameText.text = role.Config.Name;
        }

        private void UpdateInfo()
        {
            if (Role == null) return;

            ATKStatBar.Set(EAttrType.Atk, Role.Set[EAttrType.Atk]);
            DEFStatBar.Set(EAttrType.Def, Role.Set[EAttrType.Def]);
            ImpactStatBar.Set(EAttrType.Impact, Role.Set[EAttrType.Impact]);
            CRIT_RateStatBar.Set(EAttrType.CritRate, Role.Set[EAttrType.CritRate]);
            CRIT_DMGStatBar.Set(EAttrType.CritDmg, Role.Set[EAttrType.CritDmg]);
            AnomalyMasteryStatBar.Set(EAttrType.AnomalyMastery, Role.Set[EAttrType.AnomalyMastery]);
            AnomalyProficiencyStatBar.Set(EAttrType.AnomalyProficiency, Role.Set[EAttrType.AnomalyProficiency]);
            PEN_RatioStatBar.Set(EAttrType.PenRatio, Role.Set[EAttrType.PenRatio]);
            EnergyRegenStatBar.Set(EAttrType.EnergyRegen, Role.Set[EAttrType.EnergyRegen]);
            HPStatBar.Set(EAttrType.Hp, Role.Set[EAttrType.Hp]);
        }

        private void Update()
        {
            UpdateInfo();
        }
    }
}