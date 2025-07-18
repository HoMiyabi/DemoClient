﻿using Kirara.Model;
using Manager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using YooAsset;

namespace Kirara.UI
{
    public class UIWeaponSlot : MonoBehaviour
    {
        #region View
        private Image  Img;
        private Button Btn;
        private void InitUI()
        {
            var c = GetComponent<KiraraDirectBinder>();
            Img   = c.Q<Image>(0, "Img");
            Btn   = c.Q<Button>(1, "Btn");
        }
        #endregion

        private void Awake()
        {
            InitUI();
        }

        private Role ch;
        private AssetHandle iconHandle;

        private void OnDestroy()
        {
            Clear();
        }

        private void Clear()
        {
            if (ch != null)
            {
                ch.OnWeaponChanged -= UpdateView;
            }
            iconHandle?.Release();
            iconHandle = null;
        }

        public UnityAction OnClick
        {
            set
            {
                Btn.onClick.RemoveAllListeners();
                Btn.onClick.AddListener(value);
            }
        }

        private void UpdateView()
        {
            if (ch.Weapon != null)
            {
                iconHandle = AssetMgr.Instance.package.LoadAssetSync<Sprite>(ch.Weapon.IconLoc);
                Img.sprite = iconHandle.AssetObject as Sprite;
                Img.color = Color.white;
            }
            else
            {
                Img.sprite = null;
                Img.color = Color.clear;
            }
        }

        public void Set(Role ch, UnityAction onClick)
        {
            Clear();
            this.ch = ch;

            UpdateView();
            ch.OnWeaponChanged += UpdateView;

            OnClick = onClick;
        }
    }
}