using System.Collections.Generic;


namespace Kirara.Model
{
    public class PlayerModel
    {
        public NPlayerInfo playerInfo;

        // public int UId;
        // public string Username;
        // public string Signature;
        // public int AvatarCid;
        // public List<int> FriendUIds;
        // public List<int> FriendRequestUIds;
        // public List<NMaterialItem> MatItems;
        // public List<NCurrencyItem> CurItems;
        // public List<int> GroupChCids;
        // public int FrontChCid;

        public List<CharacterModel> chModels;
        public List<NOtherPlayerInfo> friendInfos;
        public Dictionary<int, List<NChatMsgRecordItem>> allChatRecords;
        public List<(int questChainCid, int currentQuestCid)> questProgresses;

        public List<DiscItem> discs;
        public List<WeaponItem> weapons;
        public List<CurrencyItem> currencies;
        public List<MaterialItem> materials;
    }
}