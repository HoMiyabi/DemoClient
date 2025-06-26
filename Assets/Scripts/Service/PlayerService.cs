using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Kirara.Model;
using Kirara.Service;

namespace Kirara
{
    public static class PlayerService
    {
        public static PlayerModel player;

        public static async UniTask FetchData()
        {
            // 获取物品信息
            InventoryService.Init();

            // 玩家信息
            var rsp = await NetFn.ReqGetPlayerData(new ReqGetPlayerData());
            player = new PlayerModel(rsp.PlayerData);

            // 任务进度
            player.questProgresses = new List<(int questChainCid, int currentQuestCid)>
            {
                (1, 1),
            };
        }
    }
}