namespace AdventOfCode.Days;

internal class Day14 : DayBase<Day14, Day14.Data>
{
    internal record Robot((int X, int Y) Position, (int X, int Y) Velocity);
    internal record Data(IReadOnlyList<Robot> Robots);
    public override int Number => 14;

    protected override Data ParseInput()
    {
        var robots = new List<Robot>();
        var inputSpan = Input.AsSpan();
        var lineRanges = inputSpan.Split(Environment.NewLine);
        foreach (var lineRange in lineRanges)
        {
            var robotSpan = inputSpan[lineRange];
            var robotPartRanges = robotSpan.Split(' ');
            if (!robotPartRanges.MoveNext())
                throw new InvalidOperationException("Expected robot position");
            var positionSpan = robotSpan[robotPartRanges.Current];
            var positionPartRanges = positionSpan.Split(',');
            if (!positionPartRanges.MoveNext())
                throw new InvalidOperationException("Expected robot X position");
            var positionXSpan = positionSpan[positionPartRanges.Current];
            var positionX = int.Parse(positionXSpan[2..]);
            if (!positionPartRanges.MoveNext())
                throw new InvalidOperationException("Expected robot Y position");
            var positionY = int.Parse(positionSpan[positionPartRanges.Current]);
            if (!robotPartRanges.MoveNext())
                throw new InvalidOperationException("Expected robot velocity");
            var velocitySpan = robotSpan[robotPartRanges.Current];
            var velocityPartRanges = velocitySpan.Split(',');
            if (!velocityPartRanges.MoveNext())
                throw new InvalidOperationException("Expected robot X velocity");
            var velocityXSpan = velocitySpan[velocityPartRanges.Current];
            var velocityX = int.Parse(velocityXSpan[2..]);
            if (!velocityPartRanges.MoveNext())
                throw new InvalidOperationException("Expected robot Y velocity");
            var velocityY = int.Parse(velocitySpan[velocityPartRanges.Current]);
            robots.Add(new Robot((positionX, positionY), (velocityX, velocityY)));
        }
        return new Data(robots);
    }
    
    private Robot MoveRobot(Robot robot, int steps, int width, int height)
    {
        var xPosition = robot.Position.X + robot.Velocity.X * steps;
        var yPosition = robot.Position.Y + robot.Velocity.Y * steps;
        while (xPosition < 0)
            xPosition += width;
        xPosition %= width;
        while (yPosition < 0)
            yPosition += height;
        yPosition %= height;
        return robot with { Position = (X: xPosition, Y: yPosition) };
    }
    
    private IReadOnlyList<Robot> MoveRobots(IReadOnlyList<Robot> robots, int steps, int width, int height) =>
        robots.Select(r => MoveRobot(r, steps, width, height)).ToList();

    private void Print(IReadOnlyList<Robot> robots, int width, int height)
    {
        var robotPositions = robots.Select(r => r.Position).ToHashSet();
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                Console.Write(robotPositions.Contains((x, y)) ? '#' : '.');
            }
            Console.WriteLine();
        }
    }

    public override string FirstPart()
    {
        var data = ParsedInput;
        var width = SampleNumber is null or 0 ? 101 : 11;
        var height = SampleNumber is null or 0 ? 103 : 7;
        var middleWidth = width / 2;
        var middleHeight = height / 2;

        var robots = data.Value.Robots;

        return MoveRobots(robots, 100, width, height)
            .Where(r => r.Position.X != middleWidth && r.Position.Y != middleHeight)
            .CountBy(r => (r.Position.X - middleWidth, r.Position.Y - middleHeight) switch
            {
                { Item1: > 0, Item2: < 0 } => 0,
                { Item1: > 0, Item2: > 0 } => 1,
                { Item1: < 0, Item2: > 0 } => 2,
                { Item1: < 0, Item2: < 0 } => 3,
                _ => throw new InvalidOperationException()
            })
            .Aggregate(1L, (i, g) => i * g.Value)
            .ToString();
    }

    public override string SecondPart()
    {
        var data = ParsedInput;
        var width = SampleNumber is null or 0 ? 101 : 11;
        var height = SampleNumber is null or 0 ? 103 : 7;

        var robots = data.Value.Robots;
        var steps = 0;
        while (!Condition())
        {
            robots = MoveRobots(robots, 1, width, height);
            steps++;
        }
        robots = MoveRobots(robots, 1, width, height);
        steps++;
        while (!Condition())
        {
            robots = MoveRobots(robots, 1, width, height);
            steps++;
        }
        Print(robots, width, height);

        return steps.ToString();
        
        bool Condition() => robots.CountBy(r => r.Position).All(g => g.Value == 1);
    }
}