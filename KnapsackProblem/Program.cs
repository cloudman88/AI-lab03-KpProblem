using System;
using KnapsackProblem.GeneticsAlgorithms;
using KnapsackProblem.GeneticsSol;
using KnapsackProblem.HeuristicSol;

namespace KnapsackProblem
{
    class Program
    {
        static void Main(string[] args)
        {
            //Manager man = new Manager();
            do
            {
                //KsProblem ksp = new KsProblem(CrossoverMethod.Uniform, SelectionMethod.Truncation);
                //ksp.init_population();
                //ksp.run_algorithm();

                KnapsackHeuristic ksh = new KnapsackHeuristic(SearchAlgorithm.BestFirstSearch, NeglectedConstrain.Capacity);
                ksh.run_algorithm();

                //man.Run();
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
        }
    }
}
