using System.Collections.Frozen;
using System.Collections.Immutable;

namespace AdventOfCode.Days;

internal class Day20 : DayBase<Day20, Day20.Data>
{
    internal record Data(ImmutableArray<(int X, int Y)> Path, FrozenDictionary<(int X, int Y), int> BestTimes);
    public override int Number => 20;

    protected override Data ParseInput()
    {
        var inputSpan = Input.AsSpan();
        var lineRanges = inputSpan.Split(Environment.NewLine);
        var pathPositions = new HashSet<(int X, int Y)>();
        var start = (1, 1);
        var y = 0;
        foreach (var lineRange in lineRanges)
        {
            var lineSpan = inputSpan[lineRange];
            for(var x = 0; x < lineSpan.Length; ++x)
            {
                switch (lineSpan[x])
                {
                    case '.':
                    case 'E':
                        pathPositions.Add((x, y));
                        break;
                    case 'S':
                        start = (x, y);
                        pathPositions.Add((x, y));
                        break;
                }
            }
            ++y;
        }

        var path = new List<(int X, int Y)>();
        var current = ((int X, int Y)?) start;
        var processed = new HashSet<(int X, int Y)>();
        while (current is not null)
        {
            path.Add(current.Value);
            processed.Add(current.Value);
            current = FourDirectionsUtils.GetNeighbors(current.Value)
                .OfType<(int X, int Y)?>()
                .SingleOrDefault(n => n is not null && pathPositions.Contains(n.Value) && !processed.Contains(n.Value));
        }
        
        var bestTimes = new Dictionary<(int X, int Y), int>();
        var distance = path.Count - 1;
        foreach (var position in path)
            bestTimes[position] = distance--;

        return new Data([..path], bestTimes.ToFrozenDictionary());
    }

    private string Solve(int maxCheatingSteps)
    {
        var areaRange = maxCheatingSteps * 2 + 1;
        var data = ParsedInput.Value;
        
        var vanillaPathTime = data.BestTimes[data.Path.First()];
        var stepsTaken = 0;

        return GetSavedTimes().Count(t => t >= 100).ToString();

        IEnumerable<int> GetSavedTimes()
        {
            foreach (var step in data.Path.SkipLast(1))
            {
                var possibleCheatingStarts = Enumerable.Range(step.X - maxCheatingSteps, areaRange)
                    .SelectMany(x => Enumerable.Range(step.Y - maxCheatingSteps, areaRange).Select(y => (X: x, Y: y)));

                foreach (var cheatingStart in possibleCheatingStarts)
                {
                    var distance = Math.Abs(cheatingStart.X - step.X) + Math.Abs(cheatingStart.Y - step.Y);
                    if (distance > maxCheatingSteps)
                        continue;
                    
                    if (!data.BestTimes.TryGetValue(cheatingStart, out var time))
                        continue;
                    
                    var timeWithCheat = stepsTaken + distance + time;
                    if (timeWithCheat < vanillaPathTime)
                        yield return vanillaPathTime - timeWithCheat;
                }
                
                ++stepsTaken;
            }
        }
    }

    public override string FirstPart() => Solve(2);

    public override string SecondPart() => Solve(20);
}