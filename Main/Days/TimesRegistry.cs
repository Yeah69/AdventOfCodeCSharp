using MrMeeseeks.DIE.UserUtility;

namespace AdventOfCode.Days;

internal interface ITimesPrinter
{
    void Print();
}

internal interface ITimesRegistry
{
    void Register(IDay day, Stage stage, TimeSpan duration);
}

internal class TimesRegistry : ITimesRegistry, ITimesPrinter, IScopeInstance
{
    private readonly List<(IDay Day, Stage Stage, TimeSpan Duration)> _results = [];
    public void Register(IDay day, Stage stage, TimeSpan duration) => _results.Add((day, stage, duration));
    public void Print()
    {
        var maybeMaxTime = _results.Count > 0 
            ? _results.MaxBy(x => x.Duration).Duration
            : (TimeSpan?)null;
        Console.WriteLine("Times Summary:");
        if (maybeMaxTime is { } maxTime)
        {
            var maxLabelLength = _results.Select(t => t.Day.GetShortDayLabelText(Stage.SecondPart)).Max(x => x.Length);
            var timeSpanTextLength = new TimeSpan(0, 23, 59, 59, 999, 999).ToString().Length;
            var barLength = Console.WindowWidth - maxLabelLength - 2 /* delimiting spaces */ - timeSpanTextLength;
            
            var timeUnit = maxTime.TotalMilliseconds / barLength;
            var greenUnitCount = (int) (Consts.TotalTaskYellowMilliseconds / timeUnit);
            var yellowUnitCount = (int) (Consts.TotalTaskRedMilliseconds / timeUnit) - greenUnitCount;
            var sortedTimeRecords = _results.OrderBy(x => x.Duration);
            
            foreach (var (day, stage, duration) in sortedTimeRecords)
            {
                var dayLabelLength = day.GetShortDayLabelText(stage).Length;
                var timeUnitCount = (int) (duration.TotalMilliseconds / timeUnit);
                var greens = Math.Max(Math.Min(timeUnitCount, greenUnitCount), 1);
                timeUnitCount -= greenUnitCount;
                var yellows = timeUnitCount > 0 ? Math.Min(timeUnitCount, yellowUnitCount) : 0;
                timeUnitCount -= yellowUnitCount;
                var reds = Math.Max(timeUnitCount, 0);
                var remainingSpaces = barLength - greens - yellows - reds;
                day.PrintShortDayLabel(stage);
                Console.Write($"{"".PadRight(maxLabelLength - dayLabelLength, ' ')} ");
                ConsoleHelper.WriteColored("".PadRight(greens, '|'), ConsoleColor.DarkGreen);
                ConsoleHelper.WriteColored("".PadRight(yellows, '|'), ConsoleColor.DarkYellow);
                ConsoleHelper.WriteColored("".PadRight(reds, '|'), ConsoleColor.DarkRed);
                Console.Write("".PadRight(remainingSpaces, ' '));
                Console.Write(" ");
                ConsoleHelper.PrintTaskTime(duration);
                Console.WriteLine();
            }
        }
        else
            Console.WriteLine("No times to display.");

        Console.WriteLine();
    }
}