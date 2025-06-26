using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Kirara.Model;
using Kirara.Service;
using UnityEngine;

namespace Kirara
{
    public static class PlayerService
    {
        public static PlayerModel player;

        public static async UniTask FetchData()
        {
            player = new PlayerModel();

            // 获取物品信息
            await FetchInventoryData();
            InventoryService.Init();

            // 玩家信息
            var rsp = await NetFn.ReqGetPlayerInfo(new ReqGetPlayerInfo());
            if (rsp.Code != 0)
            {
                Debug.LogError(rsp.Msg);
            }
            player.playerInfo = rsp.PlayerData;
            player.currencies = rsp.PlayerInfo.CurItems
                .Select(it => new CurrencyItem(it)).ToList();
            player.materials = rsp.PlayerInfo.MatItems
                .Select(it => new MaterialItem(it)).ToList();

            // 角色信息
            var rsp1 = await NetFn.ReqGetCharacterInfos(new ReqGetCharacterInfos());
            if (rsp1.Code != 0)
            {
                Debug.LogError(rsp1.Msg);
            }
            player.chModels = rsp1.CharacterInfos.Select(it => new CharacterModel(it)).ToList();

            // 好友信息
            var rsp2 = await NetFn.ReqGetFriendInfos(new ReqGetFriendInfos());
            if (rsp2.Code != 0)
            {
                Debug.LogError(rsp2.Msg);
            }
            player.friendInfos = rsp2.OtherPlayerInfos.ToList();

            // 聊天记录
            player.allChatRecords = new Dictionary<int, List<NChatMsgRecordItem>>();
            foreach (var friendInfo in player.friendInfos)
            {
                var req3 = new ReqGetChatRecords
                {
                    FriendUId = friendInfo.UId
                };
                var rsp3 = await NetFn.ReqGetChatRecords(req3);
                if (rsp3.Code != 0)
                {
                    Debug.LogError(rsp3.Msg);
                }
                player.allChatRecords.Add(friendInfo.UId, rsp3.ChatMsgRecordItems.ToList());
            }

            // 任务进度
            player.questProgresses = new List<(int questChainCid, int currentQuestCid)>
            {
                (1, 1),
            };
        }

        public static async UniTask FetchInventoryData()
        {
            // 驱动盘信息
            var rsp1 = await NetFn.ReqGetDiscItems(new ReqGetDiscItems());
            if (rsp1.Code != 0)
            {
                Debug.LogWarning(rsp1.Msg);
            }
            player.discs = rsp1.Items.Select(it => new DiscItem(it)).ToList();

            // 武器信息
            var rsp2 = await NetFn.ReqGetWeaponItems(new ReqGetWeaponItems());
            player.weapons = rsp2.Items.Select(it => new WeaponItem(it)).ToList();
        }
    }
}