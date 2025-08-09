using Kirara.Network;

namespace Kirara.NetHandler.Social
{
    public class NotifyFriendRequestsRemove_Handler : MsgHandler<NotifyFriendRequestsRemove>
    {
        protected override void Run(Session session, NotifyFriendRequestsRemove msg)
        {
            PlayerService.Player.FriendRequests.RemoveAll(x => x.Uid == msg.Uid);
            PlayerService.Player.OnFriendRequestsChanged?.Invoke();
        }
    }
}