using System.Collections.Generic;
using System.Linq;
using Kirara.Model;
using Kirara.UI.Panel;
using UnityEngine;

namespace Kirara.UI
{
    public class UISelectEquipmentDisc : MonoBehaviour
    {
        #region View
        private bool _isBound;
        private Kirara.UI.UIDiscDetail UIDiscDetail;
        private UnityEngine.UI.Button  UpgradeBtn;
        private UnityEngine.UI.Button  EquipBtn;
        private TMPro.TextMeshProUGUI  EquipBtnText;
        private KiraraLoopScroll.GridScrollView         Scroller;
        public void BindUI()
        {
            if (_isBound) return;
            _isBound = true;
            var c        = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            UIDiscDetail = c.Q<Kirara.UI.UIDiscDetail>(0, "UIDiscDetail");
            UpgradeBtn   = c.Q<UnityEngine.UI.Button>(1, "UpgradeBtn");
            EquipBtn     = c.Q<UnityEngine.UI.Button>(2, "EquipBtn");
            EquipBtnText = c.Q<TMPro.TextMeshProUGUI>(3, "EquipBtnText");
            Scroller     = c.Q<KiraraLoopScroll.GridScrollView>(4, "Scroller");
        }
        #endregion

        [SerializeField] private GameObject UIInventoryCellDiscPrefab;
        private LoopScrollGOPool Pool { get; set; }

        private void Awake()
        {
            BindUI();

            UpgradeBtn.onClick.AddListener(() =>
            {
                UIMgr.Instance.PushPanel<UpgradeDiscDialogPanel>().Set(SelectedDisc);
            });

            Pool = new LoopScrollGOPool(UIInventoryCellDiscPrefab, transform);
            Scroller.SetGOSourceFunc(Pool.GetObject, Pool.ReturnObject);
            Scroller.provideData = ProvideData;
        }

        private List<DiscItem> discs;
        private int _pos;

        private DiscItem _selectedDisc;
        private DiscItem SelectedDisc
        {
            get => _selectedDisc;
            set
            {
                if (_selectedDisc == value) return;
                _selectedDisc = value;
                UIDiscDetail.Set(value);
                UpdateEquipBtn();
            }
        }

        private Role Role { get; set; }

        private void OnDestroy()
        {
            Clear();
        }

        private void Clear()
        {
            if (Role != null)
            {
                Role.OnDiscChanged -= OnRoleDiscChanged;
            }
        }

        public void Set(Role role, int pos)
        {
            Clear();
            Role = role;
            _pos = pos;

            Role.OnDiscChanged += OnRoleDiscChanged;

            discs = PlayerService.Player.Discs
                .Where(it => it.Pos == pos)
                .ToList();
            ReorderDisc();

            Scroller._totalCount = discs.Count;
            Scroller.Refresh();

            if (discs.Count > 0)
            {
                SelectedDisc = discs[0];
            }
        }

        private void OnRoleDiscChanged(int discPos)
        {
            if (discPos != _pos) return;
            UpdateEquipBtn();
        }

        private void ReorderDisc()
        {
            if (Role.Disc(_pos) != null)
            {
                int idx = discs.FindIndex(item => item.Id == Role.Disc(_pos).Id);
                (discs[0], discs[idx]) = (discs[idx], discs[0]);
            }
        }

        #region Equip Btn

        private void UpdateEquipBtn()
        {
            if (Role.Disc(_pos) == null && SelectedDisc.RoleId == "")
            {
                EquipBtnSwitchEquip();
            }
            else if (Role.Disc(_pos) != null && SelectedDisc.RoleId == Role.Id)
            {
                EquipBtnSwitchRemove();
            }
            else
            {
                EquipBtnSwitchNull();
            }
        }

        private void EquipBtnSwitchNull()
        {
            EquipBtnText.text = "";
            EquipBtn.interactable = false;
        }

        private void EquipBtnSwitchRemove()
        {
            EquipBtnText.text = "卸下";
            EquipBtn.interactable = true;
            EquipBtn.onClick.RemoveAllListeners();
            EquipBtn.onClick.AddListener(() => Role.RemoveDisc(_pos).Forget());
        }

        private void EquipBtnSwitchEquip()
        {
            EquipBtnText.text = "装备";
            EquipBtn.interactable = true;
            EquipBtn.onClick.RemoveAllListeners();
            EquipBtn.onClick.AddListener(() => Role.EquipDisc(_pos, SelectedDisc).Forget());
        }

        #endregion

        private void ProvideData(GameObject go, int idx)
        {
            go.GetComponent<UIInventoryCellDisc>().Set(discs[idx], () =>
            {
                SelectedDisc = discs[idx];
            });
        }
    }
}