using MrMeeseeks.DIE.UserUtility;

namespace AdventOfCode.Days;

internal class DayDecoratorPrintSolution : IDay, IDecorator<IDay>
{
    private readonly IDay _inner;

    internal DayDecoratorPrintSolution(IDay inner) => _inner = inner;

    public int Number => _inner.Number;
    public int? SampleNumber => _inner.SampleNumber;
    public string Input => _inner.Input;
    public string FirstPart()
    {
        var result = _inner.FirstPart();
        PrintSolution(result, "First");
        return result;
    }

    public string SecondPart()
    {
        var result = _inner.SecondPart();
        PrintSolution(result, "Second");
        return result;
    }

    public IEnumerable<IDay> Samples() => _inner.Samples();

    private void PrintSolution(string solution, string partLabel)
    {
        var samplePart = SampleNumber is > 0 ? $".{SampleNumber}" : string.Empty;
        var taskPart = partLabel == "First" ? "I" : "II";
        var knownSolution = Solutions.ResourceManager.GetString($"{Number.ToString().PadLeft(2, '0')}{samplePart}{taskPart}");
        var resultText = knownSolution is null 
            ? ("?", ConsoleColor.DarkYellow)
            : solution == knownSolution ? ("\u2713", ConsoleColor.Green) : ("X", ConsoleColor.Red);
        DayHelper.WriteDayLabel(_inner, partLabel);
        Console.Write(" Solution: ");
        ConsoleHelper.WriteLineColored(resultText.Item1, resultText.Item2);
        var foregroundColor = solution is Consts.NoSolutionFound or Consts.NothingToDoHere 
            ? ConsoleColor.DarkRed 
            : ConsoleColor.DarkGreen;
        ConsoleHelper.WriteLineColored(solution, foregroundColor);
        Console.WriteLine();
    }
}