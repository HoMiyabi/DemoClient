using System.Linq;
using Manager;
using UnityEngine;
using UnityEngine.Events;
using YooAsset;

namespace Kirara.UI.Panel
{
    public class ExchangeDialogPanel : BasePanel
    {
        #region View
        private bool _isBound;
        private TMPro.TextMeshProUGUI TitleText;
        private TMPro.TextMeshProUGUI ToNameCountText;
        private TMPro.TextMeshProUGUI ToDescText;
        private Kirara.UI.UINumSlider UINumSlider;
        private UnityEngine.UI.Button UICloseBtn;
        private UnityEngine.UI.Image  FromIcon;
        private TMPro.TextMeshProUGUI FromCountCostText;
        private UnityEngine.UI.Button UIOverlayBtn;
        private TMPro.TextMeshProUGUI ExchangeCountText;
        private UnityEngine.UI.Button ConfirmBtn;
        private UnityEngine.UI.Image  ToIcon;
        public override void BindUI()
        {
            if (_isBound) return;
            _isBound = true;
            var b             = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            TitleText         = b.Q<TMPro.TextMeshProUGUI>(0, "TitleText");
            ToNameCountText   = b.Q<TMPro.TextMeshProUGUI>(1, "ToNameCountText");
            ToDescText        = b.Q<TMPro.TextMeshProUGUI>(2, "ToDescText");
            UINumSlider       = b.Q<Kirara.UI.UINumSlider>(3, "UINumSlider");
            UICloseBtn        = b.Q<UnityEngine.UI.Button>(4, "UICloseBtn");
            FromIcon          = b.Q<UnityEngine.UI.Image>(5, "FromIcon");
            FromCountCostText = b.Q<TMPro.TextMeshProUGUI>(6, "FromCountCostText");
            UIOverlayBtn      = b.Q<UnityEngine.UI.Button>(7, "UIOverlayBtn");
            ExchangeCountText = b.Q<TMPro.TextMeshProUGUI>(8, "ExchangeCountText");
            ConfirmBtn        = b.Q<UnityEngine.UI.Button>(9, "ConfirmBtn");
            ToIcon            = b.Q<UnityEngine.UI.Image>(10, "ToIcon");
        }
        #endregion

        public string Title { get => TitleText.text; set => TitleText.text = value; }
        public int Value => UINumSlider.Value;

        private AssetHandle fromIconHandle;
        private AssetHandle toIconHandle;

        protected override void Awake()
        {
            base.Awake();

            Title = "兑换确认";
            UICloseBtn.onClick.AddListener(() => UIMgr.Instance.PopPanel(this));
            UIOverlayBtn.onClick.AddListener(() => UIMgr.Instance.PopPanel(this));
        }

        public UnityAction Confirmed
        {
            set
            {
                ConfirmBtn.onClick.RemoveAllListeners();
                ConfirmBtn.onClick.AddListener(value);
            }
        }

        public void Clear()
        {
            fromIconHandle?.Release();
            fromIconHandle = null;
            toIconHandle?.Release();
            toIconHandle = null;
        }

        private void OnDestroy()
        {
            Clear();
        }

        public void SetFrom(NExchangeItem item)
        {
            var fromConfig = ConfigMgr.tb.TbCurrencyItemConfig[item.FromConfigId];
            fromIconHandle = YooAssets.LoadAssetSync<Sprite>(fromConfig.IconLoc);
            FromIcon.sprite = fromIconHandle.AssetObject as Sprite;
        }

        public void SetTo(NExchangeItem item)
        {
            var toConfig = ConfigMgr.tb.TbWeaponConfig[item.ToConfigId];
            toIconHandle = YooAssets.LoadAssetSync<Sprite>(toConfig.IconLoc);
            ToIcon.sprite = toIconHandle.AssetObject as Sprite;
            ToNameCountText.text = $"{toConfig.Name} X{item.ToCount}";
            ToDescText.text = toConfig.Desc;
        }

        private void SetCount(NExchangeItem item)
        {
            var items = PlayerService.Player.Currencies;
            var currency = items.First(x => x.Cid == item.FromConfigId);
            UINumSlider.Set(1, currency.Count / item.FromCount);

            ExchangeCountText.text = "兑换数量 X 1";
            UINumSlider.OnValueChanged += (exchangeCnt) =>
            {
                ExchangeCountText.text = $"兑换数量 X {exchangeCnt}";
            };
            UINumSlider.OnValueChanged += (exchangeCnt) =>
            {
                FromCountCostText.text = $"{currency.Count} / {item.FromCount * exchangeCnt}";
            };
        }

        public ExchangeDialogPanel Set(NExchangeItem item)
        {
            Clear();

            SetFrom(item);
            SetTo(item);
            SetCount(item);

            return this;
        }
    }
}