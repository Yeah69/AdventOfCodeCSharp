namespace AdventOfCode.Days.Pickers;

internal interface IDayPicker
{
    IEnumerable<IDay> PickDay(IReadOnlyList<IDay> days);
}