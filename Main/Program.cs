// See https://aka.ms/new-console-template for more information

using AdventOfCode;
using AdventOfCode.Days;

var container = Container.DIE_CreateContainer();
var program = container.Create();
program.Run();

internal interface IProgram
{
    void Run();
}

internal class Prog : IProgram
{
    private readonly IDayRunner _dayRunner;

    internal Prog(IDayRunner dayRunner)
    {
        _dayRunner = dayRunner;
    }
    
    public void Run()
    {
        _dayRunner.Run();
        Console.WriteLine("Hello, World!");
    }
}