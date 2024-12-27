namespace AdventOfCode.Days;

internal class Day05 : DayBase<Day05, Day05.Data>
{
    internal record Rule(long First, long Second);
    internal record Data(IReadOnlyList<Rule> Rules, IReadOnlyList<IReadOnlyList<long>> Updates);
    
    public override int Number => 5;

    protected override Data ParseInput()
    {
        var rules = new List<Rule>();
        var updates = new List<IReadOnlyList<long>>();
        
        var inputSpan = Input.AsSpan();
        var sectionRanges = inputSpan.Split(Environment.NewLine + Environment.NewLine);
        
        if(sectionRanges.MoveNext() is false) 
            throw new();
        var rulesSpan = inputSpan[sectionRanges.Current];
        var rulesRanges = rulesSpan.Split(Environment.NewLine);
        foreach (var rulesRange in rulesRanges)
        {
            var ruleSpan = rulesSpan[rulesRange];
            var ruleNumbersRanges = ruleSpan.Split('|');
            if(ruleNumbersRanges.MoveNext() is false) 
                throw new();
            var first = long.Parse(ruleSpan[ruleNumbersRanges.Current]);
            if(ruleNumbersRanges.MoveNext() is false) 
                throw new();
            var second = long.Parse(ruleSpan[ruleNumbersRanges.Current]);
            rules.Add(new(first, second));
        }
        
        if(sectionRanges.MoveNext() is false) 
            throw new();
        var updatesSpan = inputSpan[sectionRanges.Current];
        var updatesRanges = updatesSpan.Split(Environment.NewLine);
        foreach (var updatesRange in updatesRanges)
        {
            var update = new List<long>();
            var updateSpan = updatesSpan[updatesRange];
            var updateNumbersRanges = updateSpan.Split(',');
            foreach (var updateNumbersRange in updateNumbersRanges)
                update.Add(long.Parse(updateSpan[updateNumbersRange]));
            updates.Add(update);
        }
        
        return new(rules, updates);
    }

    private abstract record CheckResult
    {
        internal record Success : CheckResult;
        internal record Fail(int FirstIndex, int SecondIndex) : CheckResult;
    }

    private static CheckResult Check(IReadOnlyList<Rule> rules, IReadOnlyList<long> update)
    {
        foreach (var rule in rules)
        {
            var firstPageIndex = GetPageIndex(rule.First);
            var secondPageIndex = GetPageIndex(rule.Second);

            if(firstPageIndex != -1 && secondPageIndex != -1 && firstPageIndex > secondPageIndex)
                return new CheckResult.Fail(firstPageIndex, secondPageIndex);
            continue;

            int GetPageIndex(long page) => 
                update.Select((u, i) => (u, i)).FirstOrDefault(t => t.Item1 == page, (-1L, -1)).Item2;
        }
        return new CheckResult.Success();
    }
    
    public override string FirstPart()
    {
        var result = 0L;
        
        var data = ParsedInput.Value;
        foreach (var update in data.Updates)
        {
            var check = Check(data.Rules, update);
            if (check is CheckResult.Success)
                result += update[update.Count / 2];
        }
        
        return result.ToString();
    }

    public override string SecondPart()
    {
        var result = 0L;
        
        var data = ParsedInput.Value;
        foreach (var updateOriginal in data.Updates)
        {
            var update = updateOriginal.ToArray();
            var check = Check(data.Rules, update);
            if (check is CheckResult.Success)
                continue;
            while (check is CheckResult.Fail {FirstIndex: var firstIndex, SecondIndex: var secondIndex})
            {
                var first = update[firstIndex];
                var second = update[secondIndex];
                update[firstIndex] = second;
                update[secondIndex] = first;
                
                check = Check(data.Rules, update);
            }
            result += update[update.Length / 2];
        }
        
        return result.ToString();
    }
}