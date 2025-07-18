﻿using Kirara.Model;
using Kirara.UI.Panel;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YooAsset;

namespace Kirara.UI
{
    public class UIUserInfoBar : MonoBehaviour
    {
        #region View
        private Button          AvatarBtn;
        private Image           AvatarImg;
        private Button          ChatBtn;
        private TextMeshProUGUI SignatureText;
        private TextMeshProUGUI UsernameText;
        private TextMeshProUGUI UIUserOnlineStatus;
        private void InitUI()
        {
            var c              = GetComponent<KiraraDirectBinder>();
            AvatarBtn          = c.Q<Button>(0, "AvatarBtn");
            AvatarImg          = c.Q<Image>(1, "AvatarImg");
            ChatBtn            = c.Q<Button>(2, "ChatBtn");
            SignatureText      = c.Q<TextMeshProUGUI>(3, "SignatureText");
            UsernameText       = c.Q<TextMeshProUGUI>(4, "UsernameText");
            UIUserOnlineStatus = c.Q<TextMeshProUGUI>(5, "UIUserOnlineStatus");
        }
        #endregion

        private AssetHandle avatarHandle;

        private void Clear()
        {
            avatarHandle?.Release();
            avatarHandle = null;
        }

        private void OnDestroy()
        {
            Clear();
        }

        private void Awake()
        {
            InitUI();
        }

        public void Set(SocialPlayer player)
        {
            Clear();
            avatarHandle = ConfigAsset.GetIconInterKnotRole(player.AvatarCid);
            AvatarImg.sprite = avatarHandle.AssetObject as Sprite;

            SignatureText.text = player.Signature;
            UsernameText.text = player.Username;
            UIUserOnlineStatus.text = player.IsOnline ? "在线" : "离线";

            ChatBtn.onClick.AddListener(() => UIMgr.Instance.PushPanel<ChatPanel>());
        }
    }
}