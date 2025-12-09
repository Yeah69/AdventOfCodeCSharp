namespace AdventOfCode.Days.Year2025;

internal class Day09 : DayOfYear2025<Day09, List<Day09.Coordinates>>
{
    internal record struct Coordinates(long X, long Y);
    public override int Number => 9;

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
            coordinates.Add(new (x, y));
        }

        return coordinates;
    }

    public override string FirstPart()
    {
        var coordinates = ParsedInput.Value;
        var maxArea = long.MinValue;
        
        for (var i = 0; i < coordinates.Count; i++)
        {
            var firstCorner = coordinates[i];
            for (var j = i + 1; j < coordinates.Count; j++)
            {
                var secondCorner = coordinates[j];
                var area = (Math.Abs(firstCorner.X - secondCorner.X) + 1) * (Math.Abs(firstCorner.Y - secondCorner.Y) + 1);
                if (area > maxArea)
                    maxArea = area;
            }
        }

        return maxArea.ToString();
    }

    public override string SecondPart()
    {
        var coordinates = ParsedInput.Value;
        var maxArea = long.MinValue;
        
        for (var i = 0; i < coordinates.Count; i++)
        {
            var firstCorner = coordinates[i];
            for (var j = i + 1; j < coordinates.Count; j++)
            {
                var secondCorner = coordinates[j];
                var minX = Math.Min(firstCorner.X, secondCorner.X);
                var minY = Math.Min(firstCorner.Y, secondCorner.Y);
                var maxX = Math.Max(firstCorner.X, secondCorner.X);
                var maxY = Math.Max(firstCorner.Y, secondCorner.Y);
                
                var anyInside = coordinates.Any(c => c != firstCorner &&  c != secondCorner && c.X > minX  && c.X < maxX && c.Y > minY && c.Y < maxY);
                if (anyInside)
                    continue;

                var anyCrossingLine = coordinates.Select((lineStart, k) =>
                {
                    var lineEnd = coordinates[(k + 1) % coordinates.Count];
                    if (lineStart == firstCorner || lineStart == secondCorner || lineEnd == firstCorner ||
                        lineEnd == secondCorner)
                        return false;
                    if (lineStart.X == lineEnd.X && lineStart.X > minX && lineStart.X < maxX)
                    {
                        var minLineY = Math.Min(lineStart.Y, lineEnd.Y);
                        var maxLineY = Math.Max(lineStart.Y, lineEnd.Y);
                        return minLineY <= minY && maxLineY > minY || maxLineY >= maxY && minLineY < maxY;
                    }

                    if (lineStart.Y == lineEnd.Y && lineStart.Y > minY && lineStart.Y < maxY)
                    {
                        var minLineX = Math.Min(lineStart.X, lineEnd.X);
                        var maxLineX = Math.Max(lineStart.X, lineEnd.X);
                        return minLineX <= minX && maxLineX > minX || maxLineX >= maxX && minLineX < maxX;
                    }

                    return false;
                }).Any(b => b);
                
                var area = (maxX - minX + 1)  * (maxY - minY + 1);
                
                if (!anyCrossingLine && area > maxArea)
                    maxArea = area;
            }
        }

        return maxArea.ToString();
    }
}