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
    
    private static IEnumerable<string> Possibilities(string code, char prevButton, Func<char, (int X, int Y)> mapToCoordinates, (int X, int Y) gapPosition)
    {
        if (code.Length == 0)
            return [""];
        var prevButtonCoordinates = mapToCoordinates(prevButton);
        foreach (var button in code)
        {
            var buttonCoordinates = mapToCoordinates(button);
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
            return Possibilities(code[1..], button, mapToCoordinates, gapPosition).SelectMany(s => prefixes.Select(prefix => prefix + s));
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

    private static readonly (int X, int Y) DirPadGap = (0, 0);
    private static readonly (int X, int Y) NumPadGap = (0, 3);
    private static readonly char[] AllDirButtons = ['^', 'v', '<', '>', 'A'];
    private string Solve(int dirSteps)
    {
        var data = ParsedInput.Value;

        var sum = 0L;
        
        var dirMap = AllDirButtons.SelectMany(b1 => AllDirButtons.Select(b2 => (b1, b2)))
            .ToDictionary(t => t, t =>
            {
                var prevButton = t.b1;
                var code = t.b2.ToString();
                return Possibilities(code, prevButton, MapToDirPadCoordinates, DirPadGap)
                    .Select(s => (long) s.Length)
                    .Min();
            });

        for (var i = 0; i < dirSteps - 1; i++)
        {
            dirMap = AllDirButtons.SelectMany(b1 => AllDirButtons.Select(b2 => (b1, b2)))
                .ToDictionary(t => t, t =>
                {
                    var prevButton = t.b1;
                    var code = t.b2.ToString();
                    return Possibilities(code, prevButton, MapToDirPadCoordinates, DirPadGap)
                        .Select(s => s.Prepend('A')
                            .Zip(s, (prev, current) => dirMap[(prev, current)])
                            .Sum())
                        .Min();
                });
        }
        
        foreach (var code in data.Codes)
        {
            var minSequenceLength = Possibilities(code, 'A', MapToNumPadCoordinates, NumPadGap)
                .Select(s => s.Prepend('A').Zip(s, (prev, current) => dirMap[(prev, current)]).Sum())
                .Min();

            var numberPart = long.Parse(code[..^1]);
            sum += minSequenceLength * numberPart;
        }
        
        return sum.ToString();
    }

    public override string FirstPart() => Solve(2);

    public override string SecondPart() => Solve(25);
}