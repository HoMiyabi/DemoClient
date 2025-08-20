using Manager;
using UnityEngine;
using UnityEngine.Events;
using YooAsset;

namespace Kirara.UI
{
    public class UIExchangeItem : MonoBehaviour
    {
        #region View
        private bool _isBound;
        private TMPro.TextMeshProUGUI ToItemNameText;
        private UnityEngine.UI.Image  FromItemIcon;
        private TMPro.TextMeshProUGUI FromItemCountText;
        private TMPro.TextMeshProUGUI ToItemCountText;
        private UnityEngine.UI.Image  ToItemIcon;
        private UnityEngine.UI.Button Btn;
        public void BindUI()
        {
            if (_isBound) return;
            _isBound = true;
            var c             = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            ToItemNameText    = c.Q<TMPro.TextMeshProUGUI>(0, "ToItemNameText");
            FromItemIcon      = c.Q<UnityEngine.UI.Image>(1, "FromItemIcon");
            FromItemCountText = c.Q<TMPro.TextMeshProUGUI>(2, "FromItemCountText");
            ToItemCountText   = c.Q<TMPro.TextMeshProUGUI>(3, "ToItemCountText");
            ToItemIcon        = c.Q<UnityEngine.UI.Image>(4, "ToItemIcon");
            Btn               = c.Q<UnityEngine.UI.Button>(5, "Btn");
        }
        #endregion

        public void Clear()
        {
            Btn.onClick.RemoveAllListeners();
        }

        private UIExchangeItem SetFromItem(NExchangeItem item)
        {
            var fromConfig = ConfigMgr.tb.TbCurrencyItemConfig[item.FromConfigId];

            var fromItemIconHandle = YooAssets.LoadAssetSync<Sprite>(fromConfig.IconLoc);

            FromItemIcon.sprite = fromItemIconHandle.AssetObject as Sprite;
            FromItemCountText.text = item.FromCount.ToString();

            return this;
        }

        private UIExchangeItem SetToItem(NExchangeItem item)
        {
            var toConfig = ConfigMgr.tb.TbWeaponConfig[item.ToConfigId];

            var toItemIconHandle = YooAssets.LoadAssetSync<Sprite>(toConfig.IconLoc);

            ToItemIcon.sprite = toItemIconHandle.AssetObject as Sprite;
            ToItemCountText.text = $"X{item.ToCount}";
            ToItemNameText.text = toConfig.Name;

            return this;
        }

        public UIExchangeItem OnClick(UnityAction action)
        {
            Btn.onClick.RemoveAllListeners();
            Btn.onClick.AddListener(action);
            return this;
        }

        public UIExchangeItem Set(NExchangeItem item)
        {
            BindUI();
            Clear();

            SetFromItem(item);
            SetToItem(item);

            return this;
        }
    }
}