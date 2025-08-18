using System;
using System.Collections.Generic;
using Kirara.Model;
using UnityEngine;

namespace Kirara.UI.Panel
{
    public class InventoryPanel : BasePanel
    {
        #region View
        private bool _isBound;
        private UnityEngine.UI.Button     UIBackBtn;
        private TMPro.TextMeshProUGUI     CoinText;
        private UnityEngine.UI.Image      CoinIconImg;
        private TMPro.TextMeshProUGUI     InventoryNameText;
        private Kirara.UI.UIDiscDetail    UIDiscDetail;
        private Kirara.UI.UITabController UITabController;
        private Kirara.UI.UIWeaponDetail  UIWeaponDetail;
        private KiraraLoopScroll.GridScrollView            WeaponLoopScroll;
        private KiraraLoopScroll.GridScrollView            DiscLoopScroll;
        public override void BindUI()
        {
            if (_isBound) return;
            _isBound = true;
            var c             = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            UIBackBtn         = c.Q<UnityEngine.UI.Button>(0, "UIBackBtn");
            CoinText          = c.Q<TMPro.TextMeshProUGUI>(1, "CoinText");
            CoinIconImg       = c.Q<UnityEngine.UI.Image>(2, "CoinIconImg");
            InventoryNameText = c.Q<TMPro.TextMeshProUGUI>(3, "InventoryNameText");
            UIDiscDetail      = c.Q<Kirara.UI.UIDiscDetail>(4, "UIDiscDetail");
            UITabController   = c.Q<Kirara.UI.UITabController>(5, "UITabController");
            UIWeaponDetail    = c.Q<Kirara.UI.UIWeaponDetail>(6, "UIWeaponDetail");
            WeaponLoopScroll  = c.Q<KiraraLoopScroll.GridScrollView>(7, "WeaponLoopScroll");
            DiscLoopScroll    = c.Q<KiraraLoopScroll.GridScrollView>(8, "DiscLoopScroll");
        }
        #endregion

        [SerializeField] private GameObject UIInventoryCellDiscPrefab;
        [SerializeField] private GameObject UIInventoryCellWeaponPrefab;

        private List<WeaponItem> _weapons = new();
        private readonly LiveData<WeaponItem> _selectedWeapon = new(null);

        private List<DiscItem> _discs = new();
        private readonly LiveData<DiscItem> _selectedDisc = new(null);

        protected override void Awake()
        {
            base.Awake();

            UIBackBtn.onClick.AddListener(() => UIMgr.Instance.PopPanel(this));

            CoinText.text = PlayerService.Player.Currencies
                .Find(x => x.Cid == 1)?.Count.ToString() ?? "0";

            _selectedWeapon.Add(value => UIWeaponDetail.Set(value));
            _selectedDisc.Add(value => UIDiscDetail.Set(value));

            SetItems();
            SetTab();
        }

        private void SetItems()
        {
            _weapons = PlayerService.Player.Weapons;
            if (_weapons.Count > 0)
            {
                _selectedWeapon.Value = _weapons[0];
            }

            var weaponPool = new LoopScrollGOPool(UIInventoryCellWeaponPrefab, transform);
            WeaponLoopScroll.SetSource(weaponPool);
            WeaponLoopScroll.provideData = ProvideWeaponData;
            WeaponLoopScroll._totalCount = _weapons.Count;

            _discs = PlayerService.Player.Discs;
            if (_discs.Count > 0)
            {
                _selectedDisc.Value = _discs[0];
            }

            var discPool = new LoopScrollGOPool(UIInventoryCellDiscPrefab, transform);
            DiscLoopScroll.SetSource(discPool);
            DiscLoopScroll.provideData = ProvideDiscData;
            DiscLoopScroll._totalCount = _discs.Count;
        }

        private void SetTab()
        {
            UITabController.onIndexChanged += UpdateTab;
            UpdateTab(UITabController.index);
        }

        private void UpdateTab(int index)
        {
            if (index == 0)
            {
                // if (weapons.Count > 0)
                // {
                //     WeaponIdx = 0;
                // }
                InventoryNameText.text = $"音擎仓库 {_weapons.Count}";
            }
            else if (index == 1)
            {
                // if (discs.Count > 0)
                // {
                //     DiscIdx = 0;
                // }
                InventoryNameText.text = $"驱动仓库 {_discs.Count}";
            }
        }

        private void ProvideWeaponData(GameObject go, int idx)
        {
            var data = _weapons[idx];
            var item = go.GetComponent<UIInventoryCellWeapon>();
            item.Set(data, _selectedWeapon);
        }

        private void ProvideDiscData(GameObject go, int idx)
        {
            var data = _discs[idx];
            var item = go.GetComponent<UIInventoryCellDisc>();
            item.Set(data, _selectedDisc);
        }
    }
}