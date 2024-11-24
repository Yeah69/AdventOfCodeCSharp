using AdventOfCode.Days;
using AdventOfCode.Days.Pickers;
using MrMeeseeks.DIE.Configuration.Attributes;

namespace AdventOfCode;

[ImplementationChoice(typeof(IDayPicker), typeof(PickAll))]
[DecoratorSequenceChoice(typeof(IDay), typeof(IDay), typeof(DayDecoratorTrackTime), typeof(DayDecoratorPrintSolution))]
[CreateFunction(typeof(IProgram), "Create")]
internal sealed partial class Container;