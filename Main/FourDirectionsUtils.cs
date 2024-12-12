namespace AdventOfCode;

internal static class FourDirectionsUtils
{
    internal static IEnumerable<(int X, int Y)> GetNeighbors((int X, int Y) currentPosition)
    {
        yield return (currentPosition.X - 1, currentPosition.Y);
        yield return (currentPosition.X + 1, currentPosition.Y);
        yield return (currentPosition.X, currentPosition.Y - 1);
        yield return (currentPosition.X, currentPosition.Y + 1);
    }
}