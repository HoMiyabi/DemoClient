using System.Collections.Generic;
using System.Linq;

namespace Kirara.Model
{
    public class SocialPlayer
    {
        public string Uid { get; set; }
        public string Username { get; set; }
        public string Signature { get; set; }
        public int AvatarCid { get; set; }
        public bool IsOnline { get; set; }
        public List<NChatMsg> ChatMsgs { get; set; }

        public SocialPlayer(NSocialPlayer player)
        {
            Uid = player.Uid;
            Username = player.Username;
            Signature = player.Signature;
            AvatarCid = player.AvatarCid;
            IsOnline = player.IsOnline;
            ChatMsgs = player.ChatMsgs.ToList();
        }
    }
}