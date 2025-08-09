using cfg.main;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YooAsset;

namespace Kirara.UI
{
    public class UIQuestRewordCell : MonoBehaviour
    {
        #region View
        private Image           Icon;
        private TextMeshProUGUI CountText;
        private void InitUI()
        {
            var c     = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            Icon      = c.Q<Image>(0, "Icon");
            CountText = c.Q<TextMeshProUGUI>(1, "CountText");
        }
        #endregion

        private AssetHandle handle;

        private void Awake()
        {
            InitUI();
        }

        private void Clear()
        {
            handle?.Release();
            handle = null;
        }

        private void OnDestroy()
        {
            Clear();
        }

        public void Set(RewordConfig reword)
        {
            Clear();
            var itemConfig = ConfigMgr.tb.TbCurrencyItemConfig[reword.CurrencyCid];

            handle = AssetMgr.Instance.package.LoadAssetSync<Sprite>(itemConfig.IconLoc);
            Icon.sprite = handle.AssetObject as Sprite;

            CountText.text = reword.Count.ToString();
        }
    }
}