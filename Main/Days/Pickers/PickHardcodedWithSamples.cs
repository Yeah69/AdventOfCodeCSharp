namespace AdventOfCode.Days.Pickers;

internal class PickHardcodedWithSamples : IDayPicker
{
    public IEnumerable<IDay> PickDay(IReadOnlyList<IDay> days) => days
        .Where(d => !string.IsNullOrEmpty(d.Input))
        .Where(d => d.Number == 21)
        .SelectMany(d => d.Samples().Append(d));
}