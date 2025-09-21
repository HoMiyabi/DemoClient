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
        private Kirara.UI.UIDiscDetail          UIDiscDetail;
        private UnityEngine.UI.Button           UpgradeBtn;
        private UnityEngine.UI.Button           EquipBtn;
        private TMPro.TextMeshProUGUI           EquipBtnText;
        private KiraraLoopScroll.GridScrollView ScrollView;
        public void BindUI()
        {
            if (_isBound) return;
            _isBound = true;
            var b        = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            UIDiscDetail = b.Q<Kirara.UI.UIDiscDetail>(0, "UIDiscDetail");
            UpgradeBtn   = b.Q<UnityEngine.UI.Button>(1, "UpgradeBtn");
            EquipBtn     = b.Q<UnityEngine.UI.Button>(2, "EquipBtn");
            EquipBtnText = b.Q<TMPro.TextMeshProUGUI>(3, "EquipBtnText");
            ScrollView   = b.Q<KiraraLoopScroll.GridScrollView>(4, "ScrollView");
        }
        #endregion

        [SerializeField] private GameObject UIInventoryCellDiscPrefab;

        private Role Role { get; set; }

        private List<DiscItem> discs;
        private int _pos;

        private readonly LiveData<DiscItem> _selectedDisc = new(null);

        private void Awake()
        {
            BindUI();

            UpgradeBtn.onClick.AddListener(() =>
            {
                UIMgr.Instance.PushPanel<UpgradeDiscDialogPanel>().Set(_selectedDisc.Value);
            });

            var pool = new LoopScrollGOPool(UIInventoryCellDiscPrefab, transform);
            ScrollView.SetSource(pool);
            ScrollView.provideData = ProvideData;

            _selectedDisc.Observe(OnSelectionChanged);
        }

        private void OnSelectionChanged(DiscItem disc)
        {
            if (disc == null) return;
            UIDiscDetail.Set(disc);
            UpdateEquipBtn();
        }

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

            ScrollView._totalCount = discs.Count;
            ScrollView.RefreshToStart();

            if (discs.Count > 0)
            {
                _selectedDisc.Value = discs[0];
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
            if (Role.Disc(_pos) == null && _selectedDisc.Value.RoleId == "")
            {
                EquipBtnSwitchEquip();
            }
            else if (Role.Disc(_pos) != null && _selectedDisc.Value.RoleId == Role.Id)
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
            EquipBtn.onClick.AddListener(() => Role.EquipDisc(_pos, _selectedDisc.Value).Forget());
        }

        #endregion

        private void ProvideData(GameObject go, int idx)
        {
            go.GetComponent<UIInventoryCellDisc>().Set(discs[idx], _selectedDisc);
        }
    }
}