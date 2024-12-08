using MrMeeseeks.DIE.UserUtility;

namespace AdventOfCode.Days;

internal interface IResultsPrinter
{
    void Print();
}

internal interface IResultsRegistry
{
    void Register(IDay day, bool isFirstPart, ResultStatus status);
}

internal class ResultsRegistry : IResultsRegistry, IResultsPrinter, IScopeInstance
{
    private readonly Dictionary<ResultStatus, List<(IDay Day, bool IsFirstPart)>> _results = new()
    {
        [ResultStatus.Incorrect] = [],
        [ResultStatus.Uncertain] = [],
        [ResultStatus.Correct] = []
    };
    public void Register(IDay day, bool isFirstPart, ResultStatus status) => _results[status].Add((day, isFirstPart));
    public void Print()
    {
        Console.WriteLine("Results Summary:");
        foreach (var (resultStatus, tasks) in _results.OrderBy(x => x.Key))
        {
            ConsoleHelper.WriteColored(resultStatus.ToChar().ToString(), resultStatus.ToConsoleColor());
            Console.Write(" ");
            if (tasks.Count == 0)
                Console.WriteLine("None");
            else
            {
                var first = tasks.First();
                first.Day.PrintShortDayLabel(first.IsFirstPart);
                foreach (var (day, isFirstPart) in tasks.Skip(1))
                {
                    Console.Write(", ");
                    day.PrintShortDayLabel(isFirstPart);
                }
                Console.WriteLine();
            }
        }
        Console.WriteLine();
    }
}