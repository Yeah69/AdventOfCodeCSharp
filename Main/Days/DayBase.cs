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

    public required int? SampleNumber { get; init; }
    
    internal required Func<int?, T> SampleFactory { get; init; }
    
    internal required Func<IDay, DayDecoratorTrackTime> TrackTimeWrappingFactory { get; init; }
    
    internal required Func<IDay, DayDecoratorPrintSolution> PrintSolutionWrappingFactory { get; init; }
    
    public abstract string FirstPart();
    public abstract string SecondPart();
    public IEnumerable<IDay> Samples()
    {
        var i = 0;
        while (Inputs.ResourceManager.GetString($"{Number.ToString().PadLeft(2, '0')}.{(i + 1).ToString().PadLeft(2, '0')}") is not null)
        {
            yield return PrintSolutionWrappingFactory(TrackTimeWrappingFactory(SampleFactory(++i)));
        }
    }
}