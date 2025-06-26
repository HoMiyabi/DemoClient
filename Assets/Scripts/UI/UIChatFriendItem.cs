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
        private Image AvatarImg;
        private TextMeshProUGUI UsernameText;
        private TextMeshProUGUI OnlineStatus;
        private Button Btn;

        private void InitUI()
        {
            var c = GetComponent<KiraraRuntimeComponents>();
            c.Init();
            AvatarImg = c.Q<Image>("AvatarImg");
            UsernameText = c.Q<TextMeshProUGUI>("UsernameText");
            OnlineStatus = c.Q<TextMeshProUGUI>("OnlineStatus");
            Btn = c.Q<Button>("Btn");
        }

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

        public void Set(NOtherPlayer info, UnityAction onClick)
        {
            Clear();

            avatarHandle = ConfigAsset.GetIconInterKnotRole(info.AvatarCid);
            AvatarImg.sprite = avatarHandle.AssetObject as Sprite;

            UsernameText.text = info.Username;
            OnlineStatus.text = info.IsOnline ? "在线" : "离线";

            Btn.onClick.AddListener(onClick);
        }
    }
}