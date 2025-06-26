using System;

using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YooAsset;

namespace Kirara.UI
{

    public class UIChatItem : MonoBehaviour
    {
        private Image AvatarImg;
        private TextMeshProUGUI ChatText;
        private HorizontalLayoutGroup Layout;
        private Image ChatBubble;
        private Image ChatStickerImg;

        private void InitUI()
        {
            var c = GetComponent<KiraraRuntimeComponents>();
            c.Init();
            AvatarImg = c.Q<Image>("AvatarImg");
            ChatText = c.Q<TextMeshProUGUI>("ChatText");
            Layout = c.Q<HorizontalLayoutGroup>("Layout");
            ChatBubble = c.Q<Image>("ChatBubble");
            ChatStickerImg = c.Q<Image>("ChatStickerImg");
        }

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

        public void Set(NChatMsgRecord record, NOtherPlayer other)
        {
            Clear();

            bool isSelf = record.SenderUid == PlayerService.player.Uid;

            // 位置左右
            isLeft = !isSelf;
            UpdateLoc();

            // 头像
            avatarHandle = ConfigAsset.GetIconInterKnotRole(isSelf ?
                PlayerService.player.AvatarCid : other.AvatarCid);
            AvatarImg.sprite = avatarHandle.AssetObject as Sprite;

            // 内容
            if (record.ChatMsg.MsgType == 0)
            {
                ChatText.gameObject.SetActive(true);
                ChatStickerImg.gameObject.SetActive(false);

                ChatText.text = record.ChatMsg.Text;
            }
            else if (record.ChatMsg.MsgType == 1)
            {
                ChatText.gameObject.SetActive(false);
                ChatStickerImg.gameObject.SetActive(true);

                stickerHandle = ConfigAsset.GetIconSticker(record.ChatMsg.StickerCid);
                ChatStickerImg.sprite = stickerHandle.AssetObject as Sprite;
            }
            else
            {
                Debug.LogError($"record.ChatMsg.MsgType: {record.ChatMsg.MsgType}");
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