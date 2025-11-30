namespace AdventOfCode.Days.Year2024;

internal class Day11 : DayOfYear2024<Day11, IReadOnlyList<long>>
{
    public override int Number => 11;

    protected override IReadOnlyList<long> ParseInput()
    {
        var list = new List<long>();
        
        var inputSpan = Input.AsSpan();
        var numberRanges = inputSpan.Split(' ');
        foreach (var numberRange in numberRanges)
            list.Add(long.Parse(inputSpan[numberRange]));
        
        return list;
    }

    long? Zero(long number) => number == 0L ? 1L : null;

    (long Left, long Right)? EvenDigits(long number)
    {
        var digitCount = MathUtils.GetDigitCount(number);
        if (MathUtils.GetDigitCount(number) % 2 != 0)
            return null;
        
        var halfDigitCount = digitCount / 2;
        var text = number.ToString();
        return (Left: long.Parse(text[..(int)halfDigitCount]), Right: long.Parse(text[(int)halfDigitCount..]));
    }

    long Remainder(long number) => number * 2024L;
    
    public override string FirstPart()
    {
        return ParsedInput.Value
            .Sum(number => CountOfStones(number, 25))
            .ToString();

        long CountOfStones(long number, int stepCount)
        {
            if (stepCount == 0) return 1;
            if (Zero(number) is {} zero)
                return CountOfStones(zero, stepCount - 1);
            if (EvenDigits(number) is {} tuple)
                return CountOfStones(tuple.Item1, stepCount - 1) + CountOfStones(tuple.Item2, stepCount - 1);
            return CountOfStones(Remainder(number), stepCount - 1);
        }
    }

    public override string SecondPart()
    {
        var numbers = ParsedInput.Value.CountBy(n => n)
            .ToDictionary(g => g.Key, g => (long) g.Value);
        for (var i = 0; i < 75; i++)
        {
            var newNumbers = new Dictionary<long, long>();
            foreach (var numberToCount in numbers)
            {
                var number = numberToCount.Key;
                var count = numberToCount.Value;
                if (Zero(number) is { } zero)
                {
                    AddNumber(zero, count);
                }
                else if (EvenDigits(number) is { } tuple)
                {
                    AddNumber(tuple.Item1, count);
                    AddNumber(tuple.Item2, count);
                }
                else
                {
                    AddNumber(Remainder(number), count);
                }

                numbers = newNumbers;
                continue;
                
                void AddNumber(long number, long count)
                {
                    if (newNumbers.TryGetValue(number, out var currentCount))
                        newNumbers[number] = currentCount + count;
                    else 
                        newNumbers[number] = count;
                }
            }
        }
        return numbers.Sum(kvp => kvp.Value).ToString();
    }
}