using Kirara.Model;
using Kirara.UI.Panel;
using UnityEngine;
using UnityEngine.Events;

namespace Kirara.UI
{
    public class UICharacterEquipment : MonoBehaviour
    {
        #region View
        private UIWeaponSlot UIWeaponSlot;
        private DiscSlot     UIDiscSlot;
        private DiscSlot     UIDiscSlot1;
        private DiscSlot     UIDiscSlot2;
        private DiscSlot     UIDiscSlot3;
        private DiscSlot     UIDiscSlot4;
        private DiscSlot     UIDiscSlot5;
        private void InitUI()
        {
            var c        = GetComponent<KiraraDirectBinder>();
            UIWeaponSlot = c.Q<UIWeaponSlot>(0, "UIWeaponSlot");
            UIDiscSlot   = c.Q<DiscSlot>(1, "UIDiscSlot");
            UIDiscSlot1  = c.Q<DiscSlot>(2, "UIDiscSlot1");
            UIDiscSlot2  = c.Q<DiscSlot>(3, "UIDiscSlot2");
            UIDiscSlot3  = c.Q<DiscSlot>(4, "UIDiscSlot3");
            UIDiscSlot4  = c.Q<DiscSlot>(5, "UIDiscSlot4");
            UIDiscSlot5  = c.Q<DiscSlot>(6, "UIDiscSlot5");
        }
        #endregion

        private DiscSlot[] discSlots;
        public int SlotCount => discSlots.Length;

        private Role ch;

        private Transform parent;

        public UnityAction WeaponOnClick
        {
            set => UIWeaponSlot.OnClick = value;
        }

        private void Awake()
        {
            InitUI();
            discSlots = new[] { UIDiscSlot, UIDiscSlot1, UIDiscSlot2, UIDiscSlot3, UIDiscSlot4, UIDiscSlot5 };
            parent = transform.parent;
        }

        public DiscSlot Slot(int pos)
        {
            return discSlots[pos - 1];
        }

        public void Set(Role ch)
        {
            this.ch = ch;
            for (int pos = 1; pos <= SlotCount; pos++)
            {
                Slot(pos).Set(ch, pos, OpenSelectDisc);
            }
            UIWeaponSlot.Set(ch, OpenSelectWeapon);
        }

        public void ResetParentAndOnClick()
        {
            for (int pos = 1; pos <= SlotCount; pos++)
            {
                Slot(pos).OnClick = OpenSelectDisc;
            }
            UIWeaponSlot.OnClick = OpenSelectWeapon;

            transform.SetParent(parent);
        }

        private void OpenSelectDisc(int discPos)
        {
            UIMgr.Instance.PushPanel<SelectEquipmentPanel>().SetDisc(ch, this, discPos);
        }

        private void OpenSelectWeapon()
        {
            UIMgr.Instance.PushPanel<SelectEquipmentPanel>().SetWeapon(ch, this);
        }
    }
}