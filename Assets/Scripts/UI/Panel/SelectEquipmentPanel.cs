using System;
using Kirara.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Kirara.UI.Panel
{
    public class SelectEquipmentPanel : BasePanel
    {
        private Button UIBackBtn;
        private UISelectEquipmentDisc UISelectEquipmentDisc;
        private UISelectEquipmentWeapon UISelectEquipmentWeapon;

        private void InitUI()
        {
            var c = GetComponent<KiraraRuntimeComponents>();
            c.Init();
            UIBackBtn = c.Q<Button>("UIBackBtn");
            UISelectEquipmentDisc = c.Q<UISelectEquipmentDisc>("UISelectEquipmentDisc");
            UISelectEquipmentWeapon = c.Q<UISelectEquipmentWeapon>("UISelectEquipmentWeapon");
        }

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