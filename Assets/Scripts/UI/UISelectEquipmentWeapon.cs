using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Kirara.Model;
using Kirara.Service;
using Kirara.System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kirara.UI
{
    public class UISelectEquipmentWeapon : MonoBehaviour, LoopScrollDataSource
    {
        private UIWeaponDetail UIWeaponDetail;
        private Button EquipBtn;
        private LoopVerticalScrollRect LoopScroll;
        private TextMeshProUGUI EquipBtnText;

        private void InitUI()
        {
            var c = GetComponent<KiraraRuntimeComponents>();
            c.Init();
            UIWeaponDetail = c.Q<UIWeaponDetail>("UIWeaponDetail");
            EquipBtn = c.Q<Button>("EquipBtn");
            LoopScroll = c.Q<LoopVerticalScrollRect>("LoopScroll");
            EquipBtnText = c.Q<TextMeshProUGUI>("EquipBtnText");
        }

        private void Awake()
        {
            InitUI();

            LoopScroll.prefabSource = new LoopScrollPool(UIInventoryCellWeaponPrefab, transform);
            LoopScroll.dataSource = this;
        }

        [SerializeField] private GameObject UIInventoryCellWeaponPrefab;

        private Role ch;
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
            if (ch != null)
            {
                ch.OnWeaponChanged -= UpdateEquipBtnView;
            }
        }

        public void Set(Role ch)
        {
            Clear();
            this.ch = ch;

            ch.OnWeaponChanged += UpdateEquipBtnView;

            weapons = GetWeapons();

            LoopScroll.totalCount = weapons.Count;
            LoopScroll.RefillCells();

            if (weapons.Count > 0)
            {
                SelectedWeapon = weapons[0];
            }
        }

        private List<WeaponItem> GetWeapons()
        {
            var l = PlayerService.Player.Weapons.ToList();
            if (l.Count > 0 && ch.Weapon != null)
            {
                int idx = l.FindIndex(item => item.Id == ch.Weapon.Id);
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
            if (ch.Weapon == null && SelectedWeapon.RoleId == null)
            {
                SetEquipBtnEquip();
            }
            else if (ch.Weapon != null && SelectedWeapon.RoleId == ch.Id)
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
            EquipBtn.onClick.AddListener(() => ch.RemoveWeapon().Forget());
        }

        private void SetEquipBtnEquip()
        {
            EquipBtnText.text = "装备";
            EquipBtn.interactable = true;
            EquipBtn.onClick.RemoveAllListeners();
            EquipBtn.onClick.AddListener(() => ch.EquipWeapon(SelectedWeapon).Forget());
        }

        #endregion

        public void ProvideData(Transform tra, int idx)
        {
            // todo))
            // tra.GetComponent<UIInventoryCellWeapon>().Set(weapons[idx], () =>
            // {
            //     SelectedWeapon = weapons[idx];
            // });
        }
    }
}