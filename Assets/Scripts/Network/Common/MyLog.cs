namespace Kirara.Network
{
    public static class MyLog
    {
        public static void Debug(string text)
        {
#if UNITY_5_3_OR_NEWER
            UnityEngine.Debug.Log(text);
#else
            Serilog.Log.Debug(text);
#endif
        }

        public static void Error(string text)
        {
#if UNITY_5_3_OR_NEWER
            UnityEngine.Debug.LogError(text);
#else
            Serilog.Log.Error(text);
#endif
        }
    }
}