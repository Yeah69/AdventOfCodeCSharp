using MrMeeseeks.DIE.UserUtility;

namespace AdventOfCode.Days;

internal class DayDecoratorPrintSolution : IDay, IDecorator<IDay>
{
    private readonly IDay _inner;

    internal DayDecoratorPrintSolution(IDay inner)
    {
        _inner = inner;
    }
    
    public int Number => _inner.Number;
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
    
    private void PrintSolution(string solution, string partLabel)
    {
        DayHelper.WriteDayLabel(_inner, partLabel);
        Console.WriteLine(" Solution:");
        var foregroundColor = solution is Consts.NoSolutionFound or Consts.NothingToDoHere 
            ? ConsoleColor.DarkRed 
            : ConsoleColor.DarkGreen;
        ConsoleHelper.WriteLineColored(solution, foregroundColor);
        Console.WriteLine();
    }
}