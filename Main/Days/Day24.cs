namespace AdventOfCode.Days;

internal class Day24 : DayBase<Day24, Day24.Data>
{
    internal interface INode
    {
        bool Value { get; }
    }
    private record VariableNode(string Label, bool Value) : INode
    {
        public bool Value { get; set; } = Value;
    }

    private enum Operation { And, Or, XOr }
    
    private record LogicNode(string Label, Operation Operation, INode Left, INode Right) : INode
    {
        public Operation Operation { get; set; } = Operation;
        public INode Left { get; set; } = Left;
        public INode Right { get; set; } = Right;
        public bool Value => Operation switch
        {
            Operation.And => Left.Value && Right.Value,
            Operation.Or => Left.Value || Right.Value,
            Operation.XOr => Left.Value ^ Right.Value,
            _ => throw new Exception("Invalid operation")
        };
    }

    internal record Data(IReadOnlyList<INode> Nodes);
    public override int Number => 24;

    protected override Data ParseInput()
    {
        var map = new Dictionary<string, INode>();
        var inputSpan = Input.AsSpan();
        var inputPartRanges = inputSpan.Split(Environment.NewLine + Environment.NewLine);
        
        if (!inputPartRanges.MoveNext())
            throw new Exception("No input data found");
        var initialPartSpan = inputSpan[inputPartRanges.Current];
        var initialPartLineRanges = initialPartSpan.Split(Environment.NewLine);
        foreach (var initialPartLineRange in initialPartLineRanges)
        {
            var initialAssignmentSpan = initialPartSpan[initialPartLineRange];
            var label = initialAssignmentSpan[..3].ToString();
            var value = int.Parse(initialAssignmentSpan[5].ToString()) is 1;
            map.Add(label, new VariableNode(label, value));
        }
        
        if (!inputPartRanges.MoveNext())
            throw new Exception("No input data found");
        var assignmentPartSpan = inputSpan[inputPartRanges.Current];
        var assignmentPartLineRanges = assignmentPartSpan.Split(Environment.NewLine);
        var delayQueue = new Queue<(Operation Operation, string Left, string Right, string Result)>();
        foreach (var assignmentPartLineRange in assignmentPartLineRanges)
        {
            var assignmentSpan = assignmentPartSpan[assignmentPartLineRange];
            var leftLabel = assignmentSpan[..3].ToString();
            var resultLabel = assignmentSpan[^3..].ToString();
            if (assignmentSpan[4..7] is "AND")
            {
                var rightLabel = assignmentSpan[8..11].ToString();
                if (map.TryGetValue(leftLabel, out var left) && map.TryGetValue(rightLabel, out var right))
                    map.Add(resultLabel, new LogicNode(resultLabel, Operation.And, left, right));
                else
                    delayQueue.Enqueue((Operation.And, leftLabel, rightLabel, resultLabel));
            }
            else if (assignmentSpan[4..7] is "OR ")
            {
                var rightLabel = assignmentSpan[7..10].ToString();
                if (map.TryGetValue(leftLabel, out var left) && map.TryGetValue(rightLabel, out var right))
                    map.Add(resultLabel, new LogicNode(resultLabel, Operation.Or, left, right));
                else
                    delayQueue.Enqueue((Operation.Or, leftLabel, rightLabel, resultLabel));
            }
            else if (assignmentSpan[4..7] is "XOR")
            {
                var rightLabel = assignmentSpan[8..11].ToString();
                if (map.TryGetValue(leftLabel, out var left) && map.TryGetValue(rightLabel, out var right))
                    map.Add(resultLabel, new LogicNode(resultLabel, Operation.XOr, left, right));
                else
                    delayQueue.Enqueue((Operation.XOr, leftLabel, rightLabel, resultLabel));
            }
        }
        
        while (delayQueue.Count > 0)
        {
            var (operation, leftLabel, rightLabel, resultLabel) = delayQueue.Dequeue();
            if (map.TryGetValue(leftLabel, out var left) && map.TryGetValue(rightLabel, out var right))
                map.Add(resultLabel, new LogicNode(resultLabel, operation, left, right));
            else
                delayQueue.Enqueue((operation, leftLabel, rightLabel, resultLabel));
        }
        
        return new(map.Values.ToList());
    }
    
    private static long GetDecimalNumber(IReadOnlyList<LogicNode> nodes) =>
        nodes.Select((n, i) => n.Value ? 1L << i : 0L).Sum();

