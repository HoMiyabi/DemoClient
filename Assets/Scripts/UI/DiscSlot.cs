using System;
using Kirara.Model;
using Manager;
using UnityEngine;
using UnityEngine.UI;
using YooAsset;

namespace Kirara.UI
{
    public class DiscSlot : MonoBehaviour
    {
        #region View
        private Image  Img;
        private Button Btn;
        private void InitUI()
        {
            var c = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            Img   = c.Q<Image>(0, "Img");
            Btn   = c.Q<Button>(1, "Btn");
        }
        #endregion

        private void Awake()
        {
            InitUI();
        }

        private Role Role { get; set; }
        private int Pos { get; set; }
        private AssetHandle icon;

        private void OnDestroy()
        {
            Clear();
        }

        private void Clear()
        {
            if (Role != null)
            {
                Role.OnDiscChanged -= UpdateView;
                Role = null;
            }
            icon?.Release();
            icon = null;
        }

        public Action<int> OnClick
        {
            set
            {
                Btn.onClick.RemoveAllListeners();
                Btn.onClick.AddListener(() => value?.Invoke(Pos));
            }
        }

        private void UpdateView(int pos1)
        {
            if (pos1 != Pos) return;

            var disc = Role.Disc(Pos);

            if (disc != null)
            {
                icon = YooAssets.LoadAssetSync<Sprite>(disc.Config.EquipmentIconLoc);
                Img.sprite = icon.AssetObject as Sprite;
                Img.color = Color.white;
            }
            else
            {
                Img.sprite = null;
                Img.color = Color.clear;
            }
        }

        public DiscSlot Set(Role role, int pos, Action<int> onClick)
        {
            Clear();
            Role = role;
            Pos = pos;
            UpdateView(pos);
            role.OnDiscChanged += UpdateView;

            OnClick = onClick;
            return this;
        }
    }
}