using System.Collections.Immutable;
using LpSolveDotNet;

namespace AdventOfCode.Days.Year2025;

internal class Day10 : DayOfYear2025<Day10, List<Day10.Machine>>
{
    static Day10() => 
        LpSolve.Init();

    internal record struct Machine(ImmutableArray<bool> Lights, ImmutableArray<ImmutableArray<int>> Buttons, ImmutableArray<int> Joltage);
    public override int Number => 10;

    protected override List<Machine> ParseInput()
    {
        var span = Input.AsSpan();
        var lineRanges = span.Split(Environment.NewLine);
        var machines = new List<Machine>();

        foreach (var lineRange in lineRanges)
        {
            var lineSpan = span[lineRange];
            var linePartRanges = lineSpan.Split(' ');
            var lights = ImmutableArray<bool>.Empty;
            var buttons = new List<ImmutableArray<int>>();
            var joltage = new ImmutableArray<int>();
            
            foreach (var linePartRange in linePartRanges)
            {
                var linePart = lineSpan[linePartRange];
                var discriminator = linePart[0];
                linePart = linePart[1..^1];
                if (discriminator is '[')
                {
                    var flags = new List<bool>();
                    foreach (var character in linePart)
                        flags.Add(character == '#');
                    lights = [..flags];
                }
                else if (discriminator is '(')
                {
                    var numbers = new List<int>();
                    var numberRanges = linePart.Split(',');
                    
                    foreach (var numberRange in numberRanges)
                        numbers.Add(int.Parse(linePart[numberRange]));
                    
                    buttons.Add([..numbers]);
                }
                else if (discriminator is '{')
                {
                    var numbers = new List<int>();
                    var numberRanges = linePart.Split(',');
                    
                    foreach (var numberRange in numberRanges)
                        numbers.Add(int.Parse(linePart[numberRange]));
                    
                    joltage = [..numbers];
                }
            }
            machines.Add(new(lights, [..buttons], joltage));
        }

        return machines;
    }

    public override string FirstPart()
    {
        var machines = ParsedInput.Value;
        var steps = 0L;
        
        foreach (var machine in machines)
        {
            var (targetLights, buttons, _) = machine;
            var priorityQueue = new PriorityQueue<ImmutableArray<bool>, int>();
            priorityQueue.Enqueue([..Enumerable.Repeat(false, machine.Buttons.Length)], 0);
            while (priorityQueue.Count > 0)
            {
                var success = priorityQueue.TryDequeue(out var current, out var priority);
                if (!success) 
                    continue;
                var lights = ImmutableArray.CreateRange(Enumerable.Repeat(false, targetLights.Length));

                for (var i = 0; i < current.Length; ++i)
                {
                    if (!current[i])
                        continue;
                    
                    var currentButton = buttons[i];

                    foreach (var j in currentButton)
                        lights = lights.SetItem(j, !lights[j]);
                }

                if (targetLights.SequenceEqual(lights))
                {
                    steps += priority;
                    break;
                }
                
                for (var i = 0; i < current.Length; ++i)
                {
                    if (current[i])
                        continue;

                    var next = current.SetItem(i, true);
                    priorityQueue.Enqueue(next, priority + 1);
                }
            }
        }

        return steps.ToString();
    }

    public override string SecondPart()
    {
        var machines = ParsedInput.Value;
        var steps = 0L;

        foreach (var machine in machines)
        {
            var (_, buttons, targetJoltages) = machine;
            
            var columnCount = buttons.Length;
            using var lp = LpSolve.make_lp(targetJoltages.Length, columnCount);

            const int ignored = 0;

            lp.set_minim();
            lp.set_obj_fn(Enumerable.Repeat(1, columnCount)
                .Prepend(ignored)
                .Select(x => (double)x)
                .ToArray());

            for (var i = 0; i < columnCount; ++i)
            {
                lp.is_int(i + 1);
                lp.set_int(i + 1, true);
            }
            
            lp.set_add_rowmode(true);
            for (var j = 0; j < targetJoltages.Length; j++)
            {
                var array = Enumerable.Range(0, columnCount).Select(k => buttons[k].Contains(j) ? 1 : 0)
                    .Prepend(ignored)
                    .Select(x => (double)x)
                    .ToArray();
                lp.add_constraint(array, lpsolve_constr_types.EQ, targetJoltages[j]);
            }
            lp.set_add_rowmode(false);

            lp.set_verbose(lpsolve_verbosity.IMPORTANT);

            var s = lp.solve();
            if (s != lpsolve_return.OPTIMAL) 
                continue;

            var results = new double[columnCount];
            lp.get_variables(results);
            var result = Math.Round(results.Sum());
            steps += (long)result;
        }

        return steps.ToString();
    }
}