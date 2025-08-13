using System;
using System.Collections.Generic;
using System.Linq;
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

        private Stack<Transform> pool;

        private List<WeaponItem> weapons = new();
        private int _weaponIdx = -1;
        private int WeaponIdx
        {
            get => _weaponIdx;
            set
            {
                if (_weaponIdx == value) return;
                _weaponIdx = value;
                OnWeaponSelect?.Invoke();
            }
        }
        public Action OnWeaponSelect;

        private List<DiscItem> discs = new();
        private int _discIdx = -1;

        private int DiscIdx
        {
            get => _discIdx;
            set
            {
                if (_discIdx == value) return;

                _discIdx = value;
                OnDiscSelect?.Invoke();
            }
        }
        public Action OnDiscSelect;

        private LoopScrollGOPool weaponPool;
        private LoopScrollGOPool discPool;

        protected override void Awake()
        {
            base.Awake();

            UIBackBtn.onClick.AddListener(() => UIMgr.Instance.PopPanel(this));
            pool = new Stack<Transform>();

            CoinText.text = PlayerService.Player.Currencies
                .Find(x => x.Cid == 1)?.Count.ToString() ?? "0";

            SetItems();
            SetTab();
        }

        private void SetItems()
        {
            weapons = PlayerService.Player.Weapons;

            weaponPool = new LoopScrollGOPool(UIInventoryCellWeaponPrefab, transform);
            WeaponLoopScroll.SetGOSourceFunc(weaponPool.GetObject, weaponPool.ReturnObject);
            WeaponLoopScroll.provideData = ProvideWeaponData;
            WeaponLoopScroll._totalCount = weapons.Count;

            discs = PlayerService.Player.Discs;

            discPool = new LoopScrollGOPool(UIInventoryCellDiscPrefab, transform);
            DiscLoopScroll.SetGOSourceFunc(discPool.GetObject, discPool.ReturnObject);
            DiscLoopScroll.provideData = ProvideDiscData;
            DiscLoopScroll._totalCount = discs.Count;
        }

        private void SetTab()
        {
            UITabController.onIndexChanged += UpdateTab;
            UpdateTab(0);
        }

        private void UpdateTab(int index)
        {
            if (index == 0)
            {
                if (weapons.Count > 0)
                {
                    WeaponIdx = 0;
                }
                InventoryNameText.text = $"音擎仓库 {weapons.Count}";
            }
            else if (index == 1)
            {
                if (discs.Count > 0)
                {
                    DiscIdx = 0;
                }
                InventoryNameText.text = $"驱动仓库 {discs.Count}";
            }
        }

        private void ProvideWeaponData(GameObject go, int idx)
        {
            var item = weapons[idx];
            var cell = go.GetComponent<UIInventoryCellWeapon>();
            cell.Set(item, () =>
            {
                WeaponIdx = idx;
            });
        }

        private void ProvideDiscData(GameObject go, int idx)
        {
            var item = discs[idx];
            var cell = go.GetComponent<UIInventoryCellDisc>();
            cell.Set(item, () =>
            {
                DiscIdx = idx;
            });
        }
    }
}