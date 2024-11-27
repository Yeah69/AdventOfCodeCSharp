using System.Diagnostics;
using MrMeeseeks.DIE.UserUtility;

namespace AdventOfCode.Days;

internal class DayDecoratorTrackTime : IDay, IDecorator<IDay>
{
    private readonly IDay _inner;

    internal DayDecoratorTrackTime(IDay inner)
    {
        _inner = inner;
    }
    
    public int Number => _inner.Number;
    public int? SampleNumber => _inner.SampleNumber;
    public string Input => _inner.Input;
    public string FirstPart()
    {
        var start = Stopwatch.GetTimestamp();
        var result = _inner.FirstPart();
        var end = Stopwatch.GetTimestamp();
        PrintTracking("First", new TimeSpan(end - start));
        return result;
    }

    public string SecondPart()
    {
        var start = Stopwatch.GetTimestamp();
        var result = _inner.SecondPart();
        var end = Stopwatch.GetTimestamp();
        PrintTracking("Second", new TimeSpan(end - start));
        return result;
    }

    public IEnumerable<IDay> Samples() => _inner.Samples();

    private void PrintTracking(string partLabel, TimeSpan time)
    {
        DayHelper.WriteDayLabel(_inner, partLabel);
        Console.Write(" Solution took ");
        ConsoleHelper.WriteColored(time.ToString(), ConsoleHelper.TimeColor(time));
        Console.WriteLine(".");
        Console.WriteLine();
    }
}