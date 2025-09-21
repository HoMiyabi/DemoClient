using System;
using System.Collections.Generic;
using DG.Tweening;
using XLua;

namespace Kirara
{
    public static class XLuaAddition
    {
        [LuaCallCSharp]
        public static List<Type> myLuaCallCSharpList = new()
        {
            typeof(KiraraDirectBinder.KiraraDirectBinder),
            typeof(DOTweenModuleUI),
        };
    }
}