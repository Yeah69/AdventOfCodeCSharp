namespace AdventOfCode.Days.Pickers;

internal class PickAll : IDayPicker
{
    public IEnumerable<IDay> PickDay(IReadOnlyList<IDay> days) => days.OrderBy(d => d.Number);
}