using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Kirara.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kirara.UI
{
    public class UISelectEquipmentWeapon : MonoBehaviour
    {
        #region View
        private UIWeaponDetail  UIWeaponDetail;
        private Button          EquipBtn;
        private TextMeshProUGUI EquipBtnText;
        private GridScroller    Scroller;
        private void InitUI()
        {
            var c          = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            UIWeaponDetail = c.Q<UIWeaponDetail>(0, "UIWeaponDetail");
            EquipBtn       = c.Q<Button>(1, "EquipBtn");
            EquipBtnText   = c.Q<TextMeshProUGUI>(2, "EquipBtnText");
            Scroller       = c.Q<GridScroller>(3, "Scroller");
        }
        #endregion

        [SerializeField] private GameObject UIInventoryCellWeaponPrefab;
        private LoopScrollGOPool Pool { get; set; }

        private void Awake()
        {
            InitUI();

            Pool = new LoopScrollGOPool(UIInventoryCellWeaponPrefab, transform);
            Scroller.SetPoolFunc(Pool.GetObject, Pool.ReturnObject);
            Scroller.provideData = ProvideData;
        }

        private Role Role { get; set; }
        private List<WeaponItem> weapons;
        private WeaponItem _selectedWeapon;
        private WeaponItem SelectedWeapon
        {
            get => _selectedWeapon;
            set
            {
                if (_selectedWeapon == value) return;
                _selectedWeapon = value;
                UpdateEquipBtnView();
                UIWeaponDetail.Set(value);
            }
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
        }

        public void Set(Role role)
        {
            Clear();
            Role = role;

            role.OnWeaponChanged += UpdateEquipBtnView;

            weapons = GetWeapons();

            Scroller.totalCount = weapons.Count;

            if (weapons.Count > 0)
            {
                SelectedWeapon = weapons[0];
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
            if (Role.Weapon == null && SelectedWeapon.RoleId == "")
            {
                SetEquipBtnEquip();
            }
            else if (Role.Weapon != null && SelectedWeapon.RoleId == Role.Id)
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
            EquipBtn.onClick.AddListener(() => Role.EquipWeapon(SelectedWeapon).Forget());
        }

        #endregion

        private void ProvideData(GameObject go, int idx)
        {
            go.GetComponent<UIInventoryCellWeapon>().Set(weapons[idx], () =>
            {
                SelectedWeapon = weapons[idx];
            });
        }
    }
}