using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Kirara.NetHandler.Chat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kirara.UI.Panel
{
    public class ChatPanel : BasePanel
    {
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
            var c = GetComponent<KiraraRuntimeComponents>();
            c.Init();
            UIOverlayBtn = c.Q<Button>("UIOverlayBtn");
            UIBackBtn = c.Q<Button>("UIBackBtn");
            ChatTextInput = c.Q<TMP_InputField>("ChatTextInput");
            SendBtn = c.Q<Button>("SendBtn");
            UsernameText = c.Q<TextMeshProUGUI>("UsernameText");
            ChatFriendLoopScroll = c.Q<LoopVerticalScrollRect>("ChatFriendLoopScroll");
            ChatLoopScroll = c.Q<LoopVerticalScrollRect>("ChatLoopScroll");
            ChatFriendPrefabSource = c.Q<SimpleLoopScrollPrefabSource>("ChatFriendPrefabSource");
            ChatPrefabSource = c.Q<SimpleLoopScrollPrefabSource>("ChatPrefabSource");
            UISelectStickerOverlay = c.Q<Button>("UISelectStickerOverlay");
            UISelectSticker = c.Q<UISelectSticker>("UISelectSticker");
            StickerBtn = c.Q<Button>("StickerBtn");
        }

        private List<NOtherPlayer> playerInfos;

        private NOtherPlayer curChattingPlayer;
        private List<NChatMsgRecord> chatRecords;

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
            playerInfos = PlayerService.player.FriendUids;
            ChatFriendLoopScroll.prefabSource = ChatFriendPrefabSource;
            ChatFriendLoopScroll.dataSource = new LoopScrollDataSourceHandler(ProvideFriendData);
            ChatFriendLoopScroll.totalCount = playerInfos.Count;
            ChatFriendLoopScroll.RefillCells();

            // 聊天栏
            ChatLoopScroll.prefabSource = ChatPrefabSource;
            ChatLoopScroll.dataSource = new LoopScrollDataSourceHandler(ProvideChatData);

            SetSelectedPlayerInfo(playerInfos.FirstOrDefault());
        }

        private void OnReceiveChatMsg(NChatMsgRecord record)
        {
            if (curChattingPlayer == null || curChattingPlayer.Uid != record.SenderUid) return;

            ChatLoopScroll.totalCount = chatRecords.Count;
            ChatLoopScroll.RefillCellsFromEnd();
        }

        private void OnEnable()
        {
            NotifyReceiveChatMsg_Handler.OnReceiveChatMsg += OnReceiveChatMsg;
        }

        private void OnDisable()
        {
            NotifyReceiveChatMsg_Handler.OnReceiveChatMsg -= OnReceiveChatMsg;
        }

        public async UniTask SendText(string text)
        {
            var chatMsg = new NChatMsg
            {
                MsgType = 0,
                Text = text,
            };

            var rsp = await NetFn.ReqSendChatMsg(new ReqSendChatMsg
            {
                ReceiverUid = curChattingPlayer.Uid,
                ChatMsg = chatMsg,
            });
            chatRecords.Add(new NChatMsgRecord
            {
                SenderUid = PlayerService.player.Uid,
                UnixTimeMs = rsp.UnixTimeMs,
                ChatMsg = chatMsg
            });

            ChatLoopScroll.totalCount = chatRecords.Count;
            ChatLoopScroll.RefillCellsFromEnd();
        }

        public async UniTask SendSticker(int stickerCid)
        {
            var chatMsg = new NChatMsg
            {
                MsgType = 1,
                StickerCid = stickerCid,
            };

            var rsp = await NetFn.ReqSendChatMsg(new ReqSendChatMsg
            {
                ReceiverUid = curChattingPlayer.Uid,
                ChatMsg = chatMsg
            });
            chatRecords.Add(new NChatMsgRecord
            {
                SenderUid = PlayerService.player.Uid,
                UnixTimeMs = rsp.UnixTimeMs,
                ChatMsg = chatMsg
            });

            ChatLoopScroll.totalCount = chatRecords.Count;
            ChatLoopScroll.RefillCellsFromEnd();
        }

        private void SendBtn_onClick()
        {
            string text = ChatTextInput.text;
            ChatTextInput.text = "";
            SendText(text).Forget();
        }

        private void ProvideFriendData(Transform tra, int idx)
        {
            var item = tra.GetComponent<UIChatFriendItem>();
            item.Set(playerInfos[idx], () => SetSelectedPlayerInfo(playerInfos[idx]));
        }

        private void ProvideChatData(Transform tra, int idx)
        {
            var item = tra.GetComponent<UIChatItem>();
            item.Set(chatRecords[idx], curChattingPlayer);
        }

        private void SetSelectedPlayerInfo(NOtherPlayer newInfo)
        {
            curChattingPlayer = newInfo;

            if (curChattingPlayer == null)
            {
                chatRecords = null;

                UsernameText.text = "Empty";

                ChatLoopScroll.totalCount = 0;
                ChatLoopScroll.RefillCellsFromEnd();

                StickerBtn.interactable = false;
                ChatTextInput.interactable = false;
                SendBtn.interactable = false;
            }
            else
            {
                chatRecords = PlayerService.player.allChatRecords[curChattingPlayer.Uid];

                UsernameText.text = curChattingPlayer.Username;

                ChatLoopScroll.totalCount = chatRecords.Count;
                ChatLoopScroll.RefillCellsFromEnd();

                StickerBtn.interactable = true;
                ChatTextInput.interactable = true;
                SendBtn.interactable = true;
            }
        }
    }
}