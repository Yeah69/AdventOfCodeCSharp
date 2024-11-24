using MrMeeseeks.DIE.UserUtility;

namespace AdventOfCode.Days;

internal interface ITimeRegistry
{
    IReadOnlyList<(IDay Day, bool IsFirstPart, TimeSpan Duration)> Results { get; }
    void Register(IDay day, bool isFirstPart, TimeSpan duration);
}

internal class TimeRegistry : ITimeRegistry, IScopeInstance
{
    private readonly List<(IDay Day, bool IsFirstPart, TimeSpan Duration)> _results = [];
    public IReadOnlyList<(IDay Day, bool IsFirstPart, TimeSpan Duration)> Results => _results;
    public void Register(IDay day, bool isFirstPart, TimeSpan duration) => _results.Add((day, isFirstPart, duration));
}