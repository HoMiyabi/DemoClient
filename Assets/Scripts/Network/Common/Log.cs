using System;

namespace Kirara.Network
{
    public static class Log
    {
        public static void Debug(string text)
        {
#if UNITY_5_3_OR_NEWER
            UnityEngine.Debug.Log(text);
#else
            Console.WriteLine(text);
#endif
        }
    }
}