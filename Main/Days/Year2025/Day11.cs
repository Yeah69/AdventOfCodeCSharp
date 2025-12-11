namespace AdventOfCode.Days.Year2025;

internal class Day11 : DayOfYear2025<Day11, Dictionary<string, List<string>>>
{
    public override int Number => 11;

    protected override Dictionary<string, List<string>> ParseInput()
    {
        var span = Input.AsSpan();
        var lineRanges = span.Split(Environment.NewLine);
        var map = new Dictionary<string, List<string>>();

        foreach (var lineRange in lineRanges)
        {
            var lineSpan = span[lineRange];
            var partRanges = lineSpan.Split(": ");
            partRanges.MoveNext();
            var key = lineSpan[partRanges.Current].ToString();
            partRanges.MoveNext();
            var outputsSpan = lineSpan[partRanges.Current];
            var outputsRanges = outputsSpan.Split(' ');
            var outputs = new List<string>();
            foreach (var outputsRange in outputsRanges)
            {
                outputs.Add(outputsSpan[outputsRange].ToString());
            }
            map.Add(key, outputs);
        }

        return map;
    }

    public override string FirstPart() => 
        GetPathCount("you", "out").ToString();

    public override string SecondPart()
    {
        var fftToDac = GetPathCount("fft", "dac");
        
        if (fftToDac is 0)
        {
            var svrToDac = GetPathCount("svr", "dac");
            var dacToFft = GetPathCount("dac", "fft");
            var fftToOut = GetPathCount("fft", "out");
            return (svrToDac * dacToFft * fftToOut).ToString();
        }
        
        var svrToFft = GetPathCount("svr", "fft");
        var dacToOut = GetPathCount("dac", "out");
        return (svrToFft * fftToDac * dacToOut).ToString();
    }

    private long GetPathCount(string start, string target)
    {
        var cache = new Dictionary<string, long>();

        return Inner(start);

        long Inner(string current)
        {
            if (cache.TryGetValue(current, out var count))
                return count;
            if (current == target)
                return 1;
            if (!ParsedInput.Value.TryGetValue(current, out var outputs)) 
                return 0;
            
            var sum = outputs.Sum(Inner);
            cache.Add(current, sum);
            return sum;
        }
    }
}