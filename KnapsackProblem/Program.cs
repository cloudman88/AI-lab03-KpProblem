using System;
using KnapsackProblem.GeneticsAlgorithms;
using KnapsackProblem.GeneticsSol;

namespace KnapsackProblem
{
    class Program
    {
        static void Main(string[] args)
        {
            //Manager man = new Manager();
            do
            {
                KsProblemGenetics ksp = new KsProblemGenetics(CrossoverMethod.Uniform, SelectionMethod.Truncation);
                ksp.init_population();
                ksp.run_algorithm();

                //KsProblemHeuristic ksh = new KsProblemHeuristic(SearchAlgorithm.BestFirstSearch, NeglectedConstrain.Integrality);
                //ksh.run_algorithm();

                //KsProblemDynamic kspd = new KsProblemDynamic();
                //kspd.Init();
                //kspd.run_algorithm();

                //man.Run();
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
        }
    }
}