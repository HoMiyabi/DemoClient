using System;
using System.Collections.Generic;

namespace KiraraDirectBinder.Editor
{
    public static class KiraraDirectBinderAlias
    {
        /// <summary>
        /// 类型别名，在绑定窗口中，GameObject名字包含别名，会放到更前的位置
        /// </summary>
        public static readonly Dictionary<Type, List<string>> alias = new();
    }
}