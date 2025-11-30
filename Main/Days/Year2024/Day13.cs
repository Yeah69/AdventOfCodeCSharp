namespace AdventOfCode.Days.Year2024;

internal class Day13 : DayOfYear2024<Day13, Day13.Data>
{
    internal record ClawMachine((long X, long Y) A, (long X, long Y) B, (long X, long Y) Prize);
    internal record Data(IReadOnlyList<ClawMachine> ClawMachines);
    public override int Number => 13;

    protected override Data ParseInput()
    {
        var inputSpan = Input.AsSpan();
        var clawMachineRanges = inputSpan.Split(Environment.NewLine + Environment.NewLine);
        var clawMachines = new List<ClawMachine>();
        
        foreach (var clawMachineRange in clawMachineRanges)
        {
            var clawMachineSpan = inputSpan[clawMachineRange];
            var clawMachineLineRanges = clawMachineSpan.Split(Environment.NewLine);
            
            // Button A
            if (!clawMachineLineRanges.MoveNext())
                throw new InvalidOperationException("No lines found for claw machine.");
            var aButton = Get(clawMachineSpan[clawMachineLineRanges.Current], ", Y+", 12);
            
            // Button B
            if (!clawMachineLineRanges.MoveNext())
                throw new InvalidOperationException("No lines found for claw machine.");
            var bButton = Get(clawMachineSpan[clawMachineLineRanges.Current], ", Y+", 12);
            
            // Prize
            if (!clawMachineLineRanges.MoveNext())
                throw new InvalidOperationException("No lines found for claw machine.");
            var prize = Get(clawMachineSpan[clawMachineLineRanges.Current], ", Y=", 9);
            clawMachines.Add(new(aButton, bButton, prize));
            continue;

            (long X, long Y) Get(ReadOnlySpan<char> span, ReadOnlySpan<char> separator, int trim)
            {
                var partRanges = span.Split(separator);
                if (!partRanges.MoveNext())
                    throw new InvalidOperationException("No parts found.");
                var x = long.Parse(span[(partRanges.Current.Start.Value + trim)..partRanges.Current.End.Value]);
                if (!partRanges.MoveNext())
                    throw new InvalidOperationException("No parts found.");
                var y = long.Parse(span[partRanges.Current]);
                return (x, y);
            }
        }
        
        return new Data(clawMachines);
    }

    private static long GetCosts(ClawMachine clawMachine)
    {
        double xa = clawMachine.A.X;
        double ya = clawMachine.A.Y;
        double xb = clawMachine.B.X;
        double yb = clawMachine.B.Y;
        double xp = clawMachine.Prize.X;
        double yp = clawMachine.Prize.Y;
        var quotient = xb - xa * yb / ya;
        var b = (xp - xa * yp / ya) / quotient;
        var a = (yp - yb * b) / ya;
        var lA = (long) Math.Round(a);
        var lB = (long) Math.Round(b);
        if (clawMachine.A.X * lA + clawMachine.B.X * lB != clawMachine.Prize.X 
            || clawMachine.A.Y * lA + clawMachine.B.Y * lB != clawMachine.Prize.Y)
            return 0L;
        return 3L * lA  + lB;
    }

    public override string FirstPart() => ParsedInput.Value.ClawMachines.Sum(GetCosts).ToString();

    public override string SecondPart() => ParsedInput.Value.ClawMachines
        .Select(cm => cm with { Prize = (cm.Prize.X + 10000000000000, cm.Prize.Y + 10000000000000) })
        .Sum(GetCosts)
        .ToString();
}