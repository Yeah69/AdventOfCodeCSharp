namespace AdventOfCode.Days;

internal abstract class DayBase<T> : IDay where T : class, IDay
{
    public abstract int Number { get; }

    public string Input
    {
        get
        {
            var numberText = Number.TwoDigits();
            var sampleNumberText = SampleNumber is 0 or null
                ? string.Empty
                : $".{SampleNumber.Value.TwoDigits()}";
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
        while (Inputs.ResourceManager.GetString($"{Number.TwoDigits()}.{(i + 1).TwoDigits()}") is not null)
        {
            yield return PrintSolutionWrappingFactory(TrackTimeWrappingFactory(SampleFactory(++i)));
        }
    }

    public void PrintLongDayLabel(bool? isFirstPart = null)
    {
        ConsoleHelper.WriteColored($"Day {Number.TwoDigits()}", ConsoleColor.DarkBlue);
        if (SampleNumber is > 0)
        {
            Console.Write(" ");
            ConsoleHelper.WriteColored($"Sample {SampleNumber?.TwoDigits()}", ConsoleColor.DarkYellow);
        }
        if (isFirstPart is null) 
            return;
        Console.Write(" ");
        var partLabel = isFirstPart.Value ? "First" : "Second";
        var partColor = isFirstPart.Value ? ConsoleColor.DarkMagenta : ConsoleColor.DarkCyan;
        ConsoleHelper.WriteColored($"{partLabel} Part", partColor);
    }

    public void PrintShortDayLabel(bool isFirstPart)
    {
        ConsoleHelper.WriteColored(Number.TwoDigits(), ConsoleColor.DarkBlue);
        if (SampleNumber is > 0)
        {
            Console.Write(".");
            ConsoleHelper.WriteColored(SampleNumber?.TwoDigits() ?? "", ConsoleColor.DarkYellow);
        }
        var partLabel = isFirstPart ? "I" : "II";
        var partColor = isFirstPart ? ConsoleColor.DarkMagenta : ConsoleColor.DarkCyan;
        ConsoleHelper.WriteColored(partLabel, partColor);
    }

    public string GetShortDayLabelText(bool isFirstPart)
    {
        var samplePart = SampleNumber is > 0 ? $".{SampleNumber?.TwoDigits() ?? ""}" : "";
        var partLabel = isFirstPart ? "I" : "II";
        return $"{Number.TwoDigits()}{samplePart}{partLabel}";
    }
}