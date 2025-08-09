using System;
using cfg.main;
using Kirara.Model;
using UnityEngine;

namespace Kirara.UI
{
    public class UIRoleBasicStat : MonoBehaviour
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
            var c                     = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
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