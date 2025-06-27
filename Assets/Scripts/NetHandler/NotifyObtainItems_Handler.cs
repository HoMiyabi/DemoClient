using Kirara.Network;
using Kirara.Service;

namespace Kirara.NetHandler
{
    public class NotifyObtainItems_Handler : MsgHandler<NotifyObtainItems>
    {
        protected override void Run(Session session, NotifyObtainItems msg)
        {
            InventoryService.NotifyObtainItems(msg);
        }
    }
}