    public override string FirstPart() => 
        GetDecimalNumber(ParsedInput.Value.Nodes
                .OfType<LogicNode>()
                .Where(n => n.Label.StartsWith('z'))
                .OrderBy(n => n.Label)
                .ToList())
            .ToString();

    public override string SecondPart()
    {
        if (SampleNumber > 0) 
            return Consts.NothingToDoHere;
        
        var data = ParsedInput.Value;
        var pairs = new List<(LogicNode, LogicNode)>
        {
            (data.Nodes.OfType<LogicNode>().Single(n => n.Label == "djg"), data.Nodes.OfType<LogicNode>().Single(n => n.Label == "z12")),
            (data.Nodes.OfType<LogicNode>().Single(n => n.Label == "sbg"), data.Nodes.OfType<LogicNode>().Single(n => n.Label == "z19")),
            (data.Nodes.OfType<LogicNode>().Single(n => n.Label == "hjm"), data.Nodes.OfType<LogicNode>().Single(n => n.Label == "mcq")),
            (data.Nodes.OfType<LogicNode>().Single(n => n.Label == "dsd"), data.Nodes.OfType<LogicNode>().Single(n => n.Label == "z37"))
        };
        foreach (var (first, second) in pairs)
            SwapNodes(first, second);
        
        var xNodes = data.Nodes.OfType<VariableNode>().Where(n => n.Label.StartsWith('x')).OrderBy(n => n.Label).ToList();
        var yNodes = data.Nodes.OfType<VariableNode>().Where(n => n.Label.StartsWith('y')).OrderBy(n => n.Label).ToList();
        var zNodes = data.Nodes.OfType<LogicNode>().Where(n => n.Label.StartsWith('z')).OrderBy(n => n.Label).ToList();
        
        /*var graphViz = new StringBuilder();
        graphViz.AppendLine(
            """
            digraph
            {
            """);
        foreach (var dataNode in data.Nodes)
        {
            if (dataNode is VariableNode variableNode)
                graphViz.AppendLine($"    {variableNode.Label} [shape=box, label=\"{variableNode.Label}_{variableNode.Value}\"]");
            else if (dataNode is LogicNode logicNode)
                graphViz.AppendLine($"    {logicNode.Label} [shape=ellipse, label=\"{logicNode.Label}_{logicNode.Operation}_{logicNode.Value([])}\"]");
        }
        foreach (var logicNode in logicNodes)
        {
            graphViz.AppendLine($"    {logicNode.Label} -> {logicNode.Left.Label}");
            graphViz.AppendLine($"    {logicNode.Label} -> {logicNode.Right.Label}");
        }
        graphViz.AppendLine("}");
        var graphVizString = graphViz.ToString();//*/
        
        
        SetDecimalNumber(xNodes, 0);
        SetDecimalNumber(yNodes, 0);
        var zeroResult = GetDecimalNumber(zNodes);
        if (zeroResult != 0)
            return Consts.NoSolutionFound;

        for (var i = 0; i < zNodes.Count - 1; i++)
        {
            if (TryAll(i).Any(b => !b))
                return Consts.NoSolutionFound;
            continue;

            IEnumerable<bool> TryAll(int j)
            {
                yield return Try(1L << j, 0);
                yield return Try(0, 1L << j);
                yield return Try(1L << j, 1L << j);
            }

            bool Try(long newX, long newY)
            {
                var seekedNewZ = newX + newY;
                SetDecimalNumber(xNodes, newX);
                SetDecimalNumber(yNodes, newY);
                var newZ = GetDecimalNumber(zNodes);
                return newZ == seekedNewZ;
            }
        }
        
        return string.Join(",", pairs.SelectMany(p => new[] {p.Item1.Label, p.Item2.Label}).Order());
        
        static void SetDecimalNumber(IReadOnlyList<VariableNode> nodes, long number)
        {
            var digitNodes = nodes
                .Select((n, i) => (n, i));

            foreach (var (node, i) in digitNodes)
            {
                var newValue = (number >>> i & 1) is 1;
                node.Value = newValue;
            }
        }

        static void SwapNodes(LogicNode first, LogicNode second)
        {
            var firstProperties = (first.Operation, first.Left, first.Right);
            var secondProperties = (second.Operation, second.Left, second.Right);
        
            first.Operation = secondProperties.Operation;
            first.Left = secondProperties.Left;
            first.Right = secondProperties.Right;
            second.Operation = firstProperties.Operation;
            second.Left = firstProperties.Left;
            second.Right = firstProperties.Right;
        }
    }
}