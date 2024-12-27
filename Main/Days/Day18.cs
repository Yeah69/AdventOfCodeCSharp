using System.Collections.Immutable;

namespace AdventOfCode.Days;

internal class Day18 : DayBase<Day18, Day18.Data>
{
    internal record Data(
        IReadOnlyList<(long X, long Y)> Bytes, 
        long Width, 
        long Height, 
        ImmutableHashSet<(long X, long Y)> Walls,
        long InitiallyFallenBytes);
    public override int Number => 18;

    protected override Data ParseInput()
    {
        var inputSpan = Input.AsSpan();
        var lineRanges = inputSpan.Split(Environment.NewLine);
        var bytes = new List<(long X, long Y)>();
        
        foreach (var lineRange in lineRanges)
        {
            var lineSpan = inputSpan[lineRange];
            var byteRanges = lineSpan.Split(",");
            if(!byteRanges.MoveNext())
                throw new Exception("No parts found");
            var x = long.Parse(lineSpan[byteRanges.Current]);
            if(!byteRanges.MoveNext())
                throw new Exception("No parts found");
            var y = long.Parse(lineSpan[byteRanges.Current]);
            bytes.Add((x, y));
        }
        
        var width = SampleNumber > 0 ? 7L : 71L;
        var height = SampleNumber > 0 ? 7L : 71L;
        var fallenBytes = SampleNumber > 0 ? 12 : 1024;
        
        var walls = bytes.Take(fallenBytes)
            .Concat(Enumerable.Range(-1, (int) width + 2).SelectMany(x => new[] {(X: (long) x, Y: -1L), (X: x, Y: height)}))
            .Concat(Enumerable.Range(0, (int) height).SelectMany(y => new[] {(-1L, (long) y), (width, y)}))
            .ToImmutableHashSet();
        
        return new Data(bytes, width, height, walls, fallenBytes);
    }

    private static string Solve(long width, long height, ImmutableHashSet<(long, long)> walls)
    {
        var start = (X: 0L, Y: 0L);
        var end = (X: width - 1, Y: height - 1);
        
        var queue = new Queue<((long X, long Y) Position, long steps)>();
        queue.Enqueue((start, 0L));
        
        var visitedNodes = new HashSet<(long X, long Y)>();
        
        while (queue.Count > 0)
        {
            var (position, steps) = queue.Dequeue();
            if (position == end)
                return steps.ToString();
            if (!visitedNodes.Add(position))
                continue;
            if (walls.Contains(position))
                continue;
            foreach (var next in FourDirectionsUtils.GetNeighbors(position))
                queue.Enqueue((next, steps + 1));
        }
        
        return Consts.NoSolutionFound;
    }

    public override string FirstPart() => Solve(ParsedInput.Value.Width, ParsedInput.Value.Height, ParsedInput.Value.Walls);

    public override string SecondPart()
    {
        var width = ParsedInput.Value.Width;
        var height = ParsedInput.Value.Height;
        var fallenBytes = ParsedInput.Value.InitiallyFallenBytes;
        
        var walls = ParsedInput.Value.Bytes.Take((int) fallenBytes)
            .Concat(Enumerable.Range(-1, (int) width + 2).SelectMany(x => new[] {(X: (long) x, Y: -1L), (X: x, Y: height)}))
            .Concat(Enumerable.Range(0, (int) height).SelectMany(y => new[] {(-1L, (long) y), (width, y)}))
            .ToImmutableHashSet();
        
        var threshold = BinarySearchUtils.ThresholdSearch(
            (int) ParsedInput.Value.InitiallyFallenBytes + 1, 
            ParsedInput.Value.Bytes.Count - 1, 
            i =>
            {
                var addition = ImmutableHashSet.CreateRange(
                    Enumerable.Range(
                        (int) ParsedInput.Value.InitiallyFallenBytes + 1, 
                        i - (int) ParsedInput.Value.InitiallyFallenBytes).Select(j => ParsedInput.Value.Bytes[j]));
                return Solve(width, height, walls.Union(addition)) != Consts.NoSolutionFound;
            });
        var invalidatingByte = ParsedInput.Value.Bytes[threshold];
        return $"{invalidatingByte.X},{invalidatingByte.Y}";
    }
}