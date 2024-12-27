namespace AdventOfCode.Days;

internal class Day17 : DayBase<Day17, Day17.Data>
{
    internal record Data(long A, long B, long C, IReadOnlyList<long> Program);

    internal record Runtime(long A, long B, long C, long InstructionPointer, string Output);
    
    public override int Number => 17;

    protected override Data ParseInput()
    {
        var inputSpan = Input.AsSpan();
        var inputPartRanges = inputSpan.Split(Environment.NewLine + Environment.NewLine);
        
        if (!inputPartRanges.MoveNext())
            throw new InvalidOperationException("Expected program");
        var registersSpan = inputSpan[inputPartRanges.Current];
        var registerLineRanges = registersSpan.Split(Environment.NewLine);
        if (!registerLineRanges.MoveNext())
            throw new InvalidOperationException("Expected registers");
        var a = long.Parse(registersSpan[registerLineRanges.Current][12..]);
        if (!registerLineRanges.MoveNext())
            throw new InvalidOperationException("Expected registers");
        var b = long.Parse(registersSpan[registerLineRanges.Current][12..]);
        if (!registerLineRanges.MoveNext())
            throw new InvalidOperationException("Expected registers");
        var c = long.Parse(registersSpan[registerLineRanges.Current][12..]);

        if (!inputPartRanges.MoveNext())
            throw new InvalidOperationException("Expected program");
        var programSpan = inputSpan[inputPartRanges.Current][9..];
        var programPartRanges = programSpan.Split(',');
        var program = new List<long>();
        foreach (var programPartRange in programPartRanges)
        {
            program.Add(long.Parse(programSpan[programPartRange]));
        }
        
        return new Data(a, b, c, program);
    }

    string RunProgram(Data programData)
    {
        var program = programData.Program;
        var run = new Runtime(programData.A, programData.B, programData.C, 0, string.Empty);

        while (run.InstructionPointer >= 0 && run.InstructionPointer < program.Count)
        {
            var instruction = program[(int) run.InstructionPointer];
            var operand = program[(int) run.InstructionPointer + 1];

            run = instruction switch
            {
                0 => Adv(run, operand),
                1 => Bxl(run, operand),
                2 => Bst(run, operand),
                3 => Jnz(run, operand),
                4 => Bxc(run),
                5 => Out(run, operand),
                6 => Bdv(run, operand),
                7 => Cdv(run, operand),
                _ => throw new InvalidOperationException($"Unknown instruction {instruction}")
            };
        }
        
        return run.Output;

        Runtime Adv(Runtime runtime, long operand) =>
            runtime with { A = (long) (runtime.A / Math.Pow(2,Combo(runtime, operand))), InstructionPointer = runtime.InstructionPointer + 2 };

        Runtime Bxl(Runtime runtime, long operand) =>
            runtime with { B = runtime.B ^ operand, InstructionPointer = runtime.InstructionPointer + 2 };

        Runtime Bst(Runtime runtime, long operand) =>
            runtime with { B = Combo(runtime, operand) % 8, InstructionPointer = runtime.InstructionPointer + 2 };

        Runtime Jnz(Runtime runtime, long operand) =>
            runtime.A == 0 
                ? runtime with { InstructionPointer = runtime.InstructionPointer + 2 } 
                : runtime with { InstructionPointer = operand };

        Runtime Bxc(Runtime runtime) =>
            runtime with { B = runtime.B ^ runtime.C, InstructionPointer = runtime.InstructionPointer + 2 };

        Runtime Out(Runtime runtime, long operand) =>
            runtime with { Output = string.IsNullOrWhiteSpace(runtime.Output) ? (Combo(runtime, operand) % 8).ToString() : $"{runtime.Output},{Combo(runtime, operand) % 8}", InstructionPointer = runtime.InstructionPointer + 2 };

        Runtime Bdv(Runtime runtime, long operand) =>
            runtime with { B = (long) (runtime.A / Math.Pow(2,Combo(runtime, operand))), InstructionPointer = runtime.InstructionPointer + 2 };

        Runtime Cdv(Runtime runtime, long operand) =>
            runtime with { C = (long) (runtime.A / Math.Pow(2,Combo(runtime, operand))), InstructionPointer = runtime.InstructionPointer + 2 };

        long Combo(Runtime runtime, long operand) =>
            operand switch
            {
                >= 0 and <= 3 => operand,
                4 => runtime.A,
                5 => runtime.B,
                6 => runtime.C,
                _ => throw new InvalidOperationException($"Unknown operand {operand}")
            };
    }

    public override string FirstPart() => RunProgram(ParsedInput.Value);

    public override string SecondPart()
    {
        if (SampleNumber == 1) 
            return Consts.NothingToDoHere; // This sample most likely isn't valid for the second part
        var data = ParsedInput.Value;
        var a = (long) Math.Pow(8, ParsedInput.Value.Program.Count - 1);
        for (var i = ParsedInput.Value.Program.Count - 1; i >= 0; i--)
        {
            var seekedProgramEnd = string.Join(",", ParsedInput.Value.Program.TakeLast(ParsedInput.Value.Program.Count - i));
            var increment = (long) Math.Pow(8, i);
            var output = RunProgram(data with { A = a });
            while (!output.EndsWith(seekedProgramEnd))
            {
                a += increment;
                output = RunProgram(data with { A = a });
            }
        }
        
        return a.ToString();
    }
}