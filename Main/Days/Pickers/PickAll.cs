namespace AdventOfCode.Days.Pickers;

internal class PickAll : IDayPicker
{
    public IEnumerable<IDay> PickDay(IReadOnlyList<IDay> days) => days
        .Where(d => !string.IsNullOrEmpty(d.Input))
        .OrderBy(d => d.Number);
}