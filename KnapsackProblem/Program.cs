using KnapsackProblem.GeneticsAlgorithms;

namespace KnapsackProblem
{
    class Program
    {
        static void Main(string[] args)
        {
            Knapsack.KsProblem ksp = new Knapsack.KsProblem(CrossoverMethod.Cx,SelectionMethod.Truncation);
            ksp.init_population();

            //Manager man = new Manager();
            //do
            //{
            //    man.Run();
            //} while (Console.ReadKey(true).Key != ConsoleKey.Escape);
        }
    }
}
