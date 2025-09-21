using DG.Tweening;
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
        private UnityEngine.CanvasGroup           CanvasGroup;
        public override void BindUI()
        {
            if (_isBound) return;
            _isBound = true;
            var b                   = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            UIBackBtn               = b.Q<UnityEngine.UI.Button>(0, "UIBackBtn");
            UISelectEquipmentDisc   = b.Q<Kirara.UI.UISelectEquipmentDisc>(1, "UISelectEquipmentDisc");
            UISelectEquipmentWeapon = b.Q<Kirara.UI.UISelectEquipmentWeapon>(2, "UISelectEquipmentWeapon");
            CanvasGroup             = b.Q<UnityEngine.CanvasGroup>(3, "CanvasGroup");
        }
        #endregion

        private Role ch;
        private UIRoleEquipment equipment;

        private void Start()
        {
            UIBackBtn.onClick.AddListener(() => UIMgr.Instance.PopPanel(this));
        }

        private void OnDestroy()
        {
            equipment.ResetParentAndOnClick();
        }

        public void SetDisc(Role ch, UIRoleEquipment equipment, int pos)
        {
            Set(ch, equipment);
            SetDisc(pos);
        }

        public void SetWeapon(Role ch, UIRoleEquipment equipment)
        {
            Set(ch, equipment);
            SetWeapon();
        }

        private void Set(Role ch, UIRoleEquipment equipment)
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

        public override void PlayEnter()
        {
            CanvasGroup.alpha = 0f;
            CanvasGroup.DOFade(1f, 0.1f).OnComplete(base.PlayEnter);
        }

        public override void PlayExit()
        {
            CanvasGroup.DOFade(0f, 0.1f).OnComplete(base.PlayExit);
        }
    }
}