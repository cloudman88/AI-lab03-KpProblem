using System;
using KnapsackProblem.DynamicSol;
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
                try
                {
                    KsProblemGenetics ksp = new KsProblemGenetics(CrossoverMethod.Uniform, SelectionMethod.Truncation);
                    ksp.run_algorithm();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                try
                {
                    KsProblemHeuristic ksh1 = new KsProblemHeuristic(SearchAlgorithm.DepthFirstSearch, NeglectedConstrain.Capacity);
                    ksh1.run_algorithm("output_heuristics_dfs_cap.txt");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                try
                {
                    KsProblemHeuristic ksh2 = new KsProblemHeuristic(SearchAlgorithm.BestFirstSearch, NeglectedConstrain.Capacity);
                    ksh2.run_algorithm("output_heuristics_bfs_cap.txt");

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                try
                {
                    KsProblemHeuristic ksh3 = new KsProblemHeuristic(SearchAlgorithm.DepthFirstSearch, NeglectedConstrain.Integrality);
                    ksh3.run_algorithm("output_heuristics_dfs_int.txt");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                try
                {
                    KsProblemHeuristic ksh4 = new KsProblemHeuristic(SearchAlgorithm.BestFirstSearch, NeglectedConstrain.Integrality);
                    ksh4.run_algorithm("output_heuristics_bfs_int.txt");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                //KsProblemDynamic kspd = new KsProblemDynamic();
                //kspd.Init();
                //kspd.run_algorithm();

                //man.Run();
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
        }
    }
}