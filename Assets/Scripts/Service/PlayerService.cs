using Cysharp.Threading.Tasks;
using Kirara.Model;

namespace Kirara
{
    public static class PlayerService
    {
        public static Player player;

        public static async UniTask FetchData()
        {
            // 玩家信息
            var rsp = await NetFn.ReqGetPlayerData(new ReqGetPlayerData());
            player = new Player(rsp.PlayerData);
        }
    }
}