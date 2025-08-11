using System;
using Cysharp.Threading.Tasks;
using Kirara.Model;

namespace Kirara.Service
{
    public static class SocialService
    {
        public static event Action<NChatMsg> OnChatMsgsAdd;

        public static async UniTask SendText(SocialPlayer receiver, string text)
        {
            var chatMsg = new NChatMsg
            {
                SenderUid = PlayerService.Player.Uid,
                ReceiverUid = receiver.Uid,
                UnixTimeMs = 0,
                MsgType = 0,
                Text = text,
                StickerCid = 0
            };

            var rsp = await NetFn.ReqSendChatMsg(new ReqSendChatMsg
            {
                ChatMsg = chatMsg,
            });
            chatMsg.UnixTimeMs = rsp.UnixTimeMs;

            receiver.ChatMsgs.Add(chatMsg);
            OnChatMsgsAdd?.Invoke(chatMsg);
        }

        public static async UniTask SendSticker(SocialPlayer receiver, int stickerCid)
        {
            var chatMsg = new NChatMsg
            {
                SenderUid = PlayerService.Player.Uid,
                ReceiverUid = receiver.Uid,
                UnixTimeMs = 0,
                MsgType = 1,
                Text = "",
                StickerCid = stickerCid
            };

            var rsp = await NetFn.ReqSendChatMsg(new ReqSendChatMsg
            {
                ChatMsg = chatMsg
            });
            chatMsg.UnixTimeMs = rsp.UnixTimeMs;

            receiver.ChatMsgs.Add(chatMsg);
            OnChatMsgsAdd?.Invoke(chatMsg);
        }

        public static void NotifyReceiveChatMsg(NChatMsg chatMsg)
        {
            var player = PlayerService.Player.Friends
                .Find(x => x.Uid == chatMsg.SenderUid);
            player.ChatMsgs.Add(chatMsg);
            OnChatMsgsAdd?.Invoke(chatMsg);
        }
    }
}