using cfg.main;

using Manager;
using TMPro;
using UnityEngine;

namespace Kirara.UI
{
    public class UIStatBar : MonoBehaviour
    {
        private TextMeshProUGUI StatNameText;
        private TextMeshProUGUI StatValueText;
        private TextMeshProUGUI UpgradeTimeText;

        private void InitUI()
        {
            var c = GetComponent<KiraraRuntimeComponents>();
            c.Init();
            StatNameText = c.Q<TextMeshProUGUI>("StatNameText");
            StatValueText = c.Q<TextMeshProUGUI>("StatValueText");
            UpgradeTimeText = c.Q<TextMeshProUGUI>("UpgradeTimeText");
        }

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

        public UIStatBar Set(string nameText, float value, bool isPercentage, int upgradeTime = 0)
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

        public void Set(AttrEffect.AttrEffect ae, EAttrType attrType)
        {
            var config = ConfigMgr.tb.TbAttrShowConfig[attrType];
            Set(config.ShowName, ae.GetAttr(attrType).Evaluate(), config.ShowPct);
        }
    }
}