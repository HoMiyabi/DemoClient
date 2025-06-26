using System;
using Kirara.Network;

namespace Kirara.NetHandler
{
    public class NotifyObtainItems_Handler : MsgHandler<NotifyObtainItems>
    {
        public static event Action<NotifyObtainItems> OnNotifyObtainItems;

        protected override void Run(Session session, NotifyObtainItems message)
        {
            OnNotifyObtainItems?.Invoke(message);
        }
    }
}