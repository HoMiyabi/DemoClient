
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using YooAsset;

namespace Kirara.UI
{
    public class UIExchangeCell : MonoBehaviour
    {
        private TextMeshProUGUI ToItemNameText;
        private Image FromItemIcon;
        private TextMeshProUGUI FromItemCountText;
        private TextMeshProUGUI ToItemCountText;
        private Image ToItemIcon;
        private Button Btn;

        private void InitUI()
        {
            var c = GetComponent<KiraraRuntimeComponents>();
            c.Init();
            ToItemNameText = c.Q<TextMeshProUGUI>("ToItemNameText");
            FromItemIcon = c.Q<Image>("FromItemIcon");
            FromItemCountText = c.Q<TextMeshProUGUI>("FromItemCountText");
            ToItemCountText = c.Q<TextMeshProUGUI>("ToItemCountText");
            ToItemIcon = c.Q<Image>("ToItemIcon");
            Btn = c.Q<Button>("Btn");
        }

        private void Awake()
        {
            InitUI();
        }

        private AssetHandle fromItemIconHandle;
        private AssetHandle toItemIconHandle;

        public void Clear()
        {
            fromItemIconHandle?.Release();
            fromItemIconHandle = null;
            toItemIconHandle?.Release();
            toItemIconHandle = null;

            Btn.onClick.RemoveAllListeners();
        }

        private void OnDestroy()
        {
            Clear();
        }

        public UIExchangeCell SetFromItem(NExchangeItem item)
        {
            var fromConfig = ConfigMgr.tb.TbCurrencyItemConfig[item.FromConfigId];

            fromItemIconHandle = AssetMgr.Instance.package.LoadAssetSync<Sprite>(fromConfig.IconLoc);

            FromItemIcon.sprite = fromItemIconHandle.AssetObject as Sprite;
            FromItemCountText.text = item.FromCount.ToString();

            return this;
        }

        public UIExchangeCell SetToItem(NExchangeItem item)
        {
            var toConfig = ConfigMgr.tb.TbWeaponConfig[item.ToConfigId];

            toItemIconHandle = AssetMgr.Instance.package.LoadAssetSync<Sprite>(toConfig.IconLoc);

            ToItemIcon.sprite = toItemIconHandle.AssetObject as Sprite;
            ToItemCountText.text = $"X{item.ToCount}";
            ToItemNameText.text = toConfig.Name;

            return this;
        }

        public UIExchangeCell OnClick(UnityAction action)
        {
            Btn.onClick.AddListener(action);

            return this;
        }

        public UIExchangeCell Set(NExchangeItem item)
        {
            Clear();

            SetFromItem(item);
            SetToItem(item);

            return this;
        }
    }
}