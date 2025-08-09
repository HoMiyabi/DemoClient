using System;

using Kirara.Model;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YooAsset;

namespace Kirara.UI
{
    public class UIWeaponDetail : MonoBehaviour
    {
        #region View
        private TextMeshProUGUI NameText;
        private Image           WearerIconImg;
        private TextMeshProUGUI LevelText;
        private Image           BackIconImg;
        private Image           IconImg;
        private UIItemStar      UIItemStar;
        private TextMeshProUGUI EffContentText;
        private UIStatBar       BaseStatBar;
        private UIStatBar       AdvancedStatBar;
        private void InitUI()
        {
            var c           = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            NameText        = c.Q<TextMeshProUGUI>(0, "NameText");
            WearerIconImg   = c.Q<Image>(1, "WearerIconImg");
            LevelText       = c.Q<TextMeshProUGUI>(2, "LevelText");
            BackIconImg     = c.Q<Image>(3, "BackIconImg");
            IconImg         = c.Q<Image>(4, "IconImg");
            UIItemStar      = c.Q<UIItemStar>(5, "UIItemStar");
            EffContentText  = c.Q<TextMeshProUGUI>(6, "EffContentText");
            BaseStatBar     = c.Q<UIStatBar>(7, "BaseStatBar");
            AdvancedStatBar = c.Q<UIStatBar>(8, "AdvancedStatBar");
        }
        #endregion

        private AssetHandle iconHandle;

        private void Awake()
        {
            InitUI();
        }

        private void OnDestroy()
        {
            Clear();
        }

        public UIWeaponDetail Clear()
        {
            iconHandle?.Release();
            iconHandle = null;
            return this;
        }

        public UIWeaponDetail Set(WeaponItem weapon)
        {
            Clear();

            NameText.text = weapon.Name;
            // todo)) wearer icon

            LevelText.text = $"等级{weapon.Level}/{WeaponItem.MaxLevel}";

            iconHandle = AssetMgr.Instance.package.LoadAssetSync<Sprite>(weapon.IconLoc);
            var sprite = iconHandle.AssetObject as Sprite;

            BackIconImg.sprite = sprite;
            IconImg.sprite = sprite;
            UIItemStar.SetStar(weapon.RefineLevel);

            BaseStatBar.Set(weapon.BaseAttr);
            AdvancedStatBar.Set(weapon.AdvancedAttr);

            EffContentText.text = weapon.Config.PassiveDesc;
            return this;
        }
    }
}