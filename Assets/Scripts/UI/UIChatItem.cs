using Kirara.Model;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YooAsset;

namespace Kirara.UI
{

    public class UIChatItem : MonoBehaviour
    {
        #region View
        private Image                 AvatarImg;
        private TextMeshProUGUI       ChatText;
        private HorizontalLayoutGroup Layout;
        private Image                 ChatBubble;
        private Image                 ChatStickerImg;
        private void InitUI()
        {
            var c          = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            AvatarImg      = c.Q<Image>(0, "AvatarImg");
            ChatText       = c.Q<TextMeshProUGUI>(1, "ChatText");
            Layout         = c.Q<HorizontalLayoutGroup>(2, "Layout");
            ChatBubble     = c.Q<Image>(3, "ChatBubble");
            ChatStickerImg = c.Q<Image>(4, "ChatStickerImg");
        }
        #endregion

        [SerializeField] private bool isLeft = true;
        [SerializeField] private Color leftBubbleColor = Color.white;
        [SerializeField] private Color leftTextColor = Color.black;
        [SerializeField] private Color rightBubbleColor = Color.white;
        [SerializeField] private Color rightTextColor = Color.black;

        private AssetHandle avatarHandle;
        private AssetHandle stickerHandle;

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
            stickerHandle?.Release();
            stickerHandle = null;
        }

        private void UpdateLoc()
        {
            if (isLeft)
            {
                Layout.reverseArrangement = false;
                Layout.childAlignment = TextAnchor.UpperLeft;
                ChatBubble.color = leftBubbleColor;
                ChatText.color = leftTextColor;
            }
            else
            {
                Layout.reverseArrangement = true;
                Layout.childAlignment = TextAnchor.UpperRight;
                ChatBubble.color = rightBubbleColor;
                ChatText.color = rightTextColor;
            }
        }

        public void Set(NChatMsg msg, SocialPlayer other)
        {
            Clear();

            bool isSelf = msg.SenderUid == PlayerService.Player.Uid;

            // 位置左右
            isLeft = !isSelf;
            UpdateLoc();

            // 头像
            avatarHandle = ConfigAsset.GetIconInterKnotRole(isSelf ?
                PlayerService.Player.AvatarCid : other.AvatarCid);
            AvatarImg.sprite = avatarHandle.AssetObject as Sprite;

            // 内容
            if (msg.MsgType == 0)
            {
                ChatText.gameObject.SetActive(true);
                ChatStickerImg.gameObject.SetActive(false);

                ChatText.text = msg.Text;
            }
            else if (msg.MsgType == 1)
            {
                ChatText.gameObject.SetActive(false);
                ChatStickerImg.gameObject.SetActive(true);

                stickerHandle = ConfigAsset.GetIconSticker(msg.StickerCid);
                ChatStickerImg.sprite = stickerHandle.AssetObject as Sprite;
            }
            else
            {
                Debug.LogError($"record.ChatMsg.MsgType: {msg.MsgType}");
            }
        }

        private void OnValidate()
        {
            if (!AvatarImg)
            {
                InitUI();
            }
            UpdateLoc();
        }
    }
}