using cfg;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Manager
{
    public static class ConfigMgr
    {
        public static Tables tb { get; private set; }

        public static void LoadTables()
        {
            tb = new Tables(TableLoader);
        }

        private static JArray TableLoader(string filename)
        {
            var handle = AssetMgr.Instance.package.LoadAssetSync<TextAsset>(filename);
            var textAsset = handle.GetAssetObject<TextAsset>();
            var jArray = JArray.Parse(textAsset.text);
            handle.Release();
            return jArray;
        }
    }
}