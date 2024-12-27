namespace AdventOfCode.Days;

internal class Day22 : DayBase<Day22, Day22.Data>
{
    internal record Data(IReadOnlyList<long> InitialSecretNumbers);
    public override int Number => 22;

    protected override Data ParseInput()
    {
        var list = new List<long>();
        var inputSpan = Input.AsSpan();
        var lineRanges = inputSpan.Split(Environment.NewLine);
        foreach (var lineRange in lineRanges)
        {
            list.Add(long.Parse(inputSpan[lineRange]));
        }
        
        return new(list);
    }
    
    private static long MixAndPrune(long newResult, long secretNumber) => (newResult ^ secretNumber) % 16777216;

    public override string FirstPart()
    {
        var data = ParsedInput.Value;
        
        var sum = 0L;
        foreach (var dataInitialSecretNumber in data.InitialSecretNumbers)
        {
            var secretNumber = dataInitialSecretNumber;
            for (var i = 0; i < 2000; i++)
            {
                secretNumber = MixAndPrune(secretNumber * 64, secretNumber);
                secretNumber = MixAndPrune(secretNumber / 32, secretNumber);
                secretNumber = MixAndPrune(secretNumber * 2048, secretNumber);
            }
            sum += secretNumber;
        }
        
        return sum.ToString();
    }

    public override string SecondPart()
    {
        var data = ParsedInput.Value;
        
        var j = 0;
        var buyersSequenceses = new Dictionary<(long, long, long, long), long>[data.InitialSecretNumbers.Count];
        foreach (var dataInitialSecretNumber in data.InitialSecretNumbers)
        {
            var secretNumbers = new long[2001];
            var secretNumber = dataInitialSecretNumber;
            secretNumbers[0] = secretNumber;
            for (var i = 1; i < 2001; i++)
            {
                secretNumber = MixAndPrune(secretNumber * 64, secretNumber);
                secretNumber = MixAndPrune(secretNumber / 32, secretNumber);
                secretNumber = MixAndPrune(secretNumber * 2048, secretNumber);
                secretNumbers[i] = secretNumber;
            }
            
            var changesAndPrices = new (long Change, long Price)[2000];
            for (var i = 0; i < 2000; i++)
            {
                var previous = secretNumbers[i] % 10;
                var current = secretNumbers[i + 1] % 10;
                var change = current - previous;
                changesAndPrices[i] = (change, current);
            }

            var buyersSequences = Enumerable.Range(3, 1997)
                .Select(i =>
                {
                    var changeSequence = (changesAndPrices[i - 3].Change, changesAndPrices[i - 2].Change,
                        changesAndPrices[i - 1].Change, changesAndPrices[i].Change);
                    return (changeSequence, changesAndPrices[i].Price);
                })
                .GroupBy(t => t.changeSequence, t => t.Price)
                .Select(g => (g.Key, g.First()))
                .ToDictionary(g => g.Key, g => g.Item2);
            
            buyersSequenceses[j] = buyersSequences;
            j++;
        }
        
        return buyersSequenceses.SelectMany(d => d)
            .GroupBy(kvp => kvp.Key, kvp => kvp.Value)
            .Select(g => g.Sum())
            .Max()
            .ToString();
    }
}