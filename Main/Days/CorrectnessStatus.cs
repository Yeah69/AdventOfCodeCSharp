namespace AdventOfCode.Days;

internal enum CorrectnessStatus
{
    Incorrect,
    Uncertain,
    Correct
}

internal static class CorrectnessStatusExtensions
{
    internal static char ToChar(this CorrectnessStatus correctnessStatus) => correctnessStatus switch
    {
        CorrectnessStatus.Incorrect => 'X',
        CorrectnessStatus.Uncertain => '?',
        CorrectnessStatus.Correct => '\u2713',
        _ => '_'
    };

    internal static ConsoleColor ToConsoleColor(this CorrectnessStatus correctnessStatus) => correctnessStatus switch
    {
        CorrectnessStatus.Incorrect => ConsoleColor.DarkRed,
        CorrectnessStatus.Uncertain => ConsoleColor.DarkYellow,
        CorrectnessStatus.Correct => ConsoleColor.DarkGreen,
        _ => ConsoleColor.DarkGray,
    };
}

internal static class CharExtensions
{
    internal static CorrectnessStatus ToCorrectnessStatus(this char character) => character switch
    {
        'X' => CorrectnessStatus.Incorrect,
        '?' => CorrectnessStatus.Uncertain,
        '\u2713' => CorrectnessStatus.Correct,
        _ => (CorrectnessStatus)99
    };
}