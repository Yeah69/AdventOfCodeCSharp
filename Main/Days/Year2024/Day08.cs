using AdventOfCode.Extensions;

namespace AdventOfCode.Days.Year2024;

internal class Day08 : DayOfYear2024<Day08, Day08.Data>
{
    internal record Data(long Height, long Width, IReadOnlyDictionary<char, IReadOnlyList<(long X, long Y)>> AntennaMap);
    
    public override int Number => 8;

    protected override Data ParseInput()
    {
        var inputSpan = Input.AsSpan();
        var lineRanges = inputSpan.Split(Environment.NewLine);
        var antennaLocations = new List<(char Antenna, (long X, long Y) Position)>();
        var y = 0L;
        var x = 0L;
        foreach (var lineRange in lineRanges)
        {
            var lineSpan = inputSpan[lineRange];
            x = 0L;
            foreach (var c in lineSpan)
            {
                if (c != '.')
                    antennaLocations.Add((c, (x, y)));
                ++x;
            }
            ++y;
        }
        var antennaMap = antennaLocations
            .GroupBy(x => x.Antenna, x => x.Position)
            .ToDictionary(g => g.Key, IReadOnlyList<(long X, long Y)> (g) => g.ToList());
        return new(y, x, antennaMap);
    }
    
    public override string FirstPart()
    {
        var data = ParsedInput.Value;
        var height = data.Height;
        var width = data.Width;
        var antennaMap = data.AntennaMap;
        var antinodes = new HashSet<(long X, long Y)>();
        
        foreach (var (_, positions) in antennaMap)
        {
            foreach (var (onePosition, otherPosition) in positions.AllPairs())
            {
                var xDiff = otherPosition.X - onePosition.X;
                var yDiff = otherPosition.Y - onePosition.Y;
                Add((otherPosition.X + xDiff, otherPosition.Y + yDiff));
                Add((onePosition.X - xDiff, onePosition.Y - yDiff));
                continue;

                void Add((long X, long Y) position)
                {
                    if (position.X < 0 || position.X >= width || position.Y < 0 || position.Y >= height) 
                        return;
                    antinodes.Add(position);
                }
            }
        }
        
        return antinodes.Count.ToString();
    }

    public override string SecondPart()
    {
        var data = ParsedInput.Value;
        var height = data.Height;
        var width = data.Width;
        var antennaMap = data.AntennaMap;
        var antinodes = new HashSet<(long X, long Y)>();
        
        foreach (var (_, positions) in antennaMap)
        {
            foreach (var position in positions)
            {
                antinodes.Add(position);
            }

            foreach (var (onePosition, otherPosition) in positions.AllPairs())
            {
                var xDiff = otherPosition.X - onePosition.X;
                var yDiff = otherPosition.Y - onePosition.Y;
                Add((otherPosition.X + xDiff, otherPosition.Y + yDiff), xDiff, yDiff);
                Add((onePosition.X - xDiff, onePosition.Y - yDiff), -xDiff, -yDiff);
                continue;

                void Add((long X, long Y) position, long xD, long yD)
                {
                    while (position.X >= 0 && position.X < width && position.Y >= 0 && position.Y < height)
                    {
                        antinodes.Add(position);
                        position = (position.X + xD, position.Y + yD);
                    }
                }
            }
        }
        
        return antinodes.Count.ToString();
    }
}