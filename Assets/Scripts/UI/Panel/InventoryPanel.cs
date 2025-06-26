using System.Collections.Generic;
using System.Linq;
using Kirara.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kirara.UI.Panel
{
    public class InventoryPanel : BasePanel
    {
        #region View
        private Button UIBackBtn;
        private TextMeshProUGUI CoinText;
        private Image CoinIconImg;
        private TextMeshProUGUI InventoryNameText;
        private UIDiscDetail UIDiscDetail;
        private UITabController UITabController;
        private UIWeaponDetail UIWeaponDetail;
        private GridScroller WeaponLoopScroll;
        private GridScroller DiscLoopScroll;
        private void InitUI()
        {
            var c = GetComponent<KiraraRuntimeComponents>();
            c.Init();
            UIBackBtn = c.Q<Button>("UIBackBtn");
            CoinText = c.Q<TextMeshProUGUI>("CoinText");
            CoinIconImg = c.Q<Image>("CoinIconImg");
            InventoryNameText = c.Q<TextMeshProUGUI>("InventoryNameText");
            UIDiscDetail = c.Q<UIDiscDetail>("UIDiscDetail");
            UITabController = c.Q<UITabController>("UITabController");
            UIWeaponDetail = c.Q<UIWeaponDetail>("UIWeaponDetail");
            WeaponLoopScroll = c.Q<GridScroller>("WeaponLoopScroll");
            DiscLoopScroll = c.Q<GridScroller>("DiscLoopScroll");
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
                if (WeaponLoopScroll.cells.TryGetValue(_weaponIdx, out var r))
                {
                    var cell = r.GetComponent<UIInventoryCellWeapon>();
                    cell.OnDeselect();
                }

                _weaponIdx = value;

                UIWeaponDetail.GetComponent<UIWeaponDetail>().Set(weapons.ElementAtOrDefault(_weaponIdx));
                if (WeaponLoopScroll.cells.TryGetValue(_weaponIdx, out r))
                {
                    var cell = r.GetComponent<UIInventoryCellWeapon>();
                    cell.OnSelect();
                }
            }
        }

        private List<DiscItem> discs = new();
        private int _discIdx = -1;

        private int DiscIdx
        {
            get => _discIdx;
            set
            {
                if (_discIdx == value) return;
                if (DiscLoopScroll.cells.TryGetValue(_discIdx, out var r))
                {
                    var cell = r.GetComponent<UIInventoryCellDisc>();
                    cell.OnDeselect();
                }

                _discIdx = value;

                UIDiscDetail.Set(discs.ElementAtOrDefault(_discIdx));
                if (DiscLoopScroll.cells.TryGetValue(_discIdx, out r))
                {
                    var cell = r.GetComponent<UIInventoryCellDisc>();
                    cell.OnSelect();
                }
            }
        }

        private LoopScrollGOPool weaponPool;
        private LoopScrollGOPool discPool;

        private void Awake()
        {
            InitUI();
            UIBackBtn.onClick.AddListener(() => UIMgr.Instance.PopPanel(this));
            pool = new Stack<Transform>();

            CoinText.text = PlayerService.player.currencies.First(it => it.Cid == 1).Count.ToString();

            SetItems();
            SetTab();
        }

        private void SetItems()
        {
            weapons = PlayerService.player.weapons;

            weaponPool = new LoopScrollGOPool(UIInventoryCellWeaponPrefab, transform);
            WeaponLoopScroll.SetPoolFunc(weaponPool.GetObject, weaponPool.ReturnObject);
            WeaponLoopScroll.provideData = ProvideWeaponData;
            WeaponLoopScroll.totalCount = weapons.Count;

            discs = PlayerService.player.discs;

            discPool = new LoopScrollGOPool(UIInventoryCellDiscPrefab, transform);
            DiscLoopScroll.SetPoolFunc(discPool.GetObject, discPool.ReturnObject);
            DiscLoopScroll.provideData = ProvideDiscData;
            DiscLoopScroll.totalCount = discs.Count;
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