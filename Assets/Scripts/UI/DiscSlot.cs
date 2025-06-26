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
        private Image Img;
        private Button Btn;

        private void InitUI()
        {
            var c = GetComponent<KiraraRuntimeComponents>();
            c.Init();
            Img = c.Q<Image>("Img");
            Btn = c.Q<Button>("Btn");
        }

        private void Awake()
        {
            InitUI();
        }

        private RoleModel ch;
        private int pos;
        private AssetHandle icon;

        private void OnDestroy()
        {
            Clear();
        }

        private void Clear()
        {
            if (ch != null)
            {
                ch.OnDiscChanged -= UpdateView;
            }
            icon?.Release();
            icon = null;
        }

        public Action<int> OnClick
        {
            set
            {
                Btn.onClick.RemoveAllListeners();
                Btn.onClick.AddListener(() => value?.Invoke(pos));
            }
        }

        private void UpdateView(int pos1)
        {
            if (pos1 != pos) return;

            var disc = ch.Disc(pos);

            if (disc != null)
            {
                icon = AssetMgr.Instance.package.LoadAssetSync<Sprite>(disc.IconLoc);
                Img.sprite = icon.AssetObject as Sprite;
                Img.color = Color.white;
            }
            else
            {
                Img.sprite = null;
                Img.color = Color.clear;
            }
        }

        public DiscSlot Set(RoleModel ch, int pos, Action<int> onClick)
        {
            Clear();
            this.ch = ch;
            this.pos = pos;
            UpdateView(pos);
            ch.OnDiscChanged += UpdateView;

            OnClick = onClick;
            return this;
        }
    }
}