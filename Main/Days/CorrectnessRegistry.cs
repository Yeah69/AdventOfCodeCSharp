using MrMeeseeks.DIE.UserUtility;

namespace AdventOfCode.Days;

internal interface ICorrectnessRegistry
{
    IReadOnlyDictionary<CorrectnessStatus, List<(IDay Day, bool IsFirstPart)>> Results { get; }
    void Register(IDay day, bool isFirstPart, CorrectnessStatus status);
}

internal class CorrectnessRegistry : ICorrectnessRegistry, IScopeInstance
{
    private readonly Dictionary<CorrectnessStatus, List<(IDay Day, bool IsFirstPart)>> _results = new()
    {
        [CorrectnessStatus.Incorrect] = [],
        [CorrectnessStatus.Uncertain] = [],
        [CorrectnessStatus.Correct] = []
    };
    public IReadOnlyDictionary<CorrectnessStatus, List<(IDay Day, bool IsFirstPart)>> Results => _results;
    public void Register(IDay day, bool isFirstPart, CorrectnessStatus status) => _results[status].Add((day, isFirstPart));
}