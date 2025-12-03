using System.Collections.Immutable;

namespace AdventOfCode.Days.Year2025;

internal class Day02 : DayOfYear2025<Day02, ImmutableArray<(long, long)>>
{
    public override int Number => 2;

    protected override ImmutableArray<(long, long)> ParseInput()
    {
        var inputSpan = Input.AsSpan();
        var ranges = inputSpan.Split(',');
        var parsedInput = new List<(long, long)>(inputSpan.CountAny(',') + 1);
        foreach (var rangeRange in ranges)
        {
            var range = inputSpan[rangeRange];
            var boundaryRanges = range.Split('-');
            boundaryRanges.MoveNext();
            var lowerBoundary = long.Parse(range[boundaryRanges.Current]);
            boundaryRanges.MoveNext();
            var upperBoundary = long.Parse(range[boundaryRanges.Current]);
            parsedInput.Add((lowerBoundary, upperBoundary));
        }

        return [..parsedInput];
    }
    
    public override string FirstPart()
    {
        var countInvalidIds = 0L;
        
        foreach (var (lowerBoundary, upperBoundary) in ParsedInput.Value)
        {
            for (var i = lowerBoundary; i <= upperBoundary; i++)
            {
                var asText = i.ToString();
                if (asText.Length % 2 is 1)
                    continue;
                var half = asText.Length / 2;
                var asSpan = asText.AsSpan();
                if (asSpan[..half].ToString() == asSpan[half..].ToString())
                    countInvalidIds += i;
            }
        }
        
        return countInvalidIds.ToString();
    }

    public override string SecondPart()
    {
        var countInvalidIds = 0L;
        
        foreach (var (lowerBoundary, upperBoundary) in ParsedInput.Value)
        {
            for (var i = lowerBoundary; i <= upperBoundary; i++)
            {
                var asText = i.ToString();
                var halfLength = asText.Length / 2;
                for (var j = 1; j <= halfLength; j++)
                {
                    if (asText.Length % j > 0)
                        continue;
                    var ranges = Enumerable.Range(0, asText.Length / j)
                        .Select(k => new Range(k * j, (k + 1) * j))
                        .ToArray();
                    var check = asText[ranges.First()];
                    if (ranges.Skip(1).All(r => check == asText[r]))
                    {
                        countInvalidIds += i;
                        break;
                    }
                }
            }
        }
        
        return countInvalidIds.ToString();
    }
}