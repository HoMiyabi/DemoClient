using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Kirara.Model;
using Kirara.Service;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kirara.UI.Panel
{
    public class ChatPanel : BasePanel
    {
        #region View
        private Button UIOverlayBtn;
        private Button UIBackBtn;
        private TMP_InputField ChatTextInput;
        private Button SendBtn;
        private TextMeshProUGUI UsernameText;
        private LoopVerticalScrollRect ChatFriendLoopScroll;
        private LoopVerticalScrollRect ChatLoopScroll;
        private SimpleLoopScrollPrefabSource ChatFriendPrefabSource;
        private SimpleLoopScrollPrefabSource ChatPrefabSource;
        private Button UISelectStickerOverlay;
        private UISelectSticker UISelectSticker;
        private Button StickerBtn;
        private void InitUI()
        {
            var c = GetComponent<KiraraDirectBinder>();
            UIOverlayBtn = c.Q<Button>(0, "UIOverlayBtn");
            UIBackBtn = c.Q<Button>(1, "UIBackBtn");
            ChatTextInput = c.Q<TMP_InputField>(2, "ChatTextInput");
            SendBtn = c.Q<Button>(3, "SendBtn");
            UsernameText = c.Q<TextMeshProUGUI>(4, "UsernameText");
            ChatFriendLoopScroll = c.Q<LoopVerticalScrollRect>(5, "ChatFriendLoopScroll");
            ChatLoopScroll = c.Q<LoopVerticalScrollRect>(6, "ChatLoopScroll");
            ChatFriendPrefabSource = c.Q<SimpleLoopScrollPrefabSource>(7, "ChatFriendPrefabSource");
            ChatPrefabSource = c.Q<SimpleLoopScrollPrefabSource>(8, "ChatPrefabSource");
            UISelectStickerOverlay = c.Q<Button>(9, "UISelectStickerOverlay");
            UISelectSticker = c.Q<UISelectSticker>(10, "UISelectSticker");
            StickerBtn = c.Q<Button>(11, "StickerBtn");
        }
        #endregion

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
                    ChatLoopScroll.RefillCellsFromEnd();

                    StickerBtn.interactable = false;
                    ChatTextInput.interactable = false;
                    SendBtn.interactable = false;
                }
                else
                {
                    UsernameText.text = _chattingPlayer.Username;

                    ChatLoopScroll.totalCount = _chattingPlayer.ChatMsgs.Count;
                    ChatLoopScroll.RefillCellsFromEnd();

                    StickerBtn.interactable = true;
                    ChatTextInput.interactable = true;
                    SendBtn.interactable = true;
                }
            }
        }

        private void Awake()
        {
            InitUI();

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
        }

        private void Start()
        {
            // 选择栏
            friends = PlayerService.Player.Friends;
            ChatFriendLoopScroll.prefabSource = ChatFriendPrefabSource;
            ChatFriendLoopScroll.dataSource = new LoopScrollDataSourceHandler(ProvideFriendData);
            ChatFriendLoopScroll.totalCount = friends.Count;
            ChatFriendLoopScroll.RefillCells();

            // 聊天栏
            ChatLoopScroll.prefabSource = ChatPrefabSource;
            ChatLoopScroll.dataSource = new LoopScrollDataSourceHandler(ProvideChatData);

            ChattingPlayer = friends.FirstOrDefault();
        }

        private void OnEnable()
        {
            SocialService.OnAddChatMsg += OnAddChatMsg;
        }

        private void OnDisable()
        {
            SocialService.OnAddChatMsg -= OnAddChatMsg;
        }

        private void OnAddChatMsg(NChatMsg msg)
        {
            if (ChattingPlayer != null &&
                (ChattingPlayer.Uid == msg.SenderUid ||
                 ChattingPlayer.Uid == msg.ReceiverUid))
            {
                ChatLoopScroll.totalCount = ChattingPlayer.ChatMsgs.Count;
                ChatLoopScroll.RefillCellsFromEnd();
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

        private void ProvideChatData(Transform tra, int idx)
        {
            var item = tra.GetComponent<UIChatItem>();
            item.Set(ChattingPlayer.ChatMsgs[idx], ChattingPlayer);
        }
    }
}