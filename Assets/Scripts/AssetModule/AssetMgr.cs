using Kirara;
using YooAsset;

namespace Manager
{
    public class AssetMgr : UnitySingleton<AssetMgr>
    {
        public EPlayMode playMode;
        public string hostServer;

        public const float BToMB = 1 / 1048576f;
    }
}