namespace AdventOfCode.Days;

internal abstract class DayBase<T> : IDay where T : class, IDay
{
    public abstract int Number { get; }

    public string Input
    {
        get
        {
            var numberText = Number.ToString().PadLeft(2, '0');
            var sampleNumberText = SampleNumber is 0 or null
                ? string.Empty
                : $".{SampleNumber.Value.ToString().PadLeft(2, '0')}";
            return Inputs.ResourceManager.GetString($"{numberText}{sampleNumberText}") ?? string.Empty;
        }
    }

    internal required int? SampleNumber { get; init; }
    
    public abstract string FirstPart();
    public abstract string SecondPart();
}