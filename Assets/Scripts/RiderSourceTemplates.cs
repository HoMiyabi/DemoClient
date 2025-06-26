using JetBrains.Annotations;

public static class RiderSourceTemplates
{
    [SourceTemplate]
    public static void obs<T>(this T x)
    {
        //$ if (x == value) return;
        //$ x = value;
    }

    [SourceTemplate]
    public static void sr<T>(this T x)
    {
        //$ x?.Release();
        //$ x = null;
    }
}