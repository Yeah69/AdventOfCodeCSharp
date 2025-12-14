using System.Collections.Frozen;
using System.Collections.Immutable;

namespace AdventOfCode.Days.Year2025;

internal class Day12 : DayOfYear2025<Day12, (ImmutableArray<Day12.Present> Presents, ImmutableArray<Day12.Region> Regions)>
{
    internal record Region(long Width, long Length, ImmutableArray<long> Quantities);
    internal class Present
    {
        internal Present(HashSet<(long X, long Y)> original)
        {
            var list = new List<FrozenSet<(long X, long Y)>>();
            var current = original.ToFrozenSet();
            for (var i = 0L; i < 4L; ++i)
            {
                if (list.All(s => !s.SetEquals(current)))
                    list.Add(current);
                
                var maxX = current.Max(x => x.X);
                var verticallyMirrored = current.Select(c => c with { X = maxX - c.X }).ToFrozenSet();
                if (list.All(s => !s.SetEquals(verticallyMirrored)))
                    list.Add(verticallyMirrored);
                
                var maxY = current.Max(x => x.Y);
                var horizontallyMirrored = current.Select(c => c with { Y = maxY - c.Y }).ToFrozenSet();
                if (list.All(s => !s.SetEquals(horizontallyMirrored)))
                    list.Add(horizontallyMirrored);

                current = horizontallyMirrored.Select(c => (X: c.Y, Y: c.X)).ToFrozenSet();
            }

            Configurations = [..list];
        }
        
        internal ImmutableArray<FrozenSet<(long X, long Y)>> Configurations { get; }
    }
    
    public override int Number => 12;

    protected override (ImmutableArray<Present> Presents, ImmutableArray<Region> Regions) ParseInput()
    {
        var span = Input.AsSpan();
        var ranges = span.Split($"{Environment.NewLine}{Environment.NewLine}");
        ranges.MoveNext();
        var zero = ParsePresent(span[ranges.Current]);
        ranges.MoveNext();
        var one = ParsePresent(span[ranges.Current]);
        ranges.MoveNext();
        var two = ParsePresent(span[ranges.Current]);
        ranges.MoveNext();
        var three = ParsePresent(span[ranges.Current]);
        ranges.MoveNext();
        var four = ParsePresent(span[ranges.Current]);
        ranges.MoveNext();
        var five = ParsePresent(span[ranges.Current]);
        ranges.MoveNext();
        
        var regionsSpan = span[ranges.Current];
        
        var regionLineRanges = regionsSpan.Split(Environment.NewLine);
        var regions = new List<Region>();

        foreach (var regionLine in regionLineRanges)
        {
            var regionSpan = regionsSpan[regionLine];
            var regionPartsRanges = regionSpan.Split(": ");
            
            regionPartsRanges.MoveNext();
            
            var specSpan = regionSpan[regionPartsRanges.Current];
            var specRanges = specSpan.Split('x');
            specRanges.MoveNext();
            var width = long.Parse(specSpan[specRanges.Current]);
            specRanges.MoveNext();
            var length = long.Parse(specSpan[specRanges.Current]);
            
            regionPartsRanges.MoveNext();
            
            var quantitiesSpan = regionSpan[regionPartsRanges.Current];
            var quantitiesRanges = quantitiesSpan.Split(' ');
            var quantities = new List<long>();
            foreach (var quantityRange in quantitiesRanges)
                quantities.Add(long.Parse(quantitiesSpan[quantityRange]));
            regions.Add(new(width, length, [..quantities]));
        }

        return ([zero, one, two, three, four, five], [..regions]);

        Present ParsePresent(ReadOnlySpan<char> presentInput)
        {
            var presentRanges = presentInput.Split(Environment.NewLine);
            presentRanges.MoveNext();
            var y = 0;
            var set = new HashSet<(long X, long Y)>();
            while (presentRanges.MoveNext())
            {
                var lineSpan = presentInput[presentRanges.Current];
                for (var x = 0; x < lineSpan.Length; ++x)
                {
                    if (lineSpan[x] == '#')
                        set.Add((x, y));
                }
                ++y;
            }
            return new Present(set);
        }
    }

