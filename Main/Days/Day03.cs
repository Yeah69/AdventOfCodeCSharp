namespace AdventOfCode.Days;

internal class Day03 : DayBase<Day03, string>
{
    public override int Number => 3;

    private string Solve(bool withDoDont)
    {
        var multStartSpan = "mul(".AsSpan();
        var doSpan = "do()".AsSpan();
        var dontSpan = "don't()".AsSpan();
        var result = 0L;
        var isEnabled = true;
        var currentSpan = ParsedInput.Value.AsSpan();
        while (currentSpan.Length > 0)
        {
            if (currentSpan.StartsWith(multStartSpan) && isEnabled)
            {
                currentSpan = currentSpan[multStartSpan.Length..];
                if (TryParseNumberNext(ref currentSpan, ',') is not {} firstNumber) 
                    continue;
                if (TryParseNumberNext(ref currentSpan, ')') is not {} secondNumber) 
                    continue;
                result += firstNumber * secondNumber;
            } 
            else if (withDoDont)
            {
                if (currentSpan.StartsWith(doSpan))
                {
                    isEnabled = true;
                    currentSpan = currentSpan[doSpan.Length..];
                }
                else if (currentSpan.StartsWith(dontSpan))
                {
                    isEnabled = false;
                    currentSpan = currentSpan[dontSpan.Length..];
                }
                else
                    currentSpan = currentSpan[1..];
            }
            else
                currentSpan = currentSpan[1..];
        }
        return result.ToString();

        long? TryParseNumberNext(ref ReadOnlySpan<char> span, char separator)
        {
            var i = 0;
            while (span.Length > i && char.IsDigit(span[i]))
                ++i;
            if (i == 0)
                return null;
            var digits = i;
            var numberSpan = span[..i];
            span = span[digits..];
            if (i > 3 || span[0] != separator) 
                return null;
            span = span[1..];
            return long.Parse(numberSpan);
        }
    }

    protected override string ParseInput() => Input;

    public override string FirstPart() => Solve(withDoDont: false);

    public override string SecondPart() => Solve(withDoDont: true);
}