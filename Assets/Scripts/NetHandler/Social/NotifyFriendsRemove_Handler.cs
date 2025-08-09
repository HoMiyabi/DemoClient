using Kirara.Network;

namespace Kirara.NetHandler.Social
{
    public class NotifyFriendsRemove_Handler : MsgHandler<NotifyFriendsRemove>
    {
        protected override void Run(Session session, NotifyFriendsRemove msg)
        {
            PlayerService.Player.Friends.RemoveAll(x => x.Uid == msg.Uid);
            PlayerService.Player.OnFriendsChanged?.Invoke();
        }
    }
}