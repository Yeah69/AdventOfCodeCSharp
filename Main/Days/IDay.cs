namespace AdventOfCode.Days;

internal interface IDay
{
    int Number { get; }
    
    int? SampleNumber { get; }
    
    string Input { get; }
    
    string FirstPart(); 
    string SecondPart();
    IEnumerable<IDay> Samples();
}