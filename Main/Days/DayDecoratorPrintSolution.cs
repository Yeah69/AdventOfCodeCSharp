using MrMeeseeks.DIE.UserUtility;

namespace AdventOfCode.Days;

internal class DayDecoratorPrintSolution : IDay, IDecorator<IDay>
{
    private readonly IDay _inner;
    private readonly ICorrectnessRegistry _correctnessRegistry;

    internal DayDecoratorPrintSolution(IDay inner, ICorrectnessRegistry correctnessRegistry)
    {
        _inner = inner;
        _correctnessRegistry = correctnessRegistry;
    }

    public int Number => _inner.Number;
    public int? SampleNumber => _inner.SampleNumber;
    public string Input => _inner.Input;
    public string FirstPart()
    {
        var result = _inner.FirstPart();
        PrintSolution(result, true);
        return result;
    }

    public string SecondPart()
    {
        var result = _inner.SecondPart();
        PrintSolution(result, false);
        return result;
    }

    public IEnumerable<IDay> Samples() => _inner.Samples();
    public void PrintLongDayLabel(bool? isFirstPart = null) => _inner.PrintLongDayLabel(isFirstPart);
    public void PrintShortDayLabel(bool isFirstPart) => _inner.PrintShortDayLabel(isFirstPart);
    public string GetShortDayLabelText(bool isFirstPart) => _inner.GetShortDayLabelText(isFirstPart);

    private void PrintSolution(string solution, bool isFirstPart)
    {
        var samplePart = SampleNumber is > 0 ? $".{(SampleNumber ?? 0).TwoDigits()}" : string.Empty;
        var taskPart = isFirstPart ? "I" : "II";
        var taskLabel = $"{Number.TwoDigits()}{samplePart}{taskPart}";
        var knownSolution = Solutions.ResourceManager.GetString(taskLabel);
        var result = knownSolution is null 
            ? CorrectnessStatus.Uncertain
            : solution == knownSolution ? CorrectnessStatus.Correct : CorrectnessStatus.Incorrect;
        _correctnessRegistry.Register(this, isFirstPart, result);
        _inner.PrintLongDayLabel(isFirstPart);
        Console.Write(" Solution: ");
        ConsoleHelper.WriteLineColored(result.ToChar().ToString(), result.ToConsoleColor());
        var foregroundColor = solution is Consts.NoSolutionFound or Consts.NothingToDoHere 
            ? ConsoleColor.DarkRed 
            : ConsoleColor.DarkGreen;
        ConsoleHelper.WriteLineColored(solution, foregroundColor);
        Console.WriteLine();
    }
}