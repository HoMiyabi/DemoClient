using System.Collections.Generic;
using System.Linq;
using Kirara.Model;
using Kirara.UI.Panel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kirara.UI
{
    public class UISelectEquipmentDisc : MonoBehaviour, LoopScrollDataSource
    {
        private UIDiscDetail UIDiscDetail;
        private Button UpgradeBtn;
        private Button EquipBtn;
        private LoopVerticalScrollRect LoopScroll;
        private TextMeshProUGUI EquipBtnText;

        private void InitUI()
        {
            var c = GetComponent<KiraraRuntimeComponents>();
            c.Init();
            UIDiscDetail = c.Q<UIDiscDetail>("UIDiscDetail");
            UpgradeBtn = c.Q<Button>("UpgradeBtn");
            EquipBtn = c.Q<Button>("EquipBtn");
            LoopScroll = c.Q<LoopVerticalScrollRect>("LoopScroll");
            EquipBtnText = c.Q<TextMeshProUGUI>("EquipBtnText");
        }

        private void Awake()
        {
            InitUI();

            UpgradeBtn.onClick.AddListener(() =>
            {
                UIMgr.Instance.PushPanel<UpgradeDiscDialogPanel>().Set(SelectedDisc);
            });

            LoopScroll.prefabSource = new LoopScrollPool(UIInventoryCellDiscPrefab, transform);
            LoopScroll.dataSource = this;
        }

        [SerializeField] private GameObject UIInventoryCellDiscPrefab;

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

        private Role _role;

        private void OnDestroy()
        {
            Clear();
        }

        private void Clear()
        {
            if (_role != null)
            {
                _role.OnDiscChanged -= OnRoleDiscChanged;
            }
        }

        public void Set(Role role, int pos)
        {
            Clear();
            _role = role;
            _pos = pos;

            _role.OnDiscChanged += OnRoleDiscChanged;

            discs = PlayerService.Player.Discs
                .Where(it => it.Pos == pos)
                .ToList();
            ReorderDisc();

            LoopScroll.totalCount = discs.Count;
            LoopScroll.RefillCells();

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
            if (_role.Disc(_pos) != null)
            {
                int idx = discs.FindIndex(item => item.Id == _role.Disc(_pos).Id);
                (discs[0], discs[idx]) = (discs[idx], discs[0]);
            }
        }

        #region Equip Btn

        private void UpdateEquipBtn()
        {
            if (_role.Disc(_pos) == null && SelectedDisc.RoleId == null)
            {
                EquipBtnSwitchEquip();
            }
            else if (_role.Disc(_pos) != null && SelectedDisc.RoleId == _role.Id)
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
            EquipBtn.onClick.AddListener(() => _role.RemoveDisc(_pos).Forget());
        }

        private void EquipBtnSwitchEquip()
        {
            EquipBtnText.text = "装备";
            EquipBtn.interactable = true;
            EquipBtn.onClick.RemoveAllListeners();
            EquipBtn.onClick.AddListener(() => _role.EquipDisc(_pos, SelectedDisc).Forget());
        }

        #endregion

        public void ProvideData(Transform tra, int idx)
        {
            // todo))
            // tra.GetComponent<UIInventoryCellDisc>().Set(discs[idx], () =>
            // {
            //     SelectedDisc = discs[idx];
            // });
        }
    }
}