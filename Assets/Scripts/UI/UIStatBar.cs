using cfg.main;
using Manager;
using UnityEngine;

namespace Kirara.UI
{
    public class UIStatBar : MonoBehaviour
    {
        #region View
        private bool _isBound;
        private TMPro.TextMeshProUGUI StatNameText;
        private TMPro.TextMeshProUGUI StatValueText;
        private TMPro.TextMeshProUGUI UpgradeTimeText;
        public void BindUI()
        {
            if (_isBound) return;
            _isBound = true;
            var c           = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            StatNameText    = c.Q<TMPro.TextMeshProUGUI>(0, "StatNameText");
            StatValueText   = c.Q<TMPro.TextMeshProUGUI>(1, "StatValueText");
            UpgradeTimeText = c.Q<TMPro.TextMeshProUGUI>(2, "UpgradeTimeText");
        }
        #endregion

        public void Set(NWeaponAttr attr, int upgradeTime = 0)
        {
            BindUI();

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
            BindUI();

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
            BindUI();

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