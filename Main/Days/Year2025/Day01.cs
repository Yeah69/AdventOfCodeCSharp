namespace AdventOfCode.Days.Year2025;

internal class Day01 : DayOfYear2025<Day01, string>
{
    public override int Number => 1;

    protected override string ParseInput()
    {
        var inputSpan = Input.AsSpan();
        
        return Input;
    }
    
    public override string FirstPart() => 
        Consts.NoSolutionFound;

    public override string SecondPart() => 
        Consts.NoSolutionFound;
}