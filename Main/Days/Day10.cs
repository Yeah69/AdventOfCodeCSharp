﻿namespace AdventOfCode.Days;

internal class Day10 : DayBase<Day10>
{
    public override int Number => 10;
    private readonly Lazy<IReadOnlyList<IReadOnlyList<int>>> _input;
    internal Day10() => _input = new(ParseInput);

    private IReadOnlyList<IReadOnlyList<int>> ParseInput()
    {
        var map = new List<IReadOnlyList<int>>();
        
        var inputSpan = Input.AsSpan();
        var lineRanges = inputSpan.Split(Environment.NewLine);
        foreach (var lineRange in lineRanges)
        {
            var row = new List<int>();
            
            var lineSpan = inputSpan[lineRange];
            foreach (var c in lineSpan)
                row.Add(int.Parse(c.ToString()));
            
            map.Add(row);
        }
        return map;
    }
    
    private static IEnumerable<(int X, int Y)> GetNeighbors((int X, int Y) currentPosition)
    {
        yield return (currentPosition.X - 1, currentPosition.Y);
        yield return (currentPosition.X + 1, currentPosition.Y);
        yield return (currentPosition.X, currentPosition.Y - 1);
        yield return (currentPosition.X, currentPosition.Y + 1);
    }

    private string Solve(IReadOnlyList<IReadOnlyList<int>> map, Func<(int X, int Y), long> getScore) =>
        map.SelectMany((row, y) => row.Select((cell, x) => (cell, x, y)))
            .Where(c => c.cell == 0)
            .Select(c => (c.x, c.y))
            .Sum(getScore)
            .ToString();

    public override string FirstPart()
    {
        var map = _input.Value;
        return Solve(map, p => GetReachablePeaks(p).Distinct().Count());

        IEnumerable<(int X, int Y)> GetReachablePeaks((int X, int Y) currentPosition)
        {
            var currentHeight = map[currentPosition.Y][currentPosition.X];
            if (currentHeight == 9)
            {
                yield return currentPosition;
                yield break;
            }
            var nextPositions = GetNeighbors(currentPosition)
                .Where(n => n.X >= 0 && n.X < map[0].Count && n.Y >= 0 && n.Y < map.Count 
                            && map[n.Y][n.X] == currentHeight + 1)
                .SelectMany(GetReachablePeaks);
            foreach (var nextPosition in nextPositions)
                yield return nextPosition;
        }
    }

    public override string SecondPart()
    {
        var map = _input.Value;
        return Solve(map, GetPathCounts);

        long GetPathCounts((int X, int Y) currentPosition)
        {
            var currentHeight = map[currentPosition.Y][currentPosition.X];
            if (currentHeight == 9)
                return 1L;
            return GetNeighbors(currentPosition)
                .Where(n => n.X >= 0 && n.X < map[0].Count && n.Y >= 0 && n.Y < map.Count 
                            && map[n.Y][n.X] == currentHeight + 1)
                .Select(GetPathCounts)
                .Sum();
        }
    }
}