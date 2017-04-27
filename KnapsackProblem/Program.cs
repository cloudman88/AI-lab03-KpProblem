using System;
using KnapsackProblem.GeneticsAlgorithms;

namespace KnapsackProblem
{
    class Program
    {
        static void Main(string[] args)
        {
            //Manager man = new Manager();
            do
            {
                Knapsack.KsProblem ksp = new Knapsack.KsProblem(CrossoverMethod.Uniform,SelectionMethod.Truncation);
                ksp.init_population();
                ksp.run_algorithm();
                //man.Run();
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
        }
    }
}
