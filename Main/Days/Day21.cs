using System.Collections.Immutable;

namespace AdventOfCode.Days;

internal class Day21 : DayBase<Day21, Day21.Data>
{
    internal record Data(ImmutableArray<string> Codes);
    public override int Number => 21;

    protected override Data ParseInput()
    {
        var codes = ImmutableArray<string>.Empty;
        var inputSpan = Input.AsSpan();
        var lineRanges = inputSpan.Split(Environment.NewLine);
        foreach (var lineRange in lineRanges)
        {
            codes = codes.Add(inputSpan[lineRange].ToString());
        }
        
        return new(codes);
    }

    public override string FirstPart()
    {
        var data = ParsedInput.Value;

        var sum = 0L;
        
        foreach (var code in data.Codes)
        {
            Console.WriteLine(code);

            var list = NumPadPossibilities(code, 'A')
                /*.Select(s =>
                {
                    return s.Split('A').SkipLast(1).Select(p => p + 'A')
                        .Sum(p =>
                        {
                            var currentPart = p;
                            for (var i = 0; i < 2; ++i)
                            {
                                currentPart = DirPadPossibilities(currentPart, 'A').First();
                            }
                            return (long) currentPart.Length;
                        });
                })
                .First();*/
                .SelectMany(s => DirPadPossibilities(s, 'A'))
                .SelectMany(s => DirPadPossibilities(s, 'A'))
                .GroupBy(s => s.Length)
                .ToList();
            
            var allString = string.Join(Environment.NewLine, list.SelectMany(g => g));
            
            var minSequenceLength = list.Min(s => s.Key);//*/
            var numberPart = long.Parse(code[..^1]);
            sum += minSequenceLength * numberPart;
        }
        
        return sum.ToString();
    }

    private static IEnumerable<string> NumPadPossibilities(string code, char prevButton)
    {
        var gapPosition = (X: 0, Y: 3);
        if (code.Length == 0)
            return [""];
        var prevButtonCoordinates = MapToNumPadCoordinates(prevButton);
        foreach (var button in code)
        {
            var buttonCoordinates = MapToNumPadCoordinates(button);
            var xDiff = buttonCoordinates.X - prevButtonCoordinates.X;
            var yDiff = buttonCoordinates.Y - prevButtonCoordinates.Y;
            var xPart = "".PadRight(Math.Abs(xDiff), xDiff > 0 ? '>' : '<');
            var yPart = "".PadRight(Math.Abs(yDiff), yDiff > 0 ? 'v' : '^');
            var prefixes = (xDiff, yDiff) switch
            {
                (0, 0) => new[] { "A" },
                (0, _) => new[] { yPart + 'A' },
                (_, 0) => new[] { xPart + 'A' },
                _ when buttonCoordinates.Y == gapPosition.Y && prevButtonCoordinates.X == gapPosition.X => new[] { xPart + yPart + 'A' }, 
                _ when buttonCoordinates.X == gapPosition.X && prevButtonCoordinates.Y == gapPosition.Y => new[] { yPart + xPart + 'A' },
                _ => new[] { xPart + yPart + 'A', yPart + xPart + 'A' }

            };
            return NumPadPossibilities(code[1..], button).SelectMany(s => prefixes.Select(prefix => prefix + s));
        }
        return Array.Empty<string>();
    }

    private static IEnumerable<string> DirPadPossibilities(string code, char prevButton)
    {
        var gapPosition = (X: 0, Y: 0);
        if (code.Length == 0)
            return [""];
        var prevButtonCoordinates = MapToDirPadCoordinates(prevButton);
        foreach (var button in code)
        {
            var buttonCoordinates = MapToDirPadCoordinates(button);
            var xDiff = buttonCoordinates.X - prevButtonCoordinates.X;
            var yDiff = buttonCoordinates.Y - prevButtonCoordinates.Y;
            var xPart = "".PadRight(Math.Abs(xDiff), xDiff > 0 ? '>' : '<');
            var yPart = "".PadRight(Math.Abs(yDiff), yDiff > 0 ? 'v' : '^');
            var prefixes = (xDiff, yDiff) switch
            {
                (0, 0) => new[] { "A" },
                (0, _) => new[] { yPart + 'A' },
                (_, 0) => new[] { xPart + 'A' },
                _ when buttonCoordinates.Y == gapPosition.Y && prevButtonCoordinates.X == gapPosition.X => new[] { xPart + yPart + 'A' }, 
                _ when buttonCoordinates.X == gapPosition.X && prevButtonCoordinates.Y == gapPosition.Y => new[] { yPart + xPart + 'A' },
                _ => new[] { xPart + yPart + 'A', yPart + xPart + 'A' }

            };
            return DirPadPossibilities(code[1..], button).SelectMany(s => prefixes.Select(prefix => prefix + s));
        }
        return Array.Empty<string>();
    }
    
    private static (int X, int Y) MapToNumPadCoordinates(char button) => button switch
    {
        '1' => (0, 2),
        '2' => (1, 2),
        '3' => (2, 2),
        '4' => (0, 1),
        '5' => (1, 1),
        '6' => (2, 1),
        '7' => (0, 0),
        '8' => (1, 0),
        '9' => (2, 0),
        '0' => (1, 3),
        'A' => (2, 3),
        _ => throw new ArgumentOutOfRangeException(nameof(button), button, "Invalid button")
    };
    
    private static (int X, int Y) MapToDirPadCoordinates(char button) => button switch
    {
        '^' => (1, 0),
        'v' => (1, 1),
        '<' => (0, 1),
        '>' => (2, 1),
        'A' => (2, 0),
        _ => throw new ArgumentOutOfRangeException(nameof(button), button, "Invalid button")
    };
    
    /*
+---+---+---+
| 7 | 8 | 9 |
+---+---+---+
| 4 | 5 | 6 |
+---+---+---+
| 1 | 2 | 3 |
+---+---+---+
    | 0 | A |
    +---+---+
    
    +---+---+
    | ^ | A |
+---+---+---+
| < | v | > |
+---+---+---+
     */

    public override string SecondPart()
    {
        var data = ParsedInput.Value;

        var sum = 0L;
        
        return sum.ToString();
    }
}