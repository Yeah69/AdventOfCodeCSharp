﻿namespace AdventOfCode.Days;

internal interface IDay
{
    int Number { get; }
    
    int? SampleNumber { get; }
    
    string Input { get; }

    void PrepareInputParsing();
    string FirstPart(); 
    string SecondPart();
    IEnumerable<IDay> Samples();
    void PrintLongDayLabel(Stage? stage = null);
    void PrintShortDayLabel(Stage stage);
    string GetShortDayLabelText(Stage stage);
}