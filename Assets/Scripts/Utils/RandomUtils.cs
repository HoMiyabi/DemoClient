namespace Kirara
{
    public static class RandomUtils
    {
        public static bool Bool => UnityEngine.Random.Range(0, 2) == 0;
    }
}