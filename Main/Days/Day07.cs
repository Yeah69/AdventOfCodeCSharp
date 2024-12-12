using System.Runtime.InteropServices;

namespace AdventOfCode.Days;

internal class Day07 : DayBase<Day07, IReadOnlyList<Day07.Equation>>
{
    internal record Equation(long Value, List<long> Components);
    public override int Number => 7;

    protected override IReadOnlyList<Equation> ParseInput()
    {
        var inputSpan = Input.AsSpan();
        var equations = new List<Equation>();
        var lineRanges = inputSpan.Split(Environment.NewLine);
        foreach (var lineRange in lineRanges)
        {
            var lineSpan = inputSpan[lineRange];
            var linePartRanges = lineSpan.Split(':');
            if (!linePartRanges.MoveNext())
                throw new();
            var value = long.Parse(lineSpan[linePartRanges.Current]);
            
            if (!linePartRanges.MoveNext())
                throw new();
            var components = new List<long>();
            var componentsSpan = lineSpan[linePartRanges.Current];
            var componentsRanges = componentsSpan.Split(' ');
            foreach (var componentsRange in componentsRanges)
            {
                if (componentsRange.Start.Value == componentsRange.End.Value)
                    continue;
                components.Add(long.Parse(componentsSpan[componentsRange]));
            }
            equations.Add(new(value, components));
        }
        return equations;
    }

    private static bool CanBeSolved(ReadOnlySpan<long> components, long value, long previous, bool withConcatenation)
    {
        if (components.Length == 0)
            return previous == value;
        if (previous > value)
            return false;
        return CanBeSolved(components[1..], value, previous + components[0], withConcatenation)
               || CanBeSolved(components[1..], value, previous * components[0], withConcatenation)
               || withConcatenation 
               && CanBeSolved(components[1..], value, Concatenate(previous, components[0]), withConcatenation);

        long Concatenate(long a, long b) => a * (long)Math.Pow(10, MathUtils.GetDigitCount(b)) + b;
    }
    
    private string Solve(bool withConcatenation) => ParsedInput
        .Value
        .Where(equation =>
        {
            var componentsSpan = CollectionsMarshal.AsSpan(equation.Components);
            return CanBeSolved(componentsSpan[1..], equation.Value, componentsSpan[0], withConcatenation);
        })
        .Sum(equation => equation.Value)
        .ToString();
    
    public override string FirstPart() => Solve(withConcatenation: false);

    public override string SecondPart() => Solve(withConcatenation: true);
}