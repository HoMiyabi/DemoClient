using System.Collections.Generic;
using System.Linq;

namespace Kirara.Model
{
    public class Player
    {
        public string Uid { get; set; }
        public string Username { get; set; }
        public int AvatarCid { get; set; }
        public string Signature { get; set; }
        public List<SocialPlayer> Friends { get; set; }
        public List<SocialPlayer> FriendRequests { get; set; }
        public List<MaterialItem> Materials { get; set; }
        public List<CurrencyItem> Currencies { get; set; }
        public List<WeaponItem> Weapons { get; set; }
        public List<DiscItem> Discs { get; set; }
        public List<Role> Roles { get; set; }
        public List<string> TeamRoleIds { get; set; }
        public string FrontRoleId { get; set; }
        public List<(int questChainCid, int currentQuestCid)> questProgresses { get; set; }

        public Player(NPlayer player)
        {
            Uid = player.Uid;
            Username = player.Username;
            AvatarCid = player.AvatarCid;
            Signature = player.Signature;
            Friends = player.Friends.Select(x => new SocialPlayer(x)).ToList();
            FriendRequests = player.FriendRequests.Select(x => new SocialPlayer(x)).ToList();
            Materials = player.Materials.Select(x => new MaterialItem(x)).ToList();
            Currencies = player.Currencies.Select(x => new CurrencyItem(x)).ToList();
            Weapons = player.Weapons.Select(x => new WeaponItem(x)).ToList();
            Discs = player.Discs.Select(x => new DiscItem(x)).ToList();
            Roles = player.Roles.Select(x => new Role(x)).ToList();
            TeamRoleIds = player.TeamRoleIds.ToList();
            FrontRoleId = player.FrontRoleId;

            // 任务进度
            questProgresses = new List<(int questChainCid, int currentQuestCid)>
            {
                (1, 1),
            };
        }
    }
}