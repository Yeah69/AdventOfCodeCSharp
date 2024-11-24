using System.Diagnostics;
using MrMeeseeks.DIE.UserUtility;

namespace AdventOfCode.Days;

internal class DayDecoratorTrackTime : IDay, IDecorator<IDay>
{
    private readonly IDay _inner;
    private readonly ITimeRegistry _timeRegistry;

    internal DayDecoratorTrackTime(IDay inner, ITimeRegistry timeRegistry)
    {
        _inner = inner;
        _timeRegistry = timeRegistry;
    }
    
    public int Number => _inner.Number;
    public int? SampleNumber => _inner.SampleNumber;
    public string Input => _inner.Input;
    public string FirstPart()
    {
        var start = Stopwatch.GetTimestamp();
        var result = _inner.FirstPart();
        var end = Stopwatch.GetTimestamp();
        PrintTracking(true, Stopwatch.GetElapsedTime(start, end));
        return result;
    }

    public string SecondPart()
    {
        var start = Stopwatch.GetTimestamp();
        var result = _inner.SecondPart();
        var end = Stopwatch.GetTimestamp();
        PrintTracking(false, Stopwatch.GetElapsedTime(start, end));
        return result;
    }

    public IEnumerable<IDay> Samples() => _inner.Samples();
    public void PrintLongDayLabel(bool? isFirstPart = null) => _inner.PrintLongDayLabel(isFirstPart);
    public void PrintShortDayLabel(bool isFirstPart) => _inner.PrintShortDayLabel(isFirstPart);
    public string GetShortDayLabelText(bool isFirstPart) => _inner.GetShortDayLabelText(isFirstPart);

    private void PrintTracking(bool isFirstPart, TimeSpan time)
    {
        _inner.PrintLongDayLabel();
        Console.Write(" Solution took ");
        ConsoleHelper.PrintTaskTime(time);
        Console.WriteLine(".");
        Console.WriteLine();

        _timeRegistry.Register(this, isFirstPart, time);
    }
}