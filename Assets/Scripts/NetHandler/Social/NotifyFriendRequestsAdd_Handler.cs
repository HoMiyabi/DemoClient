using Kirara.Model;
using Kirara.Network;

namespace Kirara.NetHandler.Social
{
    public class NotifyFriendRequestsAdd_Handler : MsgHandler<NotifyFriendRequestsAdd>
    {
        protected override void Run(Session session, NotifyFriendRequestsAdd msg)
        {
            PlayerService.Player.FriendRequests.Add(new SocialPlayer(msg.Player));
            PlayerService.Player.OnFriendRequestsChanged?.Invoke();
        }
    }
}