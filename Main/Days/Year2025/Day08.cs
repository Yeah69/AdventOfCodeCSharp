using System.Collections.Immutable;

namespace AdventOfCode.Days.Year2025;

internal class Day08 : DayOfYear2025<Day08, List<Day08.Coordinates>>
{
    internal record struct Coordinates(long X, long Y, long Z);
    public override int Number => 8;

    protected override List<Coordinates> ParseInput()
    {
        var span = Input.AsSpan();
        var lineRanges = span.Split(Environment.NewLine);
        var coordinates = new List<Coordinates>();

        foreach (var lineRange in lineRanges)
        {
            var lineSpan = span[lineRange];
            var partRanges = lineSpan.Split(',');
            partRanges.MoveNext();
            var x = long.Parse(lineSpan[partRanges.Current]);
            partRanges.MoveNext();
            var y = long.Parse(lineSpan[partRanges.Current]);
            partRanges.MoveNext();
            var z = long.Parse(lineSpan[partRanges.Current]);
            coordinates.Add(new (x, y, z));
        }

        return coordinates;
    }

    public override string FirstPart()
    {
        var coordinates = ParsedInput.Value;
        
        var priorityQueue = BuildPriorityQueue(coordinates);
        
        var circuits = new List<HashSet<Coordinates>>();
        var max = SampleNumber > 0 ? 10 : 1000;

        for (var i = 0; i < max; ++i)
        {
            var (one, two) = priorityQueue.Dequeue();
            
            var setOne = circuits.FirstOrDefault(x => x.Contains(one));
            var setTwo = circuits.FirstOrDefault(x => x.Contains(two));
            if (setOne is null && setTwo is null)
                circuits.Add([one, two]);
            else if (setOne is not null && setTwo is null)
                setOne.Add(two);
            else if (setOne is null && setTwo is not null)
                setTwo.Add(one);
            else if (setOne is not null && setTwo is not null && setOne != setTwo)
            {
                setOne.UnionWith(setTwo);
                circuits.Remove(setTwo);
            }
        }

        return circuits
            .Select(c => c.Count)
            .OrderDescending()
            .Take(3)
            .Aggregate(1, (current, i) => current * i)
            .ToString();
    }

    public override string SecondPart()
    {
        var coordinates = ParsedInput.Value;
        
        var priorityQueue = BuildPriorityQueue(coordinates);

        var notDoneYetSet = coordinates.ToHashSet();
        var circuits = new List<HashSet<Coordinates>>();
        var lastConnection = (coordinates[0],  coordinates[1]);

        while (notDoneYetSet.Count > 0)
        {
            var (one, two) = priorityQueue.Dequeue();
            
            var setOne = circuits.FirstOrDefault(x => x.Contains(one));
            var setTwo = circuits.FirstOrDefault(x => x.Contains(two));
            if (setOne is null && setTwo is null)
            {
                circuits.Add([one, two]);
                notDoneYetSet.Remove(one);
                notDoneYetSet.Remove(two);
                lastConnection = (one, two);
            }
            else if (setOne is not null && setTwo is null)
            {
                setOne.Add(two);
                notDoneYetSet.Remove(two);
                lastConnection = (one, two);
            }
            else if (setOne is null && setTwo is not null)
            {
                setTwo.Add(one);
                notDoneYetSet.Remove(one);
                lastConnection = (one, two);
            }
            else if (setOne is not null && setTwo is not null && setOne != setTwo)
            {
                setOne.UnionWith(setTwo);
                circuits.Remove(setTwo);
                lastConnection = (one, two);
            }
        }

        return (lastConnection.Item1.X * lastConnection.Item2.X).ToString();
    }

    private static PriorityQueue<(Coordinates, Coordinates), double> BuildPriorityQueue(List<Coordinates> coordinates)
    {
        var priorityQueue = new PriorityQueue<(Coordinates, Coordinates), double>();

        for (var i = 0; i < coordinates.Count; ++i)
        {
            var coordinateOne  = coordinates[i];
            for (var j = i + 1; j < coordinates.Count; ++j)
            {
                var coordinateTwo  = coordinates[j];
                
                var distance = Math.Sqrt(
                    Math.Pow(coordinateOne.X - coordinateTwo.X, 2) 
                    + Math.Pow(coordinateOne.Y - coordinateTwo.Y, 2)
                    + Math.Pow(coordinateOne.Z - coordinateTwo.Z, 2));
                
                priorityQueue.Enqueue((coordinateOne, coordinateTwo), distance);
            }
        }

        return priorityQueue;
    }
}