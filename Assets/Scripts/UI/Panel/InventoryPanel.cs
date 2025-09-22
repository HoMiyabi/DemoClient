using System.Collections.Generic;
using DG.Tweening;
using Kirara.Model;
using UnityEngine;

namespace Kirara.UI.Panel
{
    public class InventoryPanel : BasePanel
    {
        #region View
        private bool _isBound;
        private UnityEngine.UI.Button           UIBackBtn;
        private TMPro.TextMeshProUGUI           InventoryNameText;
        private Kirara.UI.UIDiscDetail          UIDiscDetail;
        private Kirara.UI.UIWeaponDetail        UIWeaponDetail;
        private KiraraLoopScroll.GridScrollView WeaponLoopScroll;
        private KiraraLoopScroll.GridScrollView DiscLoopScroll;
        private Kirara.UI.UITabController       UITabController;
        private UnityEngine.CanvasGroup         CanvasGroup;
        private Kirara.UI.UICurrencyArea        UICurrencyArea1;
        private Kirara.UI.UICurrencyArea        UICurrencyArea2;
        public override void BindUI()
        {
            if (_isBound) return;
            _isBound = true;
            var b             = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            UIBackBtn         = b.Q<UnityEngine.UI.Button>(0, "UIBackBtn");
            InventoryNameText = b.Q<TMPro.TextMeshProUGUI>(1, "InventoryNameText");
            UIDiscDetail      = b.Q<Kirara.UI.UIDiscDetail>(2, "UIDiscDetail");
            UIWeaponDetail    = b.Q<Kirara.UI.UIWeaponDetail>(3, "UIWeaponDetail");
            WeaponLoopScroll  = b.Q<KiraraLoopScroll.GridScrollView>(4, "WeaponLoopScroll");
            DiscLoopScroll    = b.Q<KiraraLoopScroll.GridScrollView>(5, "DiscLoopScroll");
            UITabController   = b.Q<Kirara.UI.UITabController>(6, "UITabController");
            CanvasGroup       = b.Q<UnityEngine.CanvasGroup>(7, "CanvasGroup");
            UICurrencyArea1   = b.Q<Kirara.UI.UICurrencyArea>(8, "UICurrencyArea1");
            UICurrencyArea2   = b.Q<Kirara.UI.UICurrencyArea>(9, "UICurrencyArea2");
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


            UICurrencyArea1.Set(1);
            UICurrencyArea2.Set(2);

            _selectedWeapon.Add(value => UIWeaponDetail.Set(value));
            _selectedDisc.Add(value => UIDiscDetail.Set(value));

            SetItems();
            SetTab();
        }

        public override void PlayEnter()
        {
            CanvasGroup.alpha = 0f;
            CanvasGroup.DOFade(1f, 0.1f).OnComplete(base.PlayEnter);
        }

        public override void PlayExit()
        {
            CanvasGroup.DOFade(0f, 0.1f).OnComplete(base.PlayExit);
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