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
            
            // We build a model with 3 constraints and 2 variables
            const int Ncol = 2;
            using var lp = LpSolve.make_lp(3, Ncol);

// NOTE: set_obj_fnex/add_constraintex should be preferred on set_obj_fn/add_constraint
//       as they can specify only non-zero elements when working with big model.
//       The methods without _ex_ suffix will ignore the first array element so
//       let's use a constant for this for clarity.
            const double Ignored = 0;

// set the objective function: maximize (143 x + 60 y)
            lp.set_maxim();
            lp.set_obj_fn([Ignored, 143, 60]);

// add constraints to the model
//   120 x + 210 y <= 15000
//   110 x +  30 y <= 4000
//       x +     y <= 75
            lp.set_add_rowmode(true);
            lp.add_constraint([Ignored, 120, 210], lpsolve_constr_types.LE, 15000);
            lp.add_constraint([Ignored, 110, 30], lpsolve_constr_types.LE, 4000);
            lp.add_constraint([Ignored, 1, 1], lpsolve_constr_types.LE, 75);
            lp.set_add_rowmode(false);

// We only want to see important messages on screen while solving
            lp.set_verbose(lpsolve_verbosity.IMPORTANT);

// Now let lp_solve calculate a solution
            lpsolve_return s = lp.solve();
            if (s == lpsolve_return.OPTIMAL)
            {
                Console.WriteLine("Objective value: " + lp.get_objective());

                var results = new double[Ncol];
                lp.get_variables(results);
                for (int j = 0; j < Ncol; j++)
                {
                    Console.WriteLine(lp.get_col_name(j + 1) + ": " + results[j]);
                }
            }
        }

        return Consts.NoSolutionFound;
    }
}