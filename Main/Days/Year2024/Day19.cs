namespace AdventOfCode.Days.Year2024;

internal class Day19 : DayOfYear2024<Day19, Day19.Data>
{
    internal record Data(IReadOnlyList<string> Towels, IReadOnlyList<string> Designs);
    public override int Number => 19;

    protected override Data ParseInput()
    {
        var inputSpan = Input.AsSpan();
        var inputPartRanges = inputSpan.Split(Environment.NewLine + Environment.NewLine);
        
        if (!inputPartRanges.MoveNext())
            throw new();
        var towels = new List<string>();
        var towelsSpan = inputSpan[inputPartRanges.Current];
        var towelRanges = towelsSpan.Split(", ");
        foreach (var towelRange in towelRanges)
            towels.Add(towelsSpan[towelRange].ToString());
        
        if (!inputPartRanges.MoveNext())
            throw new();
        var designs = new List<string>();
        var designsSpan = inputSpan[inputPartRanges.Current];
        var designRanges = designsSpan.Split(Environment.NewLine);
        foreach (var design in designRanges)
            designs.Add(designsSpan[design].ToString());

        return new(towels, designs);
    }

    private static long Check(string designRemainder, IReadOnlyList<string> towels, Dictionary<string, long> solved)
    {
        if (solved.TryGetValue(designRemainder, out var count))
            return count;
        if (designRemainder.Length == 0)
            return 1L;
        var sum = towels
            .Where(designRemainder.StartsWith)
            .Sum(towel => Check(designRemainder[towel.Length..], towels, solved));
        solved[designRemainder] = sum;
        return sum;
    }

    public override string FirstPart() => 
        ParsedInput.Value.Designs.Count(d => Check(d, ParsedInput.Value.Towels, []) > 0).ToString();

    public override string SecondPart() => 
        ParsedInput.Value.Designs.Sum(d => Check(d, ParsedInput.Value.Towels, [])).ToString();
}