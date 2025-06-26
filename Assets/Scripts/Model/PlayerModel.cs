using System.Collections.Generic;
using System.Linq;


namespace Kirara.Model
{
    public class PlayerModel
    {
        public string Uid { get; set; }
        public string Username { get; set; }
        public int AvatarCid { get; set; }
        public string Signature { get; set; }
        public List<string> FriendUids { get; set; }
        public List<string> FriendRequestUids { get; set; }
        public List<MaterialItem> Materials { get; set; }
        public List<CurrencyItem> Currencies { get; set; }
        public List<WeaponItem> Weapons { get; set; }
        public List<DiscItem> Discs { get; set; }
        public List<RoleModel> Roles { get; set; }
        public List<string> TeamRoleIds { get; set; }
        public string FrontRoleId { get; set; }

        public Dictionary<string, List<NChatMsgRecord>> allChatRecords { get; set; }
        public List<(int questChainCid, int currentQuestCid)> questProgresses { get; set; }

        public PlayerModel(NPlayer player)
        {
            Uid = player.Uid;
            Username = player.Username;
            AvatarCid = player.AvatarCid;
            Signature = player.Signature;
            FriendUids = player.FriendUids.ToList();
            FriendRequestUids = player.FriendRequestUids.ToList();
            Materials = player.Materials.Select(x => new MaterialItem(x)).ToList();
            Currencies = player.Currencies.Select(x => new CurrencyItem(x)).ToList();
            Weapons = player.Weapons.Select(x => new WeaponItem(x)).ToList();
            Discs = player.Discs.Select(x => new DiscItem(x)).ToList();
            Roles = player.Roles.Select(x => new RoleModel(x)).ToList();
            TeamRoleIds = player.TeamRoleIds.ToList();
            FrontRoleId = player.FrontRoleId;
        }
    }
}