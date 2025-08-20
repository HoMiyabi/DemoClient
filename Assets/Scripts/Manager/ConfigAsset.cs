using UnityEngine;
using YooAsset;

namespace Manager
{
    public static class ConfigAsset
    {
        public static AssetHandle GetIconInterKnotRole(int configId)
        {
            string loc = ConfigMgr.tb.TbIconInterKnotRoleConfig[configId].Location;
            var handle = YooAssets.LoadAssetSync<Sprite>(loc);
            return handle;
        }

        public static AssetHandle GetIconSticker(int configId)
        {
            string loc = ConfigMgr.tb.TbIconSticker[configId].Location;
            var handle = YooAssets.LoadAssetSync<Sprite>(loc);
            return handle;
        }
    }
}