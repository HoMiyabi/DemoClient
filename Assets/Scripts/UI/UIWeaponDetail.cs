using Kirara.Model;
using Manager;
using UnityEngine;
using YooAsset;

namespace Kirara.UI
{
    public class UIWeaponDetail : MonoBehaviour
    {
        #region View
        private bool _isBound;
        private TMPro.TextMeshProUGUI NameText;
        private UnityEngine.UI.Image  WearerIconImg;
        private TMPro.TextMeshProUGUI LevelText;
        private UnityEngine.UI.Image  BackIconImg;
        private UnityEngine.UI.Image  IconImg;
        private Kirara.UI.UIItemStar  UIItemStar;
        private TMPro.TextMeshProUGUI EffContentText;
        private Kirara.UI.UIStatBar   BaseStatBar;
        private Kirara.UI.UIStatBar   AdvancedStatBar;
        public void BindUI()
        {
            if (_isBound) return;
            _isBound = true;
            var c           = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            NameText        = c.Q<TMPro.TextMeshProUGUI>(0, "NameText");
            WearerIconImg   = c.Q<UnityEngine.UI.Image>(1, "WearerIconImg");
            LevelText       = c.Q<TMPro.TextMeshProUGUI>(2, "LevelText");
            BackIconImg     = c.Q<UnityEngine.UI.Image>(3, "BackIconImg");
            IconImg         = c.Q<UnityEngine.UI.Image>(4, "IconImg");
            UIItemStar      = c.Q<Kirara.UI.UIItemStar>(5, "UIItemStar");
            EffContentText  = c.Q<TMPro.TextMeshProUGUI>(6, "EffContentText");
            BaseStatBar     = c.Q<Kirara.UI.UIStatBar>(7, "BaseStatBar");
            AdvancedStatBar = c.Q<Kirara.UI.UIStatBar>(8, "AdvancedStatBar");
        }
        #endregion

        private void OnDestroy()
        {
            Clear();
        }

        private void Clear()
        {
        }

        public UIWeaponDetail Set(WeaponItem weapon)
        {
            BindUI();

            Clear();

            NameText.text = weapon.Name;
            // todo)) wearer icon

            LevelText.text = $"等级{weapon.Level}/{WeaponItem.MaxLevel}";

            var iconHandle = YooAssets.LoadAssetSync<Sprite>(weapon.IconLoc);
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