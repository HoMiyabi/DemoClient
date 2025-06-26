using System;

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
        private Button AvatarBtn;
        private Image AvatarImg;
        private Button ChatBtn;
        private TextMeshProUGUI SignatureText;
        private TextMeshProUGUI UsernameText;
        private TextMeshProUGUI UIUserOnlineStatus;

        private void InitUI()
        {
            var c = GetComponent<KiraraRuntimeComponents>();
            c.Init();
            AvatarBtn = c.Q<Button>("AvatarBtn");
            AvatarImg = c.Q<Image>("AvatarImg");
            ChatBtn = c.Q<Button>("ChatBtn");
            SignatureText = c.Q<TextMeshProUGUI>("SignatureText");
            UsernameText = c.Q<TextMeshProUGUI>("UsernameText");
            UIUserOnlineStatus = c.Q<TextMeshProUGUI>("UIUserOnlineStatus");
        }

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

        public void Set(NOtherPlayer info)
        {
            Clear();
            avatarHandle = ConfigAsset.GetIconInterKnotRole(info.AvatarCid);
            AvatarImg.sprite = avatarHandle.AssetObject as Sprite;

            SignatureText.text = info.Signature;
            UsernameText.text = info.Username;
            UIUserOnlineStatus.text = info.IsOnline ? "在线" : "离线";

            ChatBtn.onClick.AddListener(() => UIMgr.Instance.PushPanel<ChatPanel>());
        }
    }
}