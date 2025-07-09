using System;
using System.Collections.Generic;
using Kirara;
using Kirara.AttrEffect;
using NUnit.Framework;
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

            // LuaEnv.DoString("weaponAbilities = require('ConfigAbility_Weapon_Setup')");
            // var weaponAbilitiesTable = LuaEnv.Global.Get<LuaTable>("weaponAbilities");
            // var list = new List<Ability>();
            // int len = weaponAbilitiesTable.Length;
            // for (int i = 1; i <= len; i++)
            // {
            //     var ability = weaponAbilitiesTable.Get<int, Ability>(i);
            //     list.Add(ability);
            // }
            // Debug.Log("ability.name = " + list[0].name);
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