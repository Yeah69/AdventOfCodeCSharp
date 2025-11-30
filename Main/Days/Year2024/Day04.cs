namespace AdventOfCode.Days.Year2024;

internal class Day04 : DayOfYear2024<Day04, string>
{
    public override int Number => 4;
    private readonly EightDirections[] _directions =
    [
        EightDirections.North,
        EightDirections.NorthEast,
        EightDirections.East,
        EightDirections.SouthEast,
        EightDirections.South,
        EightDirections.SouthWest,
        EightDirections.West,
        EightDirections.NorthWest
    ];

    internal enum EightDirections
    {
        North, NorthEast, East, SouthEast, South, SouthWest, West, NorthWest
    }
            
    private static bool CheckForLetter(string[] lines, int lx, int ly, char letter)
    {
        if (lx < 0 || lx >= lines[0].Length || ly < 0 || ly >= lines.Length) 
            return false;
        return lines[ly][lx] == letter;
    }

    protected override string ParseInput() => Input;

    public override string FirstPart()
    {
        var lines = ParsedInput.Value.Split(Environment.NewLine);
        var xmasCount = 0;

        for (var y = 0; y < lines.Length; y++)
        {
            var line = lines[y];
            for (var x = 0; x < line.Length; x++)
            {
                if (line[x] != 'X')
                    continue;
                xmasCount += _directions.Aggregate(0, (i, direction) => i + (CheckForXmas(x, y, direction) ? 1 : 0));
            }
        }
        
        return xmasCount.ToString();

        bool CheckForXmas(int x, int y, EightDirections direction)
        {
            return Sequence().Aggregate(true, (b, t) => b && CheckForLetter(lines, t.X, t.Y, t.Letter));
            
            IEnumerable<(int X, int Y, char Letter)> Sequence()
            {
                var (currentX, currentY) = (x, y);
                yield return (currentX, currentY, 'X');
                (currentX, currentY) = MakeAStep(currentX, currentY);
                yield return (currentX, currentY, 'M');
                (currentX, currentY) = MakeAStep(currentX, currentY);
                yield return (currentX, currentY, 'A');
                (currentX, currentY) = MakeAStep(currentX, currentY);
                yield return (currentX, currentY, 'S');
                yield break;
                
                (int X, int Y) MakeAStep(int sx, int sy) =>
                    direction switch
                    {
                        EightDirections.North => (sx, sy - 1),
                        EightDirections.NorthEast => (sx + 1, sy - 1),
                        EightDirections.East => (sx + 1, sy),
                        EightDirections.SouthEast => (sx + 1, sy + 1),
                        EightDirections.South => (sx, sy + 1),
                        EightDirections.SouthWest => (sx - 1, sy + 1),
                        EightDirections.West => (sx - 1, sy),
                        EightDirections.NorthWest => (sx - 1, sy - 1),
                        _ => throw new InvalidOperationException()
                    };
            }
        }
    }

    public override string SecondPart()
    {
        var lines = ParsedInput.Value.Split(Environment.NewLine);
        var xmasCount = 0;

        for (var y = 0; y < lines.Length; y++)
        {
            var line = lines[y];
            for (var x = 0; x < line.Length; x++)
            {
                if (line[x] != 'A') 
                    continue;

                if (CheckOneDirection(EightDirections.NorthEast) && CheckOneDirection(EightDirections.SouthEast)
                    || CheckOneDirection(EightDirections.SouthEast) && CheckOneDirection(EightDirections.SouthWest)
                    || CheckOneDirection(EightDirections.SouthWest) && CheckOneDirection(EightDirections.NorthWest)
                    || CheckOneDirection(EightDirections.NorthWest) && CheckOneDirection(EightDirections.NorthEast))
                    xmasCount++;
                continue;

                bool CheckOneDirection(EightDirections direction)
                {
                    var coordinates = GetCoordinates(direction);
                    return CheckForLetter(lines, coordinates.M.X, coordinates.M.Y, 'M')
                           && CheckForLetter(lines, coordinates.S.X, coordinates.S.Y, 'S');
                }
                    
                ((int X, int Y) M, (int X, int Y) S) GetCoordinates(EightDirections direction)
                {
                    return direction switch
                    {
                        EightDirections.North => ((x, y + 1), (x, y - 1)),
                        EightDirections.NorthEast => ((x - 1, y + 1), (x + 1, y - 1)),
                        EightDirections.East => ((x - 1, y), (x + 1, y)),
                        EightDirections.SouthEast => ((x - 1, y - 1), (x + 1, y + 1)),
                        EightDirections.South => ((x, y - 1), (x, y + 1)),
                        EightDirections.SouthWest => ((x + 1, y - 1), (x - 1, y + 1)),
                        EightDirections.West => ((x + 1, y), (x - 1, y)),
                        EightDirections.NorthWest => ((x + 1, y + 1), (x - 1, y - 1)),
                        _ => throw new InvalidOperationException()
                    };
                }
            }
        }
        
        return xmasCount.ToString();
    }
}