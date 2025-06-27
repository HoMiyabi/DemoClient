using Kirara.Network;
using Kirara.Service;

namespace Kirara.NetHandler.Chat
{
    public class NotifyReceiveChatMsg_Handler : MsgHandler<NotifyReceiveChatMsg>
    {
        protected override void Run(Session session, NotifyReceiveChatMsg msg)
        {
            var chatMsg = msg.ChatMsg;
            ChatService.NotifyReceiveChatMsg(chatMsg);
        }
    }
}