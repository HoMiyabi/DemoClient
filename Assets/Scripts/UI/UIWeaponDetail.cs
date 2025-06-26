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
        private TextMeshProUGUI NameText;
        private Image WearerIconImg;
        private TextMeshProUGUI LevelText;
        private Image BackIconImg;
        private Image IconImg;
        private UIItemStar UIItemStar;
        private TextMeshProUGUI EffContentText;
        private UIStatBar BaseStatBar;
        private UIStatBar AdvancedStatBar;

        private void InitUI()
        {
            var c = GetComponent<KiraraRuntimeComponents>();
            c.Init();
            NameText = c.Q<TextMeshProUGUI>("NameText");
            WearerIconImg = c.Q<Image>("WearerIconImg");
            LevelText = c.Q<TextMeshProUGUI>("LevelText");
            BackIconImg = c.Q<Image>("BackIconImg");
            IconImg = c.Q<Image>("IconImg");
            UIItemStar = c.Q<UIItemStar>("UIItemStar");
            EffContentText = c.Q<TextMeshProUGUI>("EffContentText");
            BaseStatBar = c.Q<UIStatBar>("BaseStatBar");
            AdvancedStatBar = c.Q<UIStatBar>("AdvancedStatBar");
        }

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

            EffContentText.text = weapon.PassiveDesc;
            return this;
        }
    }
}