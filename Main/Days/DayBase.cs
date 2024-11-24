namespace AdventOfCode.Days;

internal abstract class DayBase<TDay, TData> : IDay where TDay : class, IDay
{
    private protected readonly Lazy<TData> ParsedInput;
    
    protected DayBase() => ParsedInput = new Lazy<TData>(ParseInput);

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
    
    internal required Func<int?, TDay> SampleFactory { get; init; }
    
    internal required Func<IDay, DayDecoratorTrackTime> TrackTimeWrappingFactory { get; init; }
    
    internal required Func<IDay, DayDecoratorPrintSolution> PrintSolutionWrappingFactory { get; init; }

    public void PrepareInputParsing() => _ = ParsedInput.Value;
    protected abstract TData ParseInput();
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

    public void PrintLongDayLabel(Stage? stage)
    {
        ConsoleHelper.WriteColored($"Day {Number.TwoDigits()}", ConsoleColor.DarkBlue);
        if (SampleNumber is > 0)
        {
            Console.Write(" ");
            ConsoleHelper.WriteColored($"Sample {SampleNumber?.TwoDigits()}", ConsoleColor.DarkYellow);
        }
        if (stage is null) 
            return;
        var definitiveStage = stage.Value;
        Console.Write(" ");
        ConsoleHelper.WriteColored(definitiveStage.ToLongLabel(), definitiveStage.ToColor());
    }

    public void PrintShortDayLabel(Stage stage)
    {
        ConsoleHelper.WriteColored(Number.TwoDigits(), ConsoleColor.DarkBlue);
        if (SampleNumber is > 0)
        {
            Console.Write(".");
            ConsoleHelper.WriteColored(SampleNumber?.TwoDigits() ?? "", ConsoleColor.DarkYellow);
        }
        ConsoleHelper.WriteColored(stage.ToShortLabel(), stage.ToColor());
    }

    public string GetShortDayLabelText(Stage stage)
    {
        var samplePart = SampleNumber is > 0 ? $".{SampleNumber?.TwoDigits() ?? ""}" : "";
        return $"{Number.TwoDigits()}{samplePart}{stage.ToShortLabel()}";
    }
}