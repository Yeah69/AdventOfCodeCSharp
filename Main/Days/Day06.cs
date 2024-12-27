using System.Collections.Immutable;

namespace AdventOfCode.Days;

internal class Day06 : DayBase<Day06, Day06.Data>
{
    internal record Data(long Width, long Height, (long X, long Y) Start, ImmutableHashSet<(long X, long Y)> Obstacles);
    public override int Number => 6;

    protected override Data ParseInput()
    {
        var inputSpan = Input.AsSpan();
        var width = 0L;
        var height = 0L;
        var start = (0L, 0L);
        var obstacles = new HashSet<(long X, long Y)>();
        var y = 0L;
        var rowRanges = inputSpan.Split(Environment.NewLine);
        foreach (var rowRange in rowRanges)
        {
            var rowSpan = inputSpan[rowRange];
            for (var x = 0L; x < rowSpan.Length; x++)
            {
                switch (rowSpan[(int)x])
                {
                    case '#':
                        obstacles.Add((x, y));
                        break;
                    case '^':
                        start = (x, y);
                        break;
                }
            }
            width = rowSpan.Length;
            height++;
            y++;
        }
        return new(width, height, start, obstacles.ToImmutableHashSet());
    }

    private static FourDirections SwitchDirection(FourDirections currentDirection) =>
        currentDirection switch
        {
            FourDirections.North => FourDirections.East,
            FourDirections.East => FourDirections.South,
            FourDirections.South => FourDirections.West,
            FourDirections.West => FourDirections.North,
            _ => throw new ArgumentOutOfRangeException(nameof(currentDirection), currentDirection, null)
        };
    
    
    public override string FirstPart()
    {
        var data = ParsedInput.Value;
        var currentPosition = data.Start;
        var currentDirection = FourDirections.North;
        var reachedPositions = new HashSet<(long X, long Y)> { currentPosition };
        while (true)
        {
            var maybeStep = FourDirectionsUtils.MakeAStep(currentPosition, currentDirection);
            while (data.Obstacles.Contains(maybeStep))
            {
                currentDirection = SwitchDirection(currentDirection);
                maybeStep = FourDirectionsUtils.MakeAStep(currentPosition, currentDirection);
            }
            currentPosition = maybeStep;
            if (currentPosition.X < 0
                || currentPosition.X >= data.Width
                || currentPosition.Y < 0
                || currentPosition.Y >= data.Height)
                break;
            reachedPositions.Add(currentPosition);
        }
        return reachedPositions.Count.ToString();
    }

    public override string SecondPart()
    {
        var result = 0L;
        var data = ParsedInput.Value;
        for (var y = 0L; y < data.Height; y++)
        {
            for (var x = 0L; x < data.Width; x++)
            {
                if (!data.Obstacles.Contains((x, y)) && Check(data.Obstacles.Add((x, y))))
                    result++;
            }
        }
        
        return result.ToString();

        bool Check(ImmutableHashSet<(long X, long Y)> obstacles)
        {
            var currentPosition = data.Start;
            var currentDirection = FourDirections.North;
            var reachedPositions = new HashSet<((long X, long Y) Position, FourDirections Direction)>
            {
                (currentPosition, currentDirection)
            };
            while (true)
            {
                var maybeStep = FourDirectionsUtils.MakeAStep(currentPosition, currentDirection);
                while (obstacles.Contains(maybeStep))
                {
                    currentDirection = SwitchDirection(currentDirection);
                    maybeStep = FourDirectionsUtils.MakeAStep(currentPosition, currentDirection);
                }
                currentPosition = maybeStep;
                if (reachedPositions.Contains((currentPosition, currentDirection)))
                    return true;
                if (currentPosition.X < 0
                    || currentPosition.X >= data.Width
                    || currentPosition.Y < 0
                    || currentPosition.Y >= data.Height)
                    return false;
                reachedPositions.Add((currentPosition, currentDirection));
            }
        }
    }
}