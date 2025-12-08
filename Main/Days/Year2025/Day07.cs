using System.Collections.Immutable;

namespace AdventOfCode.Days.Year2025;

internal class Day07 : DayOfYear2025<Day07, ((long X, long Y) Start, HashSet<(long X, long Y)> Spliters)>
{
    public override int Number => 7;

    protected override ((long X, long Y) Start, HashSet<(long X, long Y)> Spliters) ParseInput()
    {
        var span = Input.AsSpan();
        var lineRanges = span.Split(Environment.NewLine);
        var start = (0L, 0L);
        var splitters = new HashSet<(long X, long Y)>();

        var y = 0L;
        foreach (var lineRange in lineRanges)
        {
            var lineSpan = span[lineRange];

            for (int x = 0; x < lineSpan.Length; x++)
            {
                if (lineSpan[x] == '^')
                    splitters.Add((x, y));
                else if (lineSpan[x] == 'S')
                    start = (x, y);
            }

            y++;
        }

        return (start, splitters);
    }

    public override string FirstPart()
    {
        var (start, splitters) = ParsedInput.Value;

        var splittingCount = 0L;

        var beams = ImmutableHashSet<long>.Empty.Add(start.X);
        var maxY = splitters.Max(s => s.Y) + 1;

        for (var y = start.Y; y <= maxY; ++y)
        {
            beams = beams.SelectMany(beamX =>
            {
                if (!splitters.Contains((beamX, y)))
                    return Enumerable.Empty<long>().Append(beamX);
                ++splittingCount;
                return Enumerable.Empty<long>().Append(beamX - 1).Append(beamX + 1);
            }).ToImmutableHashSet();
        }
        
        return splittingCount.ToString();
    }

    public override string SecondPart()
    {
        var (start, splitters) = ParsedInput.Value;

        var maxY = splitters.Max(s => s.Y) + 1;
        var cache = new Dictionary<(long X, long Y), long>();
        
        return GetAllTimelines(start.X, start.Y).ToString();

        long GetAllTimelines(long x, long y)
        {
            while (y != maxY && !splitters.Contains((x, y)))
                ++y;
            if (y == maxY)
                return 1L;
            if (cache.TryGetValue((x, y),  out var value))
                return value;
            var result = GetAllTimelines(x - 1, y) + GetAllTimelines(x + 1, y);
            cache.Add((x, y), result);
            return result;
        }
    }
}