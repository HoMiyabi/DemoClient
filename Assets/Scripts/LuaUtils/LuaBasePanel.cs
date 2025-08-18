using System;
using Kirara.UI.Panel;
using Manager;
using UnityEngine;
using XLua;

namespace Kirara.LuaUtils
{
    [LuaCallCSharp]
    public class LuaBasePanel : AbstractBasePanel
    {
        public TextAsset luaScript;

        private LuaTable table;

        private Action<LuaTable> luaStart;
        private Action<LuaTable> luaUpdate;
        private Action<LuaTable> luaOnDestroy;

        private void Awake()
        {
            var luaEnv = LuaMgr.Instance.LuaEnv;

            // // 为每个脚本设置一个独立的脚本域，可一定程度上防止脚本间全局变量、函数冲突
            // table = luaEnv.NewTable();
            //
            // // 设置其元表的 __index, 使其能够访问全局变量
            // using (var meta = luaEnv.NewTable())
            // {
            //     meta.Set("__index", luaEnv.Global);
            //     table.SetMetaTable(meta);
            // }

            // 执行脚本
            object[] ret = luaEnv.DoString(luaScript.text, luaScript.name);
            table = (LuaTable)ret[0];
            table.Set("com", this);

            // 从 Lua 脚本域中获取定义的函数
            var bindUI = table.Get<Action<LuaTable>>("BindUI");
            var luaAwake = table.Get<Action<LuaTable>>("Awake");
            table.Get("Start", out luaStart);
            table.Get("Update", out luaUpdate);
            table.Get("OnDestroy", out luaOnDestroy);

            bindUI?.Invoke(table);
            luaAwake?.Invoke(table);
        }

        private void Start()
        {
            luaStart?.Invoke(table);
        }

        private void Update()
        {
            luaUpdate?.Invoke(table);
        }

        private void OnDestroy()
        {
            luaOnDestroy?.Invoke(table);

            table.Dispose();
            table = null;
            luaStart = null;
            luaUpdate = null;
            luaOnDestroy = null;
        }
    }
}