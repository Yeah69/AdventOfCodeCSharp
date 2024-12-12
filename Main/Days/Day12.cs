namespace AdventOfCode.Days;

internal class Day12 : DayBase<Day12, IReadOnlyList<((int X, int Y), char C)>>
{
    public override int Number => 12;

    protected override IReadOnlyList<((int X, int Y), char C)> ParseInput()
    {
        var list = new List<((int X, int Y), char C)>();
        var inputSpan = Input.AsSpan();
        var lineRanges = inputSpan.Split(Environment.NewLine);
        var y = 0;
        foreach (var lineRange in lineRanges)
        {
            var lineSpan = inputSpan[lineRange];
            var x = 0;
            foreach (var c in lineSpan)
            {
                list.Add(((x++, y), c));
            }
            y++;
        }
        return list;
    }
    
    public override string FirstPart()
    {
        var price = 0L;
        var plots = ParsedInput.Value;
        
        var groupedByPlant = plots.GroupBy(p => p.C, p => p.Item1);
        
        foreach (var group in groupedByPlant)
        {
            var plants = group.ToHashSet();
            var toBeAssigned = plants.ToHashSet();

            while (toBeAssigned.Count > 0)
            {
                var region = new List<(int X, int Y)>();
                var queue = new Queue<(int X, int Y)>();
                queue.Enqueue(toBeAssigned.First());
                toBeAssigned.Remove(queue.Peek());
                while (queue.Count > 0)
                {
                    var currentPosition = queue.Dequeue();
                    region.Add(currentPosition);
                    var neighbors = FourDirectionsUtils.GetNeighbors(currentPosition)
                        .Where(neighbor => toBeAssigned.Contains(neighbor));
                    foreach (var neighbor in neighbors)
                    {
                        queue.Enqueue(neighbor);
                        toBeAssigned.Remove(neighbor);
                    }
                }
                
                var area = region.Count;
                var perimeter = region.Sum(r => FourDirectionsUtils.GetNeighbors(r).Count(n => !plants.Contains(n)));
                price += area * perimeter;
            }
        }
        
        return price.ToString();
    }

    public override string SecondPart()
    {
        var price = 0L;
        var plots = ParsedInput.Value;
        
        var groupedByPlant = plots.GroupBy(p => p.C, p => p.Item1);
        
        foreach (var group in groupedByPlant)
        {
            var plants = group.ToHashSet();
            var toBeAssigned = plants.ToHashSet();

            while (toBeAssigned.Count > 0)
            {
                var verticalPerimeters = new List<((int Inward, int Outward), int X)>();
                var horizontalPerimeters = new List<((int Inward, int Outward), int Y)>();
                var region = new List<(int X, int Y)>();
                var queue = new Queue<(int X, int Y)>();
                queue.Enqueue(toBeAssigned.First());
                toBeAssigned.Remove(queue.Peek());
                while (queue.Count > 0)
                {
                    var currentPosition = queue.Dequeue();
                    region.Add(currentPosition);
                    var neighbors = FourDirectionsUtils.GetNeighbors(currentPosition);
                    foreach (var neighbor in neighbors)
                    {
                        if (toBeAssigned.Contains(neighbor))
                        {
                            queue.Enqueue(neighbor);
                            toBeAssigned.Remove(neighbor);
                        }
                        else if (!plants.Contains(neighbor))
                        {
                            if (currentPosition.X == neighbor.X)
                            {
                                horizontalPerimeters.Add(((currentPosition.Y, neighbor.Y), currentPosition.X));
                            }
                            else if (currentPosition.Y == neighbor.Y)
                            {
                                verticalPerimeters.Add(((currentPosition.X, neighbor.X), currentPosition.Y));
                            }
                        }
                    }
                }
                
                var area = region.Count;
                var verticalSides = verticalPerimeters
                    .GroupBy(p => p.Item1, p => p.X)
                    .Sum(g =>
                    {
                        var ordered = g.Order().ToArray();
                        return ordered
                            .Zip(ordered.Skip(1))
                            .Aggregate(1, (acc, pair) => pair.First + 1 != pair.Second ? acc + 1 : acc);
                    });
                var horizontalSides = horizontalPerimeters
                    .GroupBy(p => p.Item1, p => p.Y)
                    .Sum(g =>
                    {
                        var ordered = g.Order().ToArray();
                        return ordered
                            .Zip(ordered.Skip(1))
                            .Aggregate(1, (acc, pair) => pair.First + 1 != pair.Second ? acc + 1 : acc);
                    });
                price += area * (verticalSides + horizontalSides);
            }
        }
        
        return price.ToString();
    }
}