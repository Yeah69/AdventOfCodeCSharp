namespace AdventOfCode.Days.Pickers;

internal class PickLatestOnly : IDayPicker
{
    public IEnumerable<IDay> PickDay(IReadOnlyList<IDay> days)
    {
        if (days.OrderByDescending(d => d.Number).FirstOrDefault() is { } latest)
        {
            yield return latest;
        }
    }
}