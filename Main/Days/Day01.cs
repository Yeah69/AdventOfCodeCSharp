namespace AdventOfCode.Days;

internal class Day01 : DayBase<Day01, (List<long> Left, List<long> Right)>
{
    public override int Number => 1;

    protected override (List<long> Left, List<long> Right) ParseInput()
    {
        var inputSpan = Input.AsSpan();
        var lineRanges = inputSpan.Split(Environment.NewLine);
        var firstList = new List<long>();
        var secondList = new List<long>();
        foreach (var range in lineRanges)
        {
            var line = inputSpan[range];
            var numbersRanges = line.Split(' ');
            (long? first, long? second) tuple = (null, null);
            foreach (var numbersRange in numbersRanges)
            {
                if (numbersRange.Start.Value == numbersRange.End.Value) continue;
                if (tuple.first is null)
                    tuple.first = long.Parse(line[numbersRange]);
                else if (tuple.second is null)
                    tuple.second = long.Parse(line[numbersRange]);
            }

            if (tuple is not ({ } first, { } second)) continue;
            firstList.Add(first);
            secondList.Add(second);
        }
        return (firstList, secondList);
    }
    
    public override string FirstPart() => 
        ParsedInput.Value.Left.Order()
            .Zip(ParsedInput.Value.Right.Order(), (first, second) => Math.Abs(first - second))
            .Sum()
            .ToString();

    public override string SecondPart()
    {
        var rightGrouped = ParsedInput.Value.Right.CountBy(number => number).ToDictionary(group => group.Key, group => group.Value);
        return ParsedInput.Value.Left
            .Select(left => left * (rightGrouped.TryGetValue(left, out var value) ? value : 0L))
            .Sum()
            .ToString();
    }
}