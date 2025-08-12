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
        private UnityEngine.UI.Button                  UIOverlayBtn;
        private UnityEngine.UI.Button                  UIBackBtn;
        private TMPro.TMP_InputField                   ChatTextInput;
        private UnityEngine.UI.Button                  SendBtn;
        private TMPro.TextMeshProUGUI                  UsernameText;
        private UnityEngine.UI.LoopVerticalScrollRect  ChatFriendLoopScroll;
        private Kirara.UI.SimpleLoopScrollPrefabSource ChatFriendPrefabSource;
        private UnityEngine.UI.Button                  UISelectStickerOverlay;
        private Kirara.UI.UISelectSticker              UISelectSticker;
        private UnityEngine.UI.Button                  StickerBtn;
        private Kirara.UI.LinearScrollView               ChatLoopScroll;
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
            ChatFriendLoopScroll   = c.Q<UnityEngine.UI.LoopVerticalScrollRect>(5, "ChatFriendLoopScroll");
            ChatFriendPrefabSource = c.Q<Kirara.UI.SimpleLoopScrollPrefabSource>(6, "ChatFriendPrefabSource");
            UISelectStickerOverlay = c.Q<UnityEngine.UI.Button>(7, "UISelectStickerOverlay");
            UISelectSticker        = c.Q<Kirara.UI.UISelectSticker>(8, "UISelectSticker");
            StickerBtn             = c.Q<UnityEngine.UI.Button>(9, "StickerBtn");
            ChatLoopScroll         = c.Q<Kirara.UI.LinearScrollView>(10, "ChatLoopScroll");
        }
        #endregion

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
            ChatFriendLoopScroll.prefabSource = ChatFriendPrefabSource;
            ChatFriendLoopScroll.dataSource = new LoopScrollDataSourceHandler(ProvideFriendData);
            ChatFriendLoopScroll.totalCount = friends.Count;
            ChatFriendLoopScroll.RefillCells();

            // 聊天列表
            var chatPool = new LoopScrollGOPool(ChatItemPrefab, transform);
            ChatLoopScroll.SetPoolFunc(chatPool.GetObject, chatPool.ReturnObject);
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

        private void ProvideFriendData(Transform tra, int idx)
        {
            var item = tra.GetComponent<UIChatFriendItem>();
            item.Set(friends[idx], () => ChattingPlayer = friends[idx]);
        }

        private void ProvideChatData(GameObject go, int idx)
        {
            var item = go.GetComponent<UIChatItem>();
            item.Set(ChattingPlayer.ChatMsgs[idx], ChattingPlayer);
        }
    }
}