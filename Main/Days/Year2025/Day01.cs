using System.Collections.Immutable;

namespace AdventOfCode.Days.Year2025;

internal class Day01 : DayOfYear2025<Day01, ImmutableArray<long>>
{
    public override int Number => 1;

    protected override ImmutableArray<long> ParseInput()
    {
        var inputSpan = Input.AsSpan();
        var lineRanges = inputSpan.Split(Environment.NewLine);
        var parsedInput = new List<long>();
        foreach (var range in lineRanges)
        {
            var line = inputSpan[range];
            var number = long.Parse(line[1..]);
            parsedInput.Add(line[0] is 'R' ? number : -number);
        }

        return [..parsedInput];
    }

    private const long Start = 50;
    private const long MaxDial = 100;
    
    public override string FirstPart()
    {
        long countZeros = 0;
        var i = Start;
        foreach (var instruction in ParsedInput.Value)
        {
            i += instruction;
            while (i < 0) i += 100;
            i %= 100;
            if (i == 0) countZeros++;
        }
        return countZeros.ToString();
    }

    public override string SecondPart()
    {
        long countZeros = 0;
        var i = Start;
        foreach (var instruction in ParsedInput.Value)
        {
            var increment = instruction >= 0 ? 1 : -1;
            var absInstruction = Math.Abs(instruction);
            var distanceToZero = increment == 1 ? MaxDial - i : i;
            if (distanceToZero > 0 && absInstruction >= distanceToZero) countZeros++;
            countZeros += (absInstruction - distanceToZero) / MaxDial;
            
            i += increment * absInstruction;
            i %= MaxDial;
            if (i < 0) i += MaxDial;
        }
        return countZeros.ToString();
    }
}