    public override string FirstPart()
    {
        var (presents, regions) = ParsedInput.Value;

        var result = regions.Count(IsValid);
        
        return result.ToString();

        bool IsValid(Region region)
        {
            var toFitPoints = region.Quantities.Select((q, i) => q * presents[i].Configurations.First().Count).Sum();
            var availablePoints = region.Width * region.Length;

            if (toFitPoints > availablePoints)
                return false;
            
            var (isValid, map) = Inner([], region.Quantities.Select((q, i) => (q, i)).Where(t => t.q > 0).ToImmutableDictionary(t => t.i, t => t.q), 'A');
            
            if (isValid)
                PrintMap();

            return isValid;

            void PrintMap()
            {
                var minX = map.Keys.Min(ap => ap.X);
                var maxX = map.Keys.Max(ap => ap.X);
                var minY = map.Keys.Min(ap => ap.Y);
                var maxY = map.Keys.Max(ap => ap.Y);

                var colors = new Dictionary<char, ConsoleColor> { { '.', ConsoleColor.Gray } };
                foreach (var label in map.Values)
                {
                    colors[label] = (ConsoleColor)((label - 'A') % 6 + 1);
                }

                for (var y = minY; y <= maxY; ++y)
                {
                    for (var x = minX; x <= maxX; ++x)
                    {
                        var character = map.GetValueOrDefault((x, y), '.');
                        var temp = Console.ForegroundColor;
                        Console.ForegroundColor = colors[character];
                        Console.Write(character);
                        Console.ForegroundColor = temp;
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
                Console.WriteLine();
            }

            (bool IsValid, ImmutableDictionary<(long X, long Y), char>) Inner(ImmutableDictionary<(long X, long Y), char> assignedPoints, ImmutableDictionary<int, long> quantities, char label)
            {
                if (assignedPoints.Count is 0)
                {
                    foreach (var (id, quantity) in quantities)
                    {
                        var newQuantities = quantities.Remove(id);
                        if (quantity > 1) newQuantities = newQuantities.Add(id, quantity - 1);
                        var newAssignedPoints = assignedPoints
                            .AddRange(presents[id].Configurations
                                .First()
                                .Select(c => new KeyValuePair<(long X, long Y), char>(c, label)));
                        var innerResult = Inner(newAssignedPoints, newQuantities, (char) ((byte) label + 1));
                        if (innerResult.IsValid)
                            return innerResult;
                    }
                }

                if (quantities.Count is 0)
                    return (true, assignedPoints);

                var minX = assignedPoints.Keys.Min(ap => ap.X);
                var minY = assignedPoints.Keys.Min(ap => ap.Y);
                var maxX = assignedPoints.Keys.Max(ap => ap.X);
                var maxY = assignedPoints.Keys.Max(ap => ap.Y);

                var priorityQueue = new PriorityQueue<(int Key, ImmutableHashSet<(long X, long Y)> Positions), long>();

                for (var x = minX - 3L; x <= maxX + 1; ++x)
                {
                    for (var y = minY - 3L; y < maxY + 1; ++y)
                    {
                        var newMinX = Math.Min(minX, x);
                        var newMinY = Math.Min(minY, y);
                        var newMaxX = Math.Max(maxX, x + 2);
                        var newMaxY = Math.Max(maxY, y + 2);
                        
                        var newWidth = newMaxX - newMinX + 1;
                        var newLength = newMaxY - newMinY + 1;

                        if ((newWidth > region.Width || newLength > region.Length) &&
                            (newLength > region.Width || newWidth > region.Length))
                            continue;

                        var priority = newWidth * newLength;
                        
                        foreach (var key in quantities.Keys)
                        {
                            foreach (var configuration in presents[key].Configurations)
                            {
                                var positions = configuration.Select(c => (X: x + c.X, Y: y + c.Y)).ToImmutableHashSet();
                                if (positions.Any(assignedPoints.Keys.Contains))
                                    continue;
                                
                                priorityQueue.Enqueue((key, positions), priority);
                            }
                        }
                    }
                }

                while (priorityQueue.TryDequeue(out var tuple, out _))
                {
                    var (key, positions) = tuple;
                    var quantity = quantities[key];
                    var newQuantities = quantities.Remove(key);
                    if (quantity > 1) newQuantities = newQuantities.Add(key, quantity - 1);
                    var newAssignedPoints = assignedPoints
                        .AddRange(positions
                            .Select(c => new KeyValuePair<(long X, long Y), char>(c, label)));
                    var innerResult = Inner(newAssignedPoints, newQuantities, label is 'Z' ? 'A' : (char) ((byte) label + 1));
                    if (innerResult.IsValid)
                        return innerResult;
                }

                return (false, []);
            }
        }
    }

    public override string SecondPart() => 
        Consts.NothingToDoHere;
}