using cfg.main;
using Kirara.Model;
using UnityEngine;

namespace Kirara.UI
{
    public class UICharacterBasicStat : MonoBehaviour
    {
        #region View
        private UIStatBar HPStatBar;
        private UIStatBar ATKStatBar;
        private UIStatBar DEFStatBar;
        private UIStatBar ImpactStatBar;
        private UIStatBar CRIT_RateStatBar;
        private UIStatBar CRIT_DMGStatBar;
        private UIStatBar AnomalyMasteryStatBar;
        private UIStatBar AnomalyProficiencyStatBar;
        private UIStatBar PEN_RatioStatBar;
        private UIStatBar EnergyRegenStatBar;
        private void InitUI()
        {
            var c                     = GetComponent<KiraraRuntimeComponents>();
            HPStatBar                 = c.Q<UIStatBar>(0, "HPStatBar");
            ATKStatBar                = c.Q<UIStatBar>(1, "ATKStatBar");
            DEFStatBar                = c.Q<UIStatBar>(2, "DEFStatBar");
            ImpactStatBar             = c.Q<UIStatBar>(3, "ImpactStatBar");
            CRIT_RateStatBar          = c.Q<UIStatBar>(4, "CRIT_RateStatBar");
            CRIT_DMGStatBar           = c.Q<UIStatBar>(5, "CRIT_DMGStatBar");
            AnomalyMasteryStatBar     = c.Q<UIStatBar>(6, "AnomalyMasteryStatBar");
            AnomalyProficiencyStatBar = c.Q<UIStatBar>(7, "AnomalyProficiencyStatBar");
            PEN_RatioStatBar          = c.Q<UIStatBar>(8, "PEN_RatioStatBar");
            EnergyRegenStatBar        = c.Q<UIStatBar>(9, "EnergyRegenStatBar");
        }
        #endregion

        private Role Role { get; set; }

        private void Awake()
        {
            InitUI();
        }

        public void Set(Role role)
        {
            Role = role;
            UpdateInfo();
        }

        private void OnEnable()
        {
            UpdateInfo();
        }

        private void UpdateInfo()
        {
            if (Role == null) return;

            ATKStatBar.Set(EAttrType.Atk, Role.AttrSet[EAttrType.Atk]);
            DEFStatBar.Set(EAttrType.Def, Role.AttrSet[EAttrType.Def]);
            ImpactStatBar.Set(EAttrType.Impact, Role.AttrSet[EAttrType.Impact]);
            CRIT_RateStatBar.Set(EAttrType.CritRate, Role.AttrSet[EAttrType.CritRate]);
            CRIT_DMGStatBar.Set(EAttrType.CritDmg, Role.AttrSet[EAttrType.CritDmg]);
            AnomalyMasteryStatBar.Set(EAttrType.AnomalyMastery, Role.AttrSet[EAttrType.AnomalyMastery]);
            AnomalyProficiencyStatBar.Set(EAttrType.AnomalyProficiency, Role.AttrSet[EAttrType.AnomalyProficiency]);
            PEN_RatioStatBar.Set(EAttrType.PenRatio, Role.AttrSet[EAttrType.PenRatio]);
            EnergyRegenStatBar.Set(EAttrType.EnergyRegen, Role.AttrSet[EAttrType.EnergyRegen]);
            HPStatBar.Set(EAttrType.Hp, Role.AttrSet[EAttrType.Hp]);
        }
    }
}