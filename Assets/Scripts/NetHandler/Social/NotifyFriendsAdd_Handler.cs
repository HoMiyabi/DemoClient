using Kirara.Model;
using Kirara.Network;

namespace Kirara.NetHandler.Social
{
    public class NotifyFriendsAdd_Handler : MsgHandler<NotifyFriendsAdd>
    {
        protected override void Run(Session session, NotifyFriendsAdd msg)
        {
            PlayerService.Player.Friends.Add(new SocialPlayer(msg.Player));
            PlayerService.Player.OnFriendsChanged?.Invoke();
        }
    }
}