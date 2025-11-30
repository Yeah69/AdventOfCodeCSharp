namespace AdventOfCode.Days.Year2024;

internal class Day25 : DayOfYear2024<Day25, Day25.Data>
{
    internal record Data(IReadOnlyList<IReadOnlyList<int>> Locks, IReadOnlyList<IReadOnlyList<int>> Keys, int Height);
    public override int Number => 25;

    protected override Data ParseInput()
    {
        var locks = new List<IReadOnlyList<int>>();
        var keys = new List<IReadOnlyList<int>>();
        var lockOrKeySections = Input.Split(Environment.NewLine + Environment.NewLine);

        var height = -1;
        foreach (var lockOrKeySection in lockOrKeySections)
        {
            var lines = lockOrKeySection.Split(Environment.NewLine);
            if (lines[0].All(c => c is '#'))
            {
                var combination = lines[0].Select((_, x) => Enumerable.Range(0, lines.Length).TakeWhile(y => lines[y][x] is '#').Count() - 1).ToList();
                locks.Add(combination);
            }
            else
            {
                var combination = lines[0].Select((_, x) => Enumerable.Range(0, lines.Length).Reverse().TakeWhile(y => lines[y][x] is '#').Count() - 1).ToList();
                keys.Add(combination);
            }
            height = lines.Length - 2;
        }
        
        return new(locks, keys, height);
    }

    public override string FirstPart() =>
        ParsedInput.Value.Locks
            .Sum(dataLock => ParsedInput.Value.Keys
                .Select(dataKey => !dataLock.Where((t, i) => t + dataKey[i] > ParsedInput.Value.Height).Any())
                .Count(fit => fit))
            .ToString();

    public override string SecondPart() => Consts.NothingToDoHere;
}