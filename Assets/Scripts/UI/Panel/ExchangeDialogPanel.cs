using System;
using System.Linq;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using YooAsset;

namespace Kirara.UI.Panel
{
    public class ExchangeDialogPanel : BasePanel
    {
        #region View
        private TextMeshProUGUI TitleText;
        private TextMeshProUGUI ToNameCountText;
        private TextMeshProUGUI ToDescText;
        private UINumSlider     UINumSlider;
        private Button          UICloseBtn;
        private Image           FromIcon;
        private TextMeshProUGUI FromCountCostText;
        private Button          UIOverlayBtn;
        private TextMeshProUGUI ExchangeCountText;
        private Button          ConfirmBtn;
        private Image           ToIcon;
        private void InitUI()
        {
            var c             = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            TitleText         = c.Q<TextMeshProUGUI>(0, "TitleText");
            ToNameCountText   = c.Q<TextMeshProUGUI>(1, "ToNameCountText");
            ToDescText        = c.Q<TextMeshProUGUI>(2, "ToDescText");
            UINumSlider       = c.Q<UINumSlider>(3, "UINumSlider");
            UICloseBtn        = c.Q<Button>(4, "UICloseBtn");
            FromIcon          = c.Q<Image>(5, "FromIcon");
            FromCountCostText = c.Q<TextMeshProUGUI>(6, "FromCountCostText");
            UIOverlayBtn      = c.Q<Button>(7, "UIOverlayBtn");
            ExchangeCountText = c.Q<TextMeshProUGUI>(8, "ExchangeCountText");
            ConfirmBtn        = c.Q<Button>(9, "ConfirmBtn");
            ToIcon            = c.Q<Image>(10, "ToIcon");
        }
        #endregion

        public string Title { get => TitleText.text; set => TitleText.text = value; }
        public int Value => UINumSlider.Value;

        private AssetHandle fromIconHandle;
        private AssetHandle toIconHandle;

        private void Awake()
        {
            InitUI();

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
            fromIconHandle = AssetMgr.Instance.package.LoadAssetSync<Sprite>(fromConfig.IconLoc);
            FromIcon.sprite = fromIconHandle.AssetObject as Sprite;
        }

        public void SetTo(NExchangeItem item)
        {
            var toConfig = ConfigMgr.tb.TbWeaponConfig[item.ToConfigId];
            toIconHandle = AssetMgr.Instance.package.LoadAssetSync<Sprite>(toConfig.IconLoc);
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