using Kirara.Model;

namespace Kirara.UI.Panel
{
    public class SelectEquipmentPanel : BasePanel
    {
        #region View
        private bool _isBound;
        private UnityEngine.UI.Button             UIBackBtn;
        private Kirara.UI.UISelectEquipmentDisc   UISelectEquipmentDisc;
        private Kirara.UI.UISelectEquipmentWeapon UISelectEquipmentWeapon;
        public override void BindUI()
        {
            if (_isBound) return;
            _isBound = true;
            var c                   = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            UIBackBtn               = c.Q<UnityEngine.UI.Button>(0, "UIBackBtn");
            UISelectEquipmentDisc   = c.Q<Kirara.UI.UISelectEquipmentDisc>(1, "UISelectEquipmentDisc");
            UISelectEquipmentWeapon = c.Q<Kirara.UI.UISelectEquipmentWeapon>(2, "UISelectEquipmentWeapon");
        }
        #endregion

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