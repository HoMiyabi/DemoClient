using System;
using System.Collections.Generic;
using Kirara;
using UnityEngine;
using XLua;

namespace Manager
{
    public class LuaMgr : UnitySingleton<LuaMgr>
    {
        public LuaEnv LuaEnv { get; private set; }

        private const float GCInterval = 1f;
        private float lastGCTime;

        [CSharpCallLua]
        public delegate void AbilityTableInit(LuaTable abilityTable, Func<string, double> inject);

        public void Init()
        {
            LuaEnv = new LuaEnv();
            LuaEnv.AddLoader(LuaLoader);
            LuaEnv.DoString("require('main')");
        }

        private byte[] LuaLoader(ref string filepath)
        {
            var handle = AssetMgr.Instance.package.LoadAssetSync<TextAsset>(filepath + ".lua");
            var textAsset = (TextAsset)handle.AssetObject;
            handle.Release();
            return textAsset.bytes;
        }

        private void Update()
        {
            if (Time.time - lastGCTime > GCInterval)
            {
                LuaEnv?.Tick();
                lastGCTime = Time.time;
            }
        }

        private void OnDestroy()
        {
            LuaEnv.Dispose();
            LuaEnv = null;
        }
    }
}