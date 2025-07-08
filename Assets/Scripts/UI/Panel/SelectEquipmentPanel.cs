using System;
using Kirara.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Kirara.UI.Panel
{
    public class SelectEquipmentPanel : BasePanel
    {
        #region View
        private Button                  UIBackBtn;
        private UISelectEquipmentDisc   UISelectEquipmentDisc;
        private UISelectEquipmentWeapon UISelectEquipmentWeapon;
        private void InitUI()
        {
            var c                   = GetComponent<KiraraRuntimeComponents>();
            UIBackBtn               = c.Q<Button>(0, "UIBackBtn");
            UISelectEquipmentDisc   = c.Q<UISelectEquipmentDisc>(1, "UISelectEquipmentDisc");
            UISelectEquipmentWeapon = c.Q<UISelectEquipmentWeapon>(2, "UISelectEquipmentWeapon");
        }
        #endregion

        private void Awake()
        {
            InitUI();
        }

        private Role ch;
        private UICharacterEquipment equipment;

        private void Start()
        {
            UIBackBtn.onClick.AddListener(() => UIMgr.Instance.PopPanel(this));
        }

        private void OnDestroy()
        {
            equipment.ResetParentAndOnClick();
        }

        public void SetDisc(Role ch, UICharacterEquipment equipment, int pos)
        {
            Set(ch, equipment);
            SetDisc(pos);
        }

        public void SetWeapon(Role ch, UICharacterEquipment equipment)
        {
            Set(ch, equipment);
            SetWeapon();
        }

        private void Set(Role ch, UICharacterEquipment equipment)
        {
            this.ch = ch;
            this.equipment = equipment;

            for (int pos = 1; pos <= equipment.SlotCount; pos++)
            {
                equipment.Slot(pos).OnClick = SetDisc;
            }
            equipment.WeaponOnClick = SetWeapon;
            equipment.transform.SetParent(transform);
        }

        private void SetDisc(int pos)
        {
            UISelectEquipmentDisc.gameObject.SetActive(true);
            UISelectEquipmentWeapon.gameObject.SetActive(false);

            UISelectEquipmentDisc.Set(ch, pos);
        }

        private void SetWeapon()
        {
            UISelectEquipmentDisc.gameObject.SetActive(false);
            UISelectEquipmentWeapon.gameObject.SetActive(true);

            UISelectEquipmentWeapon.Set(ch);
        }
    }
}