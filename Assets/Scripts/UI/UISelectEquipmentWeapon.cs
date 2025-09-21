using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Kirara.Model;
using UnityEngine;

namespace Kirara.UI
{
    public class UISelectEquipmentWeapon : MonoBehaviour
    {
        #region View
        private bool _isBound;
        private Kirara.UI.UIWeaponDetail        UIWeaponDetail;
        private UnityEngine.UI.Button           EquipBtn;
        private TMPro.TextMeshProUGUI           EquipBtnText;
        private KiraraLoopScroll.GridScrollView ScrollView;
        public void BindUI()
        {
            if (_isBound) return;
            _isBound = true;
            var b          = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            UIWeaponDetail = b.Q<Kirara.UI.UIWeaponDetail>(0, "UIWeaponDetail");
            EquipBtn       = b.Q<UnityEngine.UI.Button>(1, "EquipBtn");
            EquipBtnText   = b.Q<TMPro.TextMeshProUGUI>(2, "EquipBtnText");
            ScrollView     = b.Q<KiraraLoopScroll.GridScrollView>(3, "ScrollView");
        }
        #endregion

        [SerializeField] private GameObject UIInventoryCellWeaponPrefab;

        private Role Role { get; set; }
        private List<WeaponItem> weapons;
        private readonly LiveData<WeaponItem> _selectedWeapon = new(null);

        private void Awake()
        {
            BindUI();

            var pool = new LoopScrollGOPool(UIInventoryCellWeaponPrefab, transform);
            ScrollView.SetSource(pool);
            ScrollView.provideData = ProvideData;
            _selectedWeapon.Observe(OnSelectionChanged);
        }

        private void OnSelectionChanged(WeaponItem weapon)
        {
            if (weapon == null) return;
            UpdateEquipBtnView();
            UIWeaponDetail.Set(weapon);
        }

        private void OnDestroy()
        {
            Clear();
        }

        private void Clear()
        {
            if (Role != null)
            {
                Role.OnWeaponChanged -= UpdateEquipBtnView;
            }
            Role = null;
        }

        public void Set(Role role)
        {
            Clear();
            Role = role;

            role.OnWeaponChanged += UpdateEquipBtnView;

            weapons = GetWeapons();

            ScrollView._totalCount = weapons.Count;
            ScrollView.RefreshToStart();

            if (weapons.Count > 0)
            {
                _selectedWeapon.Value = weapons[0];
            }
        }

        private List<WeaponItem> GetWeapons()
        {
            var l = PlayerService.Player.Weapons.ToList();
            if (l.Count > 0 && Role.Weapon != null)
            {
                int idx = l.FindIndex(item => item.Id == Role.Weapon.Id);
                if (idx != -1)
                {
                    (l[0], l[idx]) = (l[idx], l[0]);
                }
            }
            return l;
        }

        #region Equip Btn

        private void UpdateEquipBtnView()
        {
            if (Role.Weapon == null && _selectedWeapon.Value.RoleId == "")
            {
                SetEquipBtnEquip();
            }
            else if (Role.Weapon != null && _selectedWeapon.Value.RoleId == Role.Id)
            {
                SetEquipBtnRemove();
            }
            else
            {
                SetEquipBtnNull();
            }
        }

        private void SetEquipBtnNull()
        {
            EquipBtnText.text = "";
            EquipBtn.interactable = false;
        }

        private void SetEquipBtnRemove()
        {
            EquipBtnText.text = "卸下";
            EquipBtn.interactable = true;
            EquipBtn.onClick.RemoveAllListeners();
            EquipBtn.onClick.AddListener(() => Role.RemoveWeapon().Forget());
        }

        private void SetEquipBtnEquip()
        {
            EquipBtnText.text = "装备";
            EquipBtn.interactable = true;
            EquipBtn.onClick.RemoveAllListeners();
            EquipBtn.onClick.AddListener(() => Role.EquipWeapon(_selectedWeapon.Value).Forget());
        }

        #endregion

        private void ProvideData(GameObject go, int index)
        {
            var item = go.GetComponent<UIInventoryCellWeapon>();
            item.Set(weapons[index], _selectedWeapon);
        }
    }
}