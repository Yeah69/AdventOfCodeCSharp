namespace AdventOfCode.Days.Year2025;

internal class Day06 : DayOfYear2025<Day06, (List<List<string>> Numbers, List<char> Operators)>
{
    public override int Number => 6;

    protected override (List<List<string>> Numbers, List<char> Operators) ParseInput()
    {
        var numbers = new List<List<string>>();

        var span = Input.AsSpan();
        var lastNewLine = span.LastIndexOf(Environment.NewLine);
        var operatorsSpan = span[(lastNewLine + Environment.NewLine.Length)..];
        var operators = new List<char>();

        var start = 0;
        var ranges = new List<Range>();

        for (var i = 0; i < operatorsSpan.Length; i++)
        {
            var character = operatorsSpan[i];
            if (character is not ' ')
            {
                if (i is not 0)
                    ranges.Add(new Range(start, i - 1));
                operators.Add(character);
                start = i;
            }
            if (i == operatorsSpan.Length - 1)
                ranges.Add(new Range(start, i + 1));
        }
        
        var numbersSpan = span[..lastNewLine];

        var lineRanges = numbersSpan.Split(Environment.NewLine);
        foreach (var lineRange in lineRanges)
        {
            var lineSpan = numbersSpan[lineRange];
            var lineNumbers = new List<string>(ranges.Count);
            foreach (var range in ranges)
                lineNumbers.Add(lineSpan[range].ToString());
            numbers.Add(lineNumbers);
        }

        return (numbers, operators);
    }

    public override string FirstPart()
    {
        var (numbers, operators) = ParsedInput.Value;

        var columns = numbers.First().Count;
        var result = 0L;

        for (var column = 0; column < columns; column++)
        {
            var seed = operators[column] is '+' ? 0L : 1L;
            var lineResult = numbers.Select(numberLine => numberLine[column])
                .Aggregate(
                    seed, 
                    (current, number) =>
                    {
                        var actualNumber = long.Parse(number.Trim());
                        return operators[column] is '+' ? current + actualNumber : current * actualNumber;
                    });
            result += lineResult;   
        }

        return result.ToString();
    }

    public override string SecondPart()
    {
        var (numbers, operators) = ParsedInput.Value;

        var result = 0L;

        for (var column = 0; column < operators.Count; column++)
        {
            var seed = operators[column] is '+' ? 0L : 1L;
            var lineResult = Enumerable.Range(0, numbers[0][column].Length)
                .Aggregate(
                    seed, 
                    (current, i) =>
                    {
                        var actualNumber = GetActualNumber(i);
                        return operators[column] is '+' ? current + actualNumber : current * actualNumber;
                    });
            result += lineResult;
            continue;

            long GetActualNumber(int index)
            {
                var text = new string(numbers.Select(numberLine => numberLine[column][index]).ToArray());
                return long.Parse(text);
            }
        }

        return result.ToString();
    }
}