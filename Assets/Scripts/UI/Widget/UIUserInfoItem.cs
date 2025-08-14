using Kirara.Model;
using Kirara.UI.Panel;
using Manager;
using UnityEngine;

namespace Kirara.UI
{
    public class UIUserInfoItem : MonoBehaviour
    {
        #region View
        private bool _isBound;
        private UnityEngine.UI.Button AvatarBtn;
        private UnityEngine.UI.Image  AvatarImg;
        private UnityEngine.UI.Button ChatBtn;
        private TMPro.TextMeshProUGUI SignatureText;
        private TMPro.TextMeshProUGUI UsernameText;
        private TMPro.TextMeshProUGUI UIUserOnlineStatus;
        public void BindUI()
        {
            if (_isBound) return;
            _isBound = true;
            var c              = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            AvatarBtn          = c.Q<UnityEngine.UI.Button>(0, "AvatarBtn");
            AvatarImg          = c.Q<UnityEngine.UI.Image>(1, "AvatarImg");
            ChatBtn            = c.Q<UnityEngine.UI.Button>(2, "ChatBtn");
            SignatureText      = c.Q<TMPro.TextMeshProUGUI>(3, "SignatureText");
            UsernameText       = c.Q<TMPro.TextMeshProUGUI>(4, "UsernameText");
            UIUserOnlineStatus = c.Q<TMPro.TextMeshProUGUI>(5, "UIUserOnlineStatus");
        }
        #endregion

        public void Set(SocialPlayer player)
        {
            BindUI();
            var handle = ConfigAsset.GetIconInterKnotRole(player.AvatarCid);
            AvatarImg.sprite = handle.AssetObject as Sprite;

            SignatureText.text = player.Signature;
            UsernameText.text = player.Username;
            UIUserOnlineStatus.text = player.IsOnline ? "在线" : "离线";

            ChatBtn.onClick.RemoveAllListeners();
            ChatBtn.onClick.AddListener(() =>
            {
                var panel = UIMgr.Instance.PushPanel<ChatPanel>();
                panel.ChattingPlayer = player;
            });
        }
    }
}