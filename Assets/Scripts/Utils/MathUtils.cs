public static class MathUtils
{
    public static int Repeat(int t, int length)
    {
        return (t % length + length) % length;
    }
}