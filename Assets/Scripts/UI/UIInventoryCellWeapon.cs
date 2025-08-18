using System.Linq;
using Kirara.Model;
using Manager;
using UnityEngine;
using UnityEngine.Events;

namespace Kirara.UI
{
    public class UIInventoryCellWeapon : MonoBehaviour
    {
        #region View
        private bool _isBound;
        private TMPro.TextMeshProUGUI        InfoText;
        private Kirara.UI.UIItemStar         UIItemStar;
        private UnityEngine.UI.Image         LockedImg;
        private UnityEngine.UI.Image         WearerIconImg;
        private UnityEngine.UI.Image         IconImg;
        private UnityEngine.UI.Button        Btn;
        private Kirara.UI.UIInventoryRankBar UIInventoryRankBar;
        private UnityEngine.UI.Image         SelectBorder;
        public void BindUI()
        {
            if (_isBound) return;
            _isBound = true;
            var c              = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            InfoText           = c.Q<TMPro.TextMeshProUGUI>(0, "InfoText");
            UIItemStar         = c.Q<Kirara.UI.UIItemStar>(1, "UIItemStar");
            LockedImg          = c.Q<UnityEngine.UI.Image>(2, "LockedImg");
            WearerIconImg      = c.Q<UnityEngine.UI.Image>(3, "WearerIconImg");
            IconImg            = c.Q<UnityEngine.UI.Image>(4, "IconImg");
            Btn                = c.Q<UnityEngine.UI.Button>(5, "Btn");
            UIInventoryRankBar = c.Q<Kirara.UI.UIInventoryRankBar>(6, "UIInventoryRankBar");
            SelectBorder       = c.Q<UnityEngine.UI.Image>(7, "SelectBorder");
        }
        #endregion

        private LiveData<WeaponItem> _selected;

        private void OnDestroy()
        {
            Clear();
        }

        private void Clear()
        {
            if (_weapon != null)
            {
                _weapon.OnRoleIdChanged -= SetRole;
                _weapon = null;
            }
            _selected?.Remove(OnSelectionChanged);
            _weapon = null;
        }

        private WeaponItem _weapon;
        public WeaponItem Weapon
        {
            get => _weapon;
            set
            {
                Clear();
                _weapon = value;
                _weapon.OnRoleIdChanged += SetRole;

                SetIcon(_weapon.IconLoc);
                UIInventoryRankBar.Set(_weapon.Config.Rank);
                SetLevelText(_weapon.Level);
                SetLocked(_weapon.Locked);
                UIItemStar.SetStar(_weapon.RefineLevel);
                SetRole(_weapon.RoleId);
            }
        }

        public UIInventoryCellWeapon Set(WeaponItem weapon, LiveData<WeaponItem> selected)
        {
            BindUI();
            Clear();

            Weapon = weapon;
            _selected = selected;
            _selected.Observe(OnSelectionChanged);
            Btn.onClick.RemoveAllListeners();
            Btn.onClick.AddListener(Btn_onClick);
            return this;
        }

        private void SetRole(string roleId)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                WearerIconImg.sprite = null;
                WearerIconImg.gameObject.SetActive(false);
            }
            else
            {
                WearerIconImg.gameObject.SetActive(true);
                var role = PlayerService.Player.Roles.First(it => it.Id == roleId);
                var wearerIconHandle = AssetMgr.Instance.package.LoadAssetSync<Sprite>(role.Config.IconLoc);
                WearerIconImg.sprite = wearerIconHandle.AssetObject as Sprite;
            }
        }

        private void SetLocked(bool locked)
        {
            LockedImg.gameObject.SetActive(locked);
        }

        private void SetIcon(string iconLocation)
        {
            var itemIconHandle = AssetMgr.Instance.package.LoadAssetSync<Sprite>(iconLocation);
            IconImg.sprite = itemIconHandle.AssetObject as Sprite;
        }

        private void SetLevelText(int level)
        {
            InfoText.text = $"等级{level}";
        }

        private void OnSelectionChanged(WeaponItem weapon)
        {
            if (weapon == _weapon)
            {
                SelectBorder.gameObject.SetActive(true);
            }
            else
            {
                SelectBorder.gameObject.SetActive(false);
            }
        }

        private void Btn_onClick()
        {
            _selected?.Set(_weapon);
        }
    }
}