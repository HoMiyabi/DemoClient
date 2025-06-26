using System;
using Kirara.Network;

namespace Kirara.NetHandler.Chat
{
    public class NotifyReceiveChatMsg_Handler : MsgHandler<NotifyReceiveChatMsg>
    {
        public static event Action<NChatMsgRecord> OnReceiveChatMsg;

        protected override void Run(Session session, NotifyReceiveChatMsg msg)
        {
            var record = msg.ChatMsgRecord;
            var records = PlayerService.player.allChatRecords[record.SenderUid];
            records.Add(record);

            OnReceiveChatMsg?.Invoke(record);
        }
    }
}