using System.Collections.Immutable;

namespace AdventOfCode.Days;

internal class Day02 : DayBase<Day02, List<ImmutableArray<long>>>
{
    public override int Number => 2;

    protected override List<ImmutableArray<long>> ParseInput()
    {
        var wholeReport = new List<ImmutableArray<long>>();
        var inputSpan = Input.AsSpan();
        var lineRanges = inputSpan.Split(Environment.NewLine);
        foreach (var lineRange in lineRanges)
        {
            var lineSpan = inputSpan[lineRange];
            var levelsRanges = lineSpan.Split(' ');
            var report = new List<long>();
            foreach (var levelsRange in levelsRanges)
            {
                report.Add(long.Parse(lineSpan[levelsRange]));
            }
            wholeReport.Add([..report]);
        }
        return wholeReport;
    }
    
    private static bool IsSafe(ImmutableArray<long> report)
    {
        var isAscending = report[0] < report[1];
        var isSafe = true;
        for (var i = 1; i < report.Length; i++)
        {
            var diff = report[i - 1] - report[i];
            if (diff is 0 or < -3 or > 3 || (isAscending && diff > 0) || (!isAscending && diff < 0))
            {
                isSafe = false;
                break;
            }
        }

        return isSafe;
    }
    
    public override string FirstPart() => ParsedInput.Value.Select(IsSafe).LongCount(isSafe => isSafe).ToString();

    public override string SecondPart()
    {
        var safeCount = 0L;
        
        foreach (var report in ParsedInput.Value)
        {
            if (IsSafe(report))
            {
                safeCount++;
                continue;
            }

            var isSafe = false;
            var i = -1;
            while (++i < report.Length && !isSafe)
                isSafe = IsSafe(report.RemoveAt(i));
            if (isSafe)
                safeCount++;
        }
        
        return safeCount.ToString();
    }
}