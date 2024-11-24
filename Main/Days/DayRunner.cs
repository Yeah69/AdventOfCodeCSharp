using System.Diagnostics;
using AdventOfCode.Days.Pickers;
using MrMeeseeks.DIE.UserUtility;

namespace AdventOfCode.Days;

internal interface IDayRunner
{
    void Run();
}

internal class DayRunner : IDayRunner, IScopeRoot
{
    private readonly IReadOnlyList<IDay> _days;
    private readonly IDayPicker _picker;
    private readonly ICorrectnessRegistry _correctnessRegistry;
    private readonly ITimeRegistry _timeRegistry;

    internal DayRunner(
        IReadOnlyList<IDay> days,
        IDayPicker picker,
        ICorrectnessRegistry correctnessRegistry,
        ITimeRegistry timeRegistry)
    {
        _days = days;
        _picker = picker;
        _correctnessRegistry = correctnessRegistry;
        _timeRegistry = timeRegistry;
    }

    public void Run()
    {
        var start = Stopwatch.GetTimestamp();
        foreach (var day in _picker.PickDay(_days))
        {
            PrintInput(day);
            day.FirstPart();
            day.SecondPart();
        }
        var end = Stopwatch.GetTimestamp();
        var time = Stopwatch.GetElapsedTime(start, end);
        Console.Write("Whole run took ");
        ConsoleHelper.PrintWholeRunTime(time);
        Console.WriteLine(".");
        Console.WriteLine();
        
        Console.WriteLine("Results Summary:");
        foreach (var (correctnessStatus, tasks) in _correctnessRegistry.Results.OrderBy(x => x.Key))
        {
            ConsoleHelper.WriteColored(correctnessStatus.ToChar().ToString(), correctnessStatus.ToConsoleColor());
            Console.Write(" => ");
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
        
        var maybeMaxTime = _timeRegistry.Results.Count > 0 
            ? _timeRegistry.Results.MaxBy(x => x.Duration).Duration
            : (TimeSpan?)null;
        if (maybeMaxTime is { } maxTime)
        {
            var timeUnit = maxTime.TotalMilliseconds / 125.0;
            var greenUnitCount = (int) (1000.0 / timeUnit);
            var yellowUnitCount = (int) (60000.0 / timeUnit) - greenUnitCount;
            var maxLabelLength = _timeRegistry.Results.Select(t => t.Day.GetShortDayLabelText(false)).Max(x => x.Length);
            Console.WriteLine("Times Summary:");
            var sortedTimeRecords = _timeRegistry.Results.OrderBy(x => x.Duration);
            foreach (var (day, isFirstPart, duration) in sortedTimeRecords)
            {
                var dayLabelLength = day.GetShortDayLabelText(isFirstPart).Length;
                var timeUnitCount = (int) (duration.TotalMilliseconds / timeUnit);
                var greens = Math.Max(Math.Min(timeUnitCount, greenUnitCount), 1);
                timeUnitCount -= greenUnitCount;
                var yellows = timeUnitCount > 0 ? Math.Min(timeUnitCount, yellowUnitCount) : 0;
                timeUnitCount -= yellowUnitCount;
                var reds = Math.Max(timeUnitCount, 0);
                var remainingSpaces = 125 - greens - yellows - reds;
                day.PrintShortDayLabel(isFirstPart);
                Console.Write($"{"".PadRight(maxLabelLength - dayLabelLength, ' ')} ");
                ConsoleHelper.WriteColored("".PadRight(greens, '|'), ConsoleColor.DarkGreen);
                ConsoleHelper.WriteColored("".PadRight(yellows, '|'), ConsoleColor.DarkYellow);
                ConsoleHelper.WriteColored("".PadRight(reds, '|'), ConsoleColor.DarkRed);
                Console.Write("".PadRight(remainingSpaces, ' '));
                Console.Write(" ");
                ConsoleHelper.PrintTaskTime(duration);
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        return;
        
        void PrintInput(IDay day)
        {
            day.PrintLongDayLabel();
            Console.WriteLine(" Input:");
            var shortenedInput = day.Input.Select((c, i) => (c, i)).Where(t => t.c is '\n').Take(5).Skip(4).FirstOrDefault() 
                is (_, var index and > 0)
                ? (Shortened: true, Input: day.Input[..index])
                : (Shortened: false, Input: day.Input);
            foreach (var line in shortenedInput.Input.Split(Environment.NewLine))
            {
                if (line.Length > 150)
                {
                    ConsoleHelper.WriteColored(line[..150], ConsoleColor.DarkGray);
                    ConsoleHelper.WriteLineColored("...", ConsoleColor.DarkYellow);
                }
                else
                {
                    ConsoleHelper.WriteLineColored(line, ConsoleColor.DarkGray);
                }
            }

            if (shortenedInput.Shortened)
            {
                ConsoleHelper.WriteLineColored("...", ConsoleColor.DarkYellow);
            }

            Console.WriteLine();
        }
    }
}