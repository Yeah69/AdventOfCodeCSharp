namespace AdventOfCode.Days;

internal interface IDay
{
    int Number { get; }
    
    string Input { get; }
    
    string FirstPart(); 
    string SecondPart();
}