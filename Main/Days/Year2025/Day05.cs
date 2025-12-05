namespace AdventOfCode.Days.Year2025;

internal class Day05 : DayOfYear2025<Day05, (List<(long, long)>, List<long>)>
{
    public override int Number => 5;

    protected override (List<(long, long)>, List<long>) ParseInput()
    {
        var ranges = new List<(long, long)>();
        
        var span = Input.AsSpan();

        var topRanges = span.Split($"{Environment.NewLine}{Environment.NewLine}");
        topRanges.MoveNext();
        
        var rangesSpan = span[topRanges.Current];
        var lineRanges = rangesSpan.Split(Environment.NewLine);
        
        foreach (var lineRange in lineRanges)
        {
            var lineSpan = rangesSpan[lineRange];
            var boundaryRanges = lineSpan.Split('-');
            boundaryRanges.MoveNext();
            var lowerBoundary = long.Parse(lineSpan[boundaryRanges.Current]);
            boundaryRanges.MoveNext();
            var upperBoundary = long.Parse(lineSpan[boundaryRanges.Current]);
            ranges.Add((lowerBoundary, upperBoundary));
        }
        
        topRanges.MoveNext();

        var ids = new List<long>();
        
        var idsSpan = span[topRanges.Current];
        var idLinesRanges = idsSpan.Split(Environment.NewLine);
        
        foreach (var idLinesRange in idLinesRanges)
        {
            ids.Add(long.Parse(idsSpan[idLinesRange]));
        }
        
        return (ranges, ids);
    }

    public override string FirstPart()
    {
        var (ranges, ids) = ParsedInput.Value;
        var count = 0L;
        
        foreach (var id in ids)
        {
            if (ranges.Any(r => r.Item1 <= id &&  id <= r.Item2))
                ++count;
        }
        
        return count.ToString();
    }

    public override string SecondPart()
    {
        var (ranges, _) = ParsedInput.Value;

        var sequence = ranges
            .OrderBy(r => r.Item1)
            .ThenBy(r => r.Item2)
            .ToList();
        
        var mergedRanges = new List<(long, long)>();

        var current = sequence[0];

        foreach (var range in sequence.Skip(1))
        {
            if (current.Item2 < range.Item1)
            {
                mergedRanges.Add(current);
                current = range;
            }
            else
            {
                current = (Math.Min(current.Item1, range.Item1), Math.Max(current.Item2, range.Item2));
            }
        }
        mergedRanges.Add(current);
        

        var count = 0L;
        foreach (var range in mergedRanges)
            count += range.Item2 - range.Item1 + 1;
        
        return count.ToString();
    }
}