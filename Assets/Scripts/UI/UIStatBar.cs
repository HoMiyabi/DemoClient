using cfg.main;
using Manager;
using TMPro;
using UnityEngine;

namespace Kirara.UI
{
    public class UIStatBar : MonoBehaviour
    {
        #region View
        private TextMeshProUGUI StatNameText;
        private TextMeshProUGUI StatValueText;
        private TextMeshProUGUI UpgradeTimeText;
        private void InitUI()
        {
            var c           = GetComponent<KiraraRuntimeComponents>();
            StatNameText    = c.Q<TextMeshProUGUI>(0, "StatNameText");
            StatValueText   = c.Q<TextMeshProUGUI>(1, "StatValueText");
            UpgradeTimeText = c.Q<TextMeshProUGUI>(2, "UpgradeTimeText");
        }
        #endregion

        private void Awake()
        {
            InitUI();
        }

        public void Set(NWeaponAttr attr, int upgradeTime = 0)
        {
            var config = ConfigMgr.tb.TbAttrShowConfig[(EAttrType)attr.AttrTypeId];
            StatNameText.text = config.ShowName;
            StatValueText.text = config.ShowPct ? $"{attr.Value:0.#%}" : $"{attr.Value:0.#}";

            if (upgradeTime == 0)
            {
                UpgradeTimeText.gameObject.SetActive(false);
            }
            else
            {
                UpgradeTimeText.gameObject.SetActive(true);
                UpgradeTimeText.text = $"+{upgradeTime}";
            }
        }

        public void Set(NDiscAttr attr)
        {
            var config = ConfigMgr.tb.TbAttrShowConfig[(EAttrType)attr.AttrTypeId];
            StatNameText.text = config.ShowName;
            StatValueText.text = config.ShowPct ? $"{attr.Value:0.#%}" : $"{attr.Value:0.#}";

            if (attr.UpgradeTimes == 0)
            {
                UpgradeTimeText.gameObject.SetActive(false);
            }
            else
            {
                UpgradeTimeText.gameObject.SetActive(true);
                UpgradeTimeText.text = $"+{attr.UpgradeTimes}";
            }
        }

        public UIStatBar Set(string nameText, double value, bool isPercentage, int upgradeTime = 0)
        {
            StatNameText.text = nameText;
            StatValueText.text = isPercentage ? $"{value:0.#%}" : $"{value:0.#}";

            if (upgradeTime == 0)
            {
                UpgradeTimeText.gameObject.SetActive(false);
            }
            else
            {
                UpgradeTimeText.gameObject.SetActive(true);
                UpgradeTimeText.text = $"+{upgradeTime}";
            }
            return this;
        }

        public void Set(EAttrType attrType, double value)
        {
            var config = ConfigMgr.tb.TbAttrShowConfig[attrType];
            Set(config.ShowName, value, config.ShowPct);
        }
    }
}