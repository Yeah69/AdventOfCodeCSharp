using System.Collections.Immutable;

namespace AdventOfCode.Days.Year2024;

internal class Day15 : DayOfYear2024<Day15, Day15.Data>
{
    internal record Data(
        IReadOnlySet<(int X, int Y)> Walls, 
        ImmutableHashSet<(int X, int Y)> Boxes, 
        (int X, int Y) Start,
        IReadOnlyList<FourDirections> Instructions);
    public override int Number => 15;

    protected override Data ParseInput()
    {
        var inputSpan = Input.AsSpan();
        var inputPartRanges = inputSpan.Split(Environment.NewLine + Environment.NewLine);
        
        if (!inputPartRanges.MoveNext())
            throw new InvalidOperationException("No parts found.");
        var mapSpan = inputSpan[inputPartRanges.Current];
        var mapLineRanges = mapSpan.Split(Environment.NewLine);
        var walls = new HashSet<(int X, int Y)>();
        var boxes = ImmutableHashSet<(int X, int Y)>.Empty;
        var start = (1, 1);
        var y = 0;
        foreach (var mapLineRange in mapLineRanges)
        {
            var mapLineSpan = mapSpan[mapLineRange];
            for(var x = 0; x < mapLineSpan.Length; ++x)
            {
                switch (mapLineSpan[x])
                {
                    case '#':
                        walls.Add((x, y));
                        break;
                    case 'O':
                        boxes = boxes.Add((x, y));
                        break;
                    case '@':
                        start = (x, y);
                        break;
                }
            }
            ++y;
        }
        
        if (!inputPartRanges.MoveNext())
            throw new InvalidOperationException("No parts found.");
        var instructionsSpan = inputSpan[inputPartRanges.Current];
        var instructions = new List<FourDirections>();
        foreach (var c in instructionsSpan)
        {
            if (c is '\r' or '\n')
                continue;
            instructions.Add(c switch
            {
                '^' => FourDirections.North,
                'v' => FourDirections.South,
                '<' => FourDirections.West,
                '>' => FourDirections.East,
                _ => throw new InvalidOperationException("Invalid instruction.")
            });
        }

        return new Data(walls, boxes, start, instructions);
    }

    public override string FirstPart()
    {
        var data = ParsedInput;

        var boxes = data.Value.Boxes;
        var currentPosition = data.Value.Start;
        var walls = data.Value.Walls;
        var instructions = new Queue<FourDirections>(data.Value.Instructions);
        
        while (instructions.Count > 0)
        {
            var instruction = instructions.Dequeue();
            var nextPosition = FourDirectionsUtils.MakeAStep(currentPosition, instruction);
            var boxesInFront = new Stack<(int X, int Y)>();
            while (boxes.Contains(nextPosition))
            {
                boxesInFront.Push(nextPosition);
                nextPosition = FourDirectionsUtils.MakeAStep(nextPosition, instruction);
            }
            if (walls.Contains(nextPosition))
                continue;
            foreach (var boxPosition in boxesInFront)
            {
                var newBoxPosition = FourDirectionsUtils.MakeAStep(boxPosition, instruction);
                boxes = boxes.Remove(boxPosition).Add(newBoxPosition);
            }
            currentPosition = FourDirectionsUtils.MakeAStep(currentPosition, instruction);
        }
        
        return boxes.Select(p => p.Y * 100 + p.X).Sum().ToString();
    }

