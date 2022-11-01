namespace Imagegram.Tests;

public static class HelperExtensions
{
    private static readonly Random Randomizer = new();
    
    public static T Random<T>(this IList<T> array)
    {
        var randomIndex = Randomizer.Next(0, array.Count - 1);

        return array[randomIndex];
    }
}