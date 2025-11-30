namespace AdventOfCode.Days.Pickers;

internal class PickSpecific(long number, long? sampleNumber) : IDayPicker
{
    public IEnumerable<IDay> PickDays(IReadOnlyList<IDay> days) => days
        .Where(d => !string.IsNullOrEmpty(d.Input))
        .Where(d => d.Number == number)
        .SelectMany(d => sampleNumber > 0 ? d.Samples().Append(d) : [d])
        .Where(d => d.SampleNumber == sampleNumber);
}