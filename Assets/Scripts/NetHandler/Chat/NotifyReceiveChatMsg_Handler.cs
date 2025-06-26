using System;
using Kirara.Network;

namespace Kirara.NetHandler.Chat
{
    public class NotifyReceiveChatMsg_Handler : MsgHandler<NotifyReceiveChatMsg>
    {
        public static event Action<NChatMsgRecordItem> OnReceiveChatMsg;

        protected override void Run(Session session, NotifyReceiveChatMsg msg)
        {
            var record = msg.ChatMsgRecordItem;
            var records = PlayerService.player.allChatRecords[record.SenderUId];
            records.Add(record);

            OnReceiveChatMsg?.Invoke(record);
        }
    }
}