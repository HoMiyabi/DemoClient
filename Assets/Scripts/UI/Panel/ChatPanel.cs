using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Kirara.Model;
using Kirara.Service;
using UnityEngine;

namespace Kirara.UI.Panel
{
    public class ChatPanel : BasePanel
    {
        #region View
        private bool _isBound;
        private UnityEngine.UI.Button             UIOverlayBtn;
        private UnityEngine.UI.Button             UIBackBtn;
        private TMPro.TMP_InputField              ChatTextInput;
        private UnityEngine.UI.Button             SendBtn;
        private TMPro.TextMeshProUGUI             UsernameText;
        private UnityEngine.UI.Button             UISelectStickerOverlay;
        private Kirara.UI.UISelectSticker         UISelectSticker;
        private UnityEngine.UI.Button             StickerBtn;
        private KiraraLoopScroll.LinearScrollView ChatLoopScroll;
        private KiraraLoopScroll.LinearScrollView ChatFriendScrollView;
        public override void BindUI()
        {
            if (_isBound) return;
            _isBound = true;
            var c                  = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            UIOverlayBtn           = c.Q<UnityEngine.UI.Button>(0, "UIOverlayBtn");
            UIBackBtn              = c.Q<UnityEngine.UI.Button>(1, "UIBackBtn");
            ChatTextInput          = c.Q<TMPro.TMP_InputField>(2, "ChatTextInput");
            SendBtn                = c.Q<UnityEngine.UI.Button>(3, "SendBtn");
            UsernameText           = c.Q<TMPro.TextMeshProUGUI>(4, "UsernameText");
            UISelectStickerOverlay = c.Q<UnityEngine.UI.Button>(5, "UISelectStickerOverlay");
            UISelectSticker        = c.Q<Kirara.UI.UISelectSticker>(6, "UISelectSticker");
            StickerBtn             = c.Q<UnityEngine.UI.Button>(7, "StickerBtn");
            ChatLoopScroll         = c.Q<KiraraLoopScroll.LinearScrollView>(8, "ChatLoopScroll");
            ChatFriendScrollView   = c.Q<KiraraLoopScroll.LinearScrollView>(9, "ChatFriendScrollView");
        }
        #endregion

        public GameObject ChatFriendItemPrefab;
        public GameObject ChatItemPrefab;

        private List<SocialPlayer> friends;

        private SocialPlayer _chattingPlayer;
        public SocialPlayer ChattingPlayer
        {
            get => _chattingPlayer;
            private set
            {
                _chattingPlayer = value;

                if (_chattingPlayer == null)
                {
                    // 聊天对象为空
                    UsernameText.text = "Empty";

                    ChatLoopScroll.totalCount = 0;
                    ChatLoopScroll.RefreshToEnd();

                    StickerBtn.interactable = false;
                    ChatTextInput.interactable = false;
                    SendBtn.interactable = false;
                }
                else
                {
                    UsernameText.text = _chattingPlayer.Username;

                    ChatLoopScroll.totalCount = _chattingPlayer.ChatMsgs.Count;
                    ChatLoopScroll.RefreshToEnd();

                    StickerBtn.interactable = true;
                    ChatTextInput.interactable = true;
                    SendBtn.interactable = true;
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();
            UIBackBtn.onClick.AddListener(() => UIMgr.Instance.PopPanel(this));
            UIOverlayBtn.onClick.AddListener(() => UIMgr.Instance.PopPanel(this));
            SendBtn.onClick.AddListener(SendBtn_onClick);


            // 贴纸
            UISelectSticker.Set(this);

            UISelectStickerOverlay.gameObject.SetActive(false);
            UISelectSticker.gameObject.SetActive(false);
            StickerBtn.onClick.AddListener(() =>
            {
                UISelectStickerOverlay.gameObject.SetActive(true);
                UISelectSticker.gameObject.SetActive(true);
            });
            UISelectStickerOverlay.onClick.AddListener(() =>
            {
                UISelectStickerOverlay.gameObject.SetActive(false);
                UISelectSticker.gameObject.SetActive(false);
            });

            // 聊天人选择列表
            friends = PlayerService.Player.Friends;
            ChatFriendScrollView.SetGOPool(new LoopScrollGOPool(ChatFriendItemPrefab, transform));
            ChatFriendScrollView.provideData = ProvideFriendData;
            ChatFriendScrollView.totalCount = friends.Count;

            // 聊天列表
            ChatLoopScroll.SetGOPool(new LoopScrollGOPool(ChatItemPrefab, transform));
            ChatLoopScroll.provideData = ProvideChatData;

            ChattingPlayer = friends.FirstOrDefault();

            SocialService.OnChatMsgsAdd += OnChatMsgsAdd;
        }

        private void OnDestroy()
        {
            SocialService.OnChatMsgsAdd -= OnChatMsgsAdd;
        }

        private void OnChatMsgsAdd(NChatMsg msg)
        {
            if (ChattingPlayer != null &&
                (ChattingPlayer.Uid == msg.SenderUid ||
                 ChattingPlayer.Uid == msg.ReceiverUid))
            {
                ChatLoopScroll.totalCount = ChattingPlayer.ChatMsgs.Count;
                ChatLoopScroll.RefreshToEnd();
            }
        }

        private void SendBtn_onClick()
        {
            string text = ChatTextInput.text;
            ChatTextInput.text = "";
            SocialService.SendText(ChattingPlayer, text).Forget();
        }

        private void ProvideFriendData(GameObject go, int index)
        {
            var item = go.GetComponent<UIChatFriendItem>();
            item.Set(friends[index], () => ChattingPlayer = friends[index]);
        }

        private void ProvideChatData(GameObject go, int index)
        {
            var item = go.GetComponent<UIChatItem>();
            item.Set(ChattingPlayer.ChatMsgs[index], ChattingPlayer);
        }
    }
}