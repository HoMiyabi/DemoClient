using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
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
        private UnityEngine.CanvasGroup           CanvasGroup;
        private UnityEngine.RectTransform         Box;
        private UnityEngine.CanvasGroup           UISelectStickerCanvasGroup;
        public override void BindUI()
        {
        if (_isBound) return;
        _isBound = true;
            var b                      = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            UIOverlayBtn               = b.Q<UnityEngine.UI.Button>(0, "UIOverlayBtn");
            UIBackBtn                  = b.Q<UnityEngine.UI.Button>(1, "UIBackBtn");
            ChatTextInput              = b.Q<TMPro.TMP_InputField>(2, "ChatTextInput");
            SendBtn                    = b.Q<UnityEngine.UI.Button>(3, "SendBtn");
            UsernameText               = b.Q<TMPro.TextMeshProUGUI>(4, "UsernameText");
            UISelectStickerOverlay     = b.Q<UnityEngine.UI.Button>(5, "UISelectStickerOverlay");
            UISelectSticker            = b.Q<Kirara.UI.UISelectSticker>(6, "UISelectSticker");
            StickerBtn                 = b.Q<UnityEngine.UI.Button>(7, "StickerBtn");
            ChatLoopScroll             = b.Q<KiraraLoopScroll.LinearScrollView>(8, "ChatLoopScroll");
            ChatFriendScrollView       = b.Q<KiraraLoopScroll.LinearScrollView>(9, "ChatFriendScrollView");
            CanvasGroup                = b.Q<UnityEngine.CanvasGroup>(10, "CanvasGroup");
            Box                        = b.Q<UnityEngine.RectTransform>(11, "Box");
            UISelectStickerCanvasGroup = b.Q<UnityEngine.CanvasGroup>(12, "UISelectStickerCanvasGroup");
        }
        #endregion

        public GameObject ChatFriendItemPrefab;
        public GameObject ChatItemPrefab;

        private List<SocialPlayer> friends;

        private SocialPlayer _chattingPlayer;
        public SocialPlayer ChattingPlayer
        {
            get => _chattingPlayer;
            set
            {
                _chattingPlayer = value;

                if (_chattingPlayer == null)
                {
                    // 聊天对象为空
                    UsernameText.text = "Empty";

                    ChatLoopScroll._totalCount = 0;
                    ChatLoopScroll.RefreshToEnd();

                    StickerBtn.interactable = false;
                    ChatTextInput.interactable = false;
                    SendBtn.interactable = false;
                }
                else
                {
                    UsernameText.text = _chattingPlayer.Username;

                    ChatLoopScroll._totalCount = _chattingPlayer.ChatMsgs.Count;
                    ChatLoopScroll.RefreshToEnd();

                    StickerBtn.interactable = true;
                    ChatTextInput.interactable = true;
                    SendBtn.interactable = true;
                }
            }
        }

        public override void PlayEnter()
        {
            Box.anchoredPosition = new Vector2(-Box.rect.width * 0.05f, 0);
            Box.DOAnchorPos(Vector2.zero, 0.1f);
            CanvasGroup.alpha = 0f;
            CanvasGroup.DOFade(1f, 0.1f).OnComplete(base.PlayEnter);
        }

        public override void PlayExit()
        {
            Box.DOAnchorPos(new Vector2(-Box.rect.width * 0.05f, 0), 0.1f);
            CanvasGroup.DOFade(0f, 0.1f).OnComplete(base.PlayExit);
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
                UISelectStickerCanvasGroup.alpha = 0f;
                UISelectStickerCanvasGroup.DOFade(1f, 0.1f);
            });
            UISelectStickerOverlay.onClick.AddListener(() =>
            {
                UISelectStickerOverlay.gameObject.SetActive(false);
                UISelectStickerCanvasGroup.DOFade(0f, 0.1f).OnComplete(() =>
                {
                    UISelectStickerOverlay.gameObject.SetActive(false);
                });
            });

            // 聊天人选择列表
            friends = PlayerService.Player.Friends;
            ChatFriendScrollView.SetSource(new LoopScrollGOPool(ChatFriendItemPrefab, transform));
            ChatFriendScrollView.provideData = ProvideFriendData;
            ChatFriendScrollView._totalCount = friends.Count;

            // 聊天列表
            ChatLoopScroll.SetSource(new LoopScrollGOPool(ChatItemPrefab, transform));
            ChatLoopScroll.provideData = ProvideChatData;

            ChattingPlayer = null;

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
                ChatLoopScroll._totalCount = ChattingPlayer.ChatMsgs.Count;
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