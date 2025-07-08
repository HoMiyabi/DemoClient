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

        private Role ch;

        private void Awake()
        {
            InitUI();
        }

        public void Set(Role ch)
        {
            this.ch = ch;
            UpdateInfo();
        }

        private void OnEnable()
        {
            UpdateInfo();
        }

        private void UpdateInfo()
        {
            if (ch == null) return;

            ATKStatBar.Set(ch.ae, EAttrType.Atk);
            DEFStatBar.Set(ch.ae, EAttrType.Def);
            ImpactStatBar.Set(ch.ae, EAttrType.Impact);
            CRIT_RateStatBar.Set(ch.ae, EAttrType.CritRate);
            CRIT_DMGStatBar.Set(ch.ae, EAttrType.CritDmg);
            AnomalyMasteryStatBar.Set(ch.ae, EAttrType.AnomalyMastery);
            AnomalyProficiencyStatBar.Set(ch.ae, EAttrType.AnomalyProficiency);
            PEN_RatioStatBar.Set(ch.ae, EAttrType.PenRatio);
            EnergyRegenStatBar.Set(ch.ae, EAttrType.EnergyRegen);
            HPStatBar.Set(ch.ae, EAttrType.Hp);
        }
    }
}