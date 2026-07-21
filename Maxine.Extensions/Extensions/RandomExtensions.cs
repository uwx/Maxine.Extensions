namespace Maxine.Extensions;

public static class RandomExtensions
{
    public static bool NextBoolean(this Random rand)
    {
        return rand.Next() % 2 == 0;
    }
}