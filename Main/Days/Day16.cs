using System.Collections.Frozen;

namespace AdventOfCode.Days;

internal class Day16 : DayBase<Day16, Day16.Data>
{
    internal record Step((int X, int Y) Position, FourDirections Direction, long Score, Step? PreviousStep);
    internal record Data(FrozenSet<(int X, int Y)> Walls, (int X, int Y) Start, (int X, int Y) End);
    public override int Number => 16;

    protected override Data ParseInput()
    {
        var inputSpan = Input.AsSpan();
        var lineRanges = inputSpan.Split(Environment.NewLine);
        var walls = new List<(int X, int Y)>();
        var start = (1, 1);
        var end = (1, 1);
        var y = 0;
        foreach (var lineRange in lineRanges)
        {
            var lineSpan = inputSpan[lineRange];
            for(var x = 0; x < lineSpan.Length; ++x)
            {
                switch (lineSpan[x])
                {
                    case '#':
                        walls.Add((x, y));
                        break;
                    case 'S':
                        start = (x, y);
                        break;
                    case 'E':
                        end = (x, y);
                        break;
                }
            }
            ++y;
        }

        return new Data(walls.ToFrozenSet(), start, end);
    }

    private static IEnumerable<Step> GetFullPath(Step lastStep)
    {
        var currentStep = lastStep;
        while (currentStep is not null)
        {
            yield return currentStep;
            currentStep = currentStep.PreviousStep;
        }
    }

    private IEnumerable<Step> GetBestPaths()
    {
        var end = ParsedInput.Value.End;
        var walls = ParsedInput.Value.Walls;
        
        var alreadyTakenSteps = new Dictionary<((int X, int Y) Position, FourDirections Direction), long>();
        var priorityQueue = new PriorityQueue<Step, long>();
        var startStep = new Step(ParsedInput.Value.Start, FourDirections.East, 0, null); 
        priorityQueue.Enqueue(startStep, startStep.Score);
        
        long? bestScore = null;
        
        while (priorityQueue.Count > 0)
        {
            var currentStep = priorityQueue.Dequeue();
            var (position, direction, score, _) = currentStep;
            if (score > bestScore)
                yield break;
            if (alreadyTakenSteps.TryGetValue((position, direction), out var previousScore) && previousScore < score)
                continue;
            alreadyTakenSteps[(position, direction)] = score;
            if (position == end && bestScore is null)
            {
                bestScore = score;
                yield return currentStep;
                continue;
            }
            if (position == end && score == bestScore)
            {
                yield return currentStep;
                continue;
            }
            
            var moveForward = FourDirectionsUtils.MakeAStep(position, direction);
            if (!walls.Contains(moveForward))
            {
                var forwardStep = new Step(moveForward, direction, score + 1, currentStep);
                priorityQueue.Enqueue(forwardStep, forwardStep.Score);
            }
            
            var counterClockwise = direction.RotateCounterClockwise();
            var clockwise = direction.RotateClockwise();
            var counterClockwiseStep = new Step(position, counterClockwise, score + 1000, currentStep);
            var clockwiseStep = new Step(position, clockwise, score + 1000, currentStep);
            if (!walls.Contains(FourDirectionsUtils.MakeAStep(position, counterClockwise))) 
                priorityQueue.Enqueue(counterClockwiseStep, counterClockwiseStep.Score);
            if (!walls.Contains(FourDirectionsUtils.MakeAStep(position, clockwise)))
                priorityQueue.Enqueue(clockwiseStep, clockwiseStep.Score);
        }
    }

    public override string FirstPart() => GetBestPaths().First().Score.ToString();


    public override string SecondPart() => GetBestPaths().SelectMany(GetFullPath).Select(s => s.Position).Distinct().Count().ToString();
}