namespace AdventOfCode.Days.Year2025;

internal class Day04 : DayOfYear2025<Day04, HashSet<(long X, long Y)>>
{
    public override int Number => 4;

    protected override HashSet<(long X, long Y)> ParseInput()
    {
        var span = Input.AsSpan();
        var set =  new HashSet<(long X, long Y)>();

        var lineRanges = span.Split(Environment.NewLine);
        var y = 0L;
        foreach (Range lineRange in lineRanges)
        {
            var lineSpan = span[lineRange];
            var x = 0L;
            foreach (var c in lineSpan)
            {
                if (c is '@') 
                    set.Add((x, y));
                ++x;
            }

            ++y;
        }

        return set;
    }

    public override string FirstPart() => 
        CanBeRemoved(ParsedInput.Value).Count().ToString();

    public override string SecondPart()
    {
        var positions = ParsedInput.Value.ToHashSet();
        var count = 0L;
        
        while (true)
        {
            var toBeRemoved = CanBeRemoved(positions).ToArray();
            count += toBeRemoved.Length;
            positions.ExceptWith(toBeRemoved);
            if (toBeRemoved.Length == 0)
                break;
        }
        
        return count.ToString();
    }

    private IEnumerable<(long X, long Y)> CanBeRemoved(HashSet<(long X, long Y)> positions) =>
        positions.Where(p =>
        {
            var (x, y) = p;
            (long X, long Y)[] neigbors =
            [
                (x - 1, y - 1), (x, y - 1), (x + 1, y - 1),
                (x - 1, y), (x + 1, y),
                (x - 1, y + 1), (x, y + 1), (x + 1, y + 1)
            ];
            return neigbors.Count(positions.Contains) < 4;
        });
}