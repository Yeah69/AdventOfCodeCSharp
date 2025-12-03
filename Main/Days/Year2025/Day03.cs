using System.Collections.Immutable;

namespace AdventOfCode.Days.Year2025;

internal class Day03 : DayOfYear2025<Day03, ImmutableArray<string>>
{
    public override int Number => 3;

    protected override ImmutableArray<string> ParseInput() => 
        [..Input.Split(Environment.NewLine)];

    public override string FirstPart() => 
        SolvePart(digits: 2);

    public override string SecondPart() => 
        SolvePart(digits: 12);

    private string SolvePart(int digits)
    {
        var sum = 0L;
    
        foreach (var line in ParsedInput.Value)
        {
            var tempLine = line;
            var digitChars = new char[digits];

            for (var i = digits; i > 0; i--)
            {
                var max = tempLine[..^(i-1)].Max();
                var maxIndex = tempLine.IndexOf(max);
                tempLine = tempLine[(maxIndex+1)..];
                digitChars[digits - i] = max;
            }
            
            sum += long.Parse(new string(digitChars));
        }
    
        return sum.ToString();
    }
}