    public override string SecondPart()
    {
        var data = ParsedInput;

        var boxes = data.Value.Boxes.Select(p => p with { X = p.X * 2 }).ToImmutableHashSet();
        var currentPosition = data.Value.Start;
        currentPosition = (currentPosition.X * 2, currentPosition.Y);
        var walls = data.Value.Walls.SelectMany(p => new (int X, int Y)[] { (p.X * 2, p.Y), (p.X * 2 + 1, p.Y) }).ToHashSet();
        var instructions = new Queue<FourDirections>(data.Value.Instructions);
        
        while (instructions.Count > 0)
        {
            var instruction = instructions.Dequeue();
            switch (instruction)
            {
                case FourDirections.North:
                case FourDirections.South:
                {
                    var nextBoxPositionRight = FourDirectionsUtils.MakeSteps(currentPosition, instruction, 1);
                    var nextBoxPositionLeft = nextBoxPositionRight with { X = nextBoxPositionRight.X - 1 };
                    var nextPosition = FourDirectionsUtils.MakeAStep(currentPosition, instruction);
                    var result = boxes.Contains(nextBoxPositionRight) && GetMoveAbilityAndBoxes([nextBoxPositionRight]) is var rightResult
                        ? rightResult
                        : boxes.Contains(nextBoxPositionLeft) && GetMoveAbilityAndBoxes([nextBoxPositionLeft]) is var leftResult
                            ? leftResult
                            : (!walls.Contains(nextPosition), Array.Empty<(int X, int Y)>());
                    if (!result.Item1)
                        continue;
                    foreach (var boxPosition in result.Item2)
                    {
                        var newBoxPosition = FourDirectionsUtils.MakeAStep(boxPosition, instruction);
                        boxes = boxes.Remove(boxPosition).Add(newBoxPosition);
                    }
                    currentPosition = FourDirectionsUtils.MakeAStep(currentPosition, instruction);
                    
                    (bool, IEnumerable<(int X, int Y)>) GetMoveAbilityAndBoxes((int X, int Y)[] currentBoxLine)
                    {
                        if (currentBoxLine
                            .SelectMany(p => new (int X, int Y)[] { (p.X, p.Y), (p.X + 1, p.Y) })
                            .Select(p => FourDirectionsUtils.MakeAStep(p, instruction))
                            .Any(p => walls.Contains(p)))
                            return (false, []);

                        var nextBoxLine = currentBoxLine
                            .SelectMany(p => new (int X, int Y)[] { (p.X - 1, p.Y), (p.X, p.Y), (p.X + 1, p.Y) })
                            .Distinct()
                            .Select(p => FourDirectionsUtils.MakeAStep(p, instruction))
                            .Where(p => boxes.Contains(p))
                            .ToArray();
                        if (nextBoxLine.Length == 0)
                            return (true, currentBoxLine);

                        var nextLineResult = GetMoveAbilityAndBoxes(nextBoxLine);
                        
                        return (nextLineResult.Item1, nextLineResult.Item2.Concat(currentBoxLine));
                    }
                }
                    break;
                case FourDirections.West: 
                {
                    var nextPosition = FourDirectionsUtils.MakeAStep(currentPosition, instruction);
                    var nextBoxPosition = FourDirectionsUtils.MakeSteps(currentPosition, instruction, 2);
                    var boxesInFront = new Stack<(int X, int Y)>();
                    while (boxes.Contains(nextBoxPosition))
                    {
                        boxesInFront.Push(nextBoxPosition);
                        nextPosition = FourDirectionsUtils.MakeAStep(nextBoxPosition, instruction);
                        nextBoxPosition = FourDirectionsUtils.MakeSteps(nextBoxPosition, instruction, 2);
                    }
                    if (walls.Contains(nextPosition))
                        continue;
                    foreach (var boxPosition in boxesInFront)
                    {
                        var newBoxPosition = FourDirectionsUtils.MakeAStep(boxPosition, instruction);
                        boxes = boxes.Remove(boxPosition).Add(newBoxPosition);
                    }
                    currentPosition = FourDirectionsUtils.MakeAStep(currentPosition, instruction);
                }
                    break;
                case FourDirections.East:
                {
                    var nextBoxPosition = FourDirectionsUtils.MakeSteps(currentPosition, instruction, 1);
                    var boxesInFront = new Stack<(int X, int Y)>();
                    while (boxes.Contains(nextBoxPosition))
                    {
                        boxesInFront.Push(nextBoxPosition);
                        nextBoxPosition = FourDirectionsUtils.MakeSteps(nextBoxPosition, instruction, 2);
                    }
                    if (walls.Contains(nextBoxPosition))
                        continue;
                    foreach (var boxPosition in boxesInFront)
                    {
                        var newBoxPosition = FourDirectionsUtils.MakeAStep(boxPosition, instruction);
                        boxes = boxes.Remove(boxPosition).Add(newBoxPosition);
                    }
                    currentPosition = FourDirectionsUtils.MakeAStep(currentPosition, instruction);
                }
                    break;
                default:
                    throw new InvalidOperationException("Invalid instruction.");
            }
        }
        
        return boxes.Select(p => p.Y * 100 + p.X).Sum().ToString();
    }
}