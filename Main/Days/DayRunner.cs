using System.Diagnostics;
using AdventOfCode.Days.Pickers;

namespace AdventOfCode.Days;

internal interface IDayRunner
{
    void Run();
}

internal class DayRunner : IDayRunner
{
    private readonly IReadOnlyList<IDay> _days;
    private readonly IDayPicker _picker;

    internal DayRunner(
        IReadOnlyList<IDay> days,
        IDayPicker picker)
    {
        _days = days;
        _picker = picker;
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
        var time = new TimeSpan(end - start);
        Console.Write("Whole run took ");
        ConsoleHelper.WriteColored(time.ToString(), ConsoleHelper.TimeColor(time));
        Console.WriteLine(".");
        Console.WriteLine();
        return;
        
        void PrintInput(IDay day)
        {
            DayHelper.WriteDayLabel(day, "");
            Console.WriteLine(" Input:");
            var shortenedInput = day.Input.Select((c, i) => (c, i)).Where(t => t.c is '\n').Take(5).Skip(4).FirstOrDefault() 
                is (_, var index and > 0)
                ? (Shortened: true, Input: day.Input[..index])
                : (Shortened: false, Input: day.Input);
            switch (shortenedInput)
            {
                case (true, var shortened):
                    ConsoleHelper.WriteLineColored(shortened, ConsoleColor.DarkGray);
                    ConsoleHelper.WriteLineColored("...", ConsoleColor.DarkYellow);
                    break;
                case (false, var full):
                    ConsoleHelper.WriteLineColored(full, ConsoleColor.DarkGray);
                    break;
            }
            Console.WriteLine();
        }
    }
}