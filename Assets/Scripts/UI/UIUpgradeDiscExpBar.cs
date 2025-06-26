using Kirara.Model;
using Kirara.Service;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kirara.UI
{
    public class UIUpgradeDiscExpBar : MonoBehaviour
    {
        private Image BgBar;
        private Image Bar;
        private TextMeshProUGUI CurrentLevelText;
        private TextMeshProUGUI UpgradedLevelText;
        private TextMeshProUGUI ExpText;

        private void InitUI()
        {
            var c = GetComponent<KiraraRuntimeComponents>();
            c.Init();
            BgBar = c.Q<Image>("BgBar");
            Bar = c.Q<Image>("Bar");
            CurrentLevelText = c.Q<TextMeshProUGUI>("CurrentLevelText");
            UpgradedLevelText = c.Q<TextMeshProUGUI>("UpgradedLevelText");
            ExpText = c.Q<TextMeshProUGUI>("ExpText");
        }

        private DiscItem disc;

        private int animLevel;
        private float animFillAmount;
        private int upgradedLevel;
        private float fillAmount;

        private void Awake()
        {
            InitUI();
        }

        private void Clear()
        {
            if (disc != null)
            {
                disc.OnLevelChanged -= UpdateCurrentLevelView;
            }
        }

        public void Set(DiscItem disc)
        {
            this.disc = disc;
            UpdateCurrentLevelView();
            disc.OnLevelChanged += UpdateCurrentLevelView;
            SetAddExp(0);
        }

        private void UpdateCurrentLevelView()
        {
            CurrentLevelText.text = disc.Level.ToString();
        }

        public void SetAddExp(int addExp)
        {
            DiscService.CalcUpgraded(disc, addExp, out int level, out int exp);
            int upgradeNextExp;
            if (level == DiscItem.MaxLevel)
            {
                upgradeNextExp = DiscItem.GetExp(level);
                exp = upgradeNextExp;
            }
            else
            {
                upgradeNextExp = DiscItem.GetExp(level + 1);
            }

            ExpText.text = $"{addExp}/{upgradeNextExp}";
            UpgradedLevelText.text = level.ToString();

            if (level > disc.Level)
            {
                BgBar.gameObject.SetActive(true);
            }
            else
            {
                BgBar.gameObject.SetActive(false);
            }

            Bar.fillAmount = (float)exp / upgradeNextExp;
        }

        // private void Update()
        // {
        //     if (animLevel != upgradedLevel || animFillAmount != fillAmount)
        //     {
        //
        //     }
        // }
    }
}