namespace AdventOfCode.Days;

internal class Day23 : DayBase<Day23, Day23.Data>
{
    internal record Data(IReadOnlyList<(string Left, string Right)> Connections, IReadOnlyDictionary<string, HashSet<string>> Map);
    public override int Number => 23;

    protected override Data ParseInput()
    {
        var inputSpan = Input.AsSpan();
        var lineRanges = inputSpan.Split(Environment.NewLine);
        var connections = new List<(string Left, string Right)>();
        
        foreach (var lineRange in lineRanges)
        {
            var lineSpan = inputSpan[lineRange];
            var partRanges = lineSpan.Split("-");
            if (!partRanges.MoveNext())
                throw new Exception("Invalid input");
            var left = lineSpan[partRanges.Current].ToString();
            if (!partRanges.MoveNext())
                throw new Exception("Invalid input");
            var right = lineSpan[partRanges.Current].ToString();
            connections.Add((left, right));
        }
        
        var map = connections.GroupBy(c => c.Left, c => c.Right)
            .Concat(connections.GroupBy(c => c.Right, c => c.Left))
            .GroupBy(g => g.Key, g => g, (key, values) => (key, values.SelectMany(v => v)))
            .ToDictionary(g => g.key, g => g.Item2.ToHashSet());
        
        return new(connections, map);
    }

    public override string FirstPart()
    {
        var data = ParsedInput.Value;

        var seekedTriples = new HashSet<(string Left, string Middle, string Right)>();
        
        foreach (var (left, right) in data.Connections)
        {
            var leftConnections = data.Map[left];
            var rightConnections = data.Map[right];
            var alreadyT = left[0] == 't' || right[0] == 't';
            foreach (var middle in leftConnections.Where(c => (alreadyT || c[0] == 't') && rightConnections.Contains(c)))
            {
                var sorted = new[] { left, middle, right }.Order().ToArray();
                seekedTriples.Add((sorted[0], sorted[1], sorted[2]));
            }
        }
        
        return seekedTriples.Count.ToString();
    }

    public override string SecondPart()
    {
        var data = ParsedInput.Value;

        var processed = new HashSet<string>();
        var maxClique = data.Map.OrderBy(kvp => kvp.Value.Count)
            .Where(kvp => !processed.Contains(kvp.Key))
            .Select(kvp =>
            {
                var clique = new List<string> { kvp.Key };
                foreach (var candidate in kvp.Value)
                {
                    var candidateConnections = data.Map[candidate];
                    if (clique.All(c => candidateConnections.Contains(c)))
                        clique.Add(candidate);
                }
                processed.UnionWith(clique);
                return clique.Order().ToArray();
            })
            .MaxBy(c => c.Length);
        
        return maxClique is not null ? string.Join(",", maxClique) : Consts.NoSolutionFound;
    }
}