﻿using System.Linq;
using Kirara.Model;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using YooAsset;

namespace Kirara.UI
{
    public class UIInventoryCellWeapon : MonoBehaviour, ISelectItem
    {
        #region View
        private TextMeshProUGUI    InfoText;
        private UIItemStar         UIItemStar;
        private Image              LockedImg;
        private Image              WearerIconImg;
        private Image              IconImg;
        private Button             Btn;
        private UIInventoryRankBar UIInventoryRankBar;
        private Image              SelectBorder;
        private void InitUI()
        {
            var c              = GetComponent<KiraraDirectBinder>();
            InfoText           = c.Q<TextMeshProUGUI>(0, "InfoText");
            UIItemStar         = c.Q<UIItemStar>(1, "UIItemStar");
            LockedImg          = c.Q<Image>(2, "LockedImg");
            WearerIconImg      = c.Q<Image>(3, "WearerIconImg");
            IconImg            = c.Q<Image>(4, "IconImg");
            Btn                = c.Q<Button>(5, "Btn");
            UIInventoryRankBar = c.Q<UIInventoryRankBar>(6, "UIInventoryRankBar");
            SelectBorder       = c.Q<Image>(7, "SelectBorder");
        }
        #endregion

        private AssetHandle itemIconHandle;
        private AssetHandle wearerIconHandle;
        private AssetHandle posIconHandle;
        private SelectController selectController;

        private void Awake()
        {
            InitUI();
        }

        private void OnDestroy()
        {
            Clear();
        }

        public void Clear()
        {
            if (_weapon != null)
            {
                _weapon.OnRoleIdChanged -= SetRole;
            }

            itemIconHandle?.Release();
            itemIconHandle = null;
            wearerIconHandle?.Release();
            wearerIconHandle = null;
            posIconHandle?.Release();
            posIconHandle = null;
        }

        private WeaponItem _weapon;
        public WeaponItem Weapon
        {
            get => _weapon;
            set
            {
                if (_weapon == value) return;
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

        public UIInventoryCellWeapon Set(WeaponItem weapon, UnityAction onClick)
        {
            Weapon = weapon;
            Btn.onClick.RemoveAllListeners();
            Btn.onClick.AddListener(onClick);
            return this;
        }

        private void SetRole(string roleId)
        {
            if (roleId == null)
            {
                WearerIconImg.sprite = null;
                WearerIconImg.gameObject.SetActive(false);
                return;
            }
            WearerIconImg.gameObject.SetActive(true);
            var chModel = PlayerService.Player.Roles.First(it => it.Id == roleId);
            wearerIconHandle = AssetMgr.Instance.package.LoadAssetSync<Sprite>(chModel.Config.IconLoc);
            WearerIconImg.sprite = wearerIconHandle.AssetObject as Sprite;
        }

        private void SetLocked(bool locked)
        {
            LockedImg.gameObject.SetActive(locked);
        }

        private void SetIcon(string iconLocation)
        {
            itemIconHandle = AssetMgr.Instance.package.LoadAssetSync<Sprite>(iconLocation);
            IconImg.sprite = itemIconHandle.AssetObject as Sprite;
        }

        private void SetLevelText(int level)
        {
            InfoText.text = $"等级{level}";
        }

        public void OnSelect()
        {
            SelectBorder.gameObject.SetActive(true);
        }

        public void OnDeselect()
        {
            SelectBorder.gameObject.SetActive(false);
        }
    }
}