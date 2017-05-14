using System;

namespace KnapsackProblem
{
    class Program
    {
        static void Main(string[] args)
        {
            Manager man = new Manager();
            do
            {
                man.Run();
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
        }
    }
}