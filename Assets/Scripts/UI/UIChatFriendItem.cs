using Kirara.Model;
using Manager;
using UnityEngine;
using UnityEngine.Events;

namespace Kirara.UI
{
    public class UIChatFriendItem : MonoBehaviour
    {
        #region View
        private bool _isBound;
        private UnityEngine.UI.Image  AvatarImg;
        private TMPro.TextMeshProUGUI UsernameText;
        private TMPro.TextMeshProUGUI OnlineStatus;
        private UnityEngine.UI.Button Btn;
        public void BindUI()
        {
            if (_isBound) return;
            _isBound = true;
            var c        = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            AvatarImg    = c.Q<UnityEngine.UI.Image>(0, "AvatarImg");
            UsernameText = c.Q<TMPro.TextMeshProUGUI>(1, "UsernameText");
            OnlineStatus = c.Q<TMPro.TextMeshProUGUI>(2, "OnlineStatus");
            Btn          = c.Q<UnityEngine.UI.Button>(3, "Btn");
        }
        #endregion

        public void Set(SocialPlayer player, UnityAction onClick)
        {
            BindUI();
            var handle = ConfigAsset.GetIconInterKnotRole(player.AvatarCid);
            AvatarImg.sprite = handle.AssetObject as Sprite;

            UsernameText.text = player.Username;
            OnlineStatus.text = player.IsOnline ? "在线" : "离线";

            Btn.onClick.AddListener(onClick);
        }
    }
}