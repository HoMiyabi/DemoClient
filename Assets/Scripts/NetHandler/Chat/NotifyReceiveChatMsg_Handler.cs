using System;
using Kirara.Network;

namespace Kirara.NetHandler.Chat
{
    public class NotifyReceiveChatMsg_Handler : MsgHandler<NotifyReceiveChatMsg>
    {
        public static event Action<NChatMsg> OnReceiveChatMsg;

        protected override void Run(Session session, NotifyReceiveChatMsg msg)
        {
            var chatMsg = msg.ChatMsg;
            var records = PlayerService.player.allChatRecords[chatMsg.SenderUid];
            records.Add(chatMsg);

            OnReceiveChatMsg?.Invoke(chatMsg);
        }
    }
}