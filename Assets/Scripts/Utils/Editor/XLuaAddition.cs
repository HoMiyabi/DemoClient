using System;
using System.Collections.Generic;
using cfg.main;
using XLua;

namespace Kirara
{
    public static class XLuaAddition
    {
        [LuaCallCSharp]
        public static List<Type> myLuaCallCSharpList = new()
        {
            // typeof(EAttrType)
        };
    }
}