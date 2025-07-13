using Kirara.Model;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using YooAsset;

namespace Kirara.UI
{
    public class UIChatFriendItem : MonoBehaviour
    {
        #region View
        private Image           AvatarImg;
        private TextMeshProUGUI UsernameText;
        private TextMeshProUGUI OnlineStatus;
        private Button          Btn;
        private void InitUI()
        {
            var c        = GetComponent<KiraraDirectBinder>();
            AvatarImg    = c.Q<Image>(0, "AvatarImg");
            UsernameText = c.Q<TextMeshProUGUI>(1, "UsernameText");
            OnlineStatus = c.Q<TextMeshProUGUI>(2, "OnlineStatus");
            Btn          = c.Q<Button>(3, "Btn");
        }
        #endregion

        private AssetHandle avatarHandle;

        private void Awake()
        {
            InitUI();
        }

        private void OnDestroy()
        {
            Clear();
        }

        private void Clear()
        {
            avatarHandle?.Release();
            avatarHandle = null;
        }

        public void Set(SocialPlayer player, UnityAction onClick)
        {
            Clear();

            avatarHandle = ConfigAsset.GetIconInterKnotRole(player.AvatarCid);
            AvatarImg.sprite = avatarHandle.AssetObject as Sprite;

            UsernameText.text = player.Username;
            OnlineStatus.text = player.IsOnline ? "在线" : "离线";

            Btn.onClick.AddListener(onClick);
        }
    }
}