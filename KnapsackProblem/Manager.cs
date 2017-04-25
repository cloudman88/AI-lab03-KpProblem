using System;
using System.Collections.Generic;
using System.Linq;
using KnapsackProblem.GeneticsAlgorithms;

namespace Genetics
{
    class Manager
    {
        private CrossoverMethod _crossoverMethod;
        private MutationOperator _mutationOperator;
        private int _n;
        private SelectionMethod _selectionMethod;

        public Manager()
        {
        }

        public void Run()
        {
            print_options();
            int input = get_input();
            switch (input)
            {
                case 1:
                    
                    break;
                    
                default:
                    Console.WriteLine("please enter a number between 1 to 5");
                    break;
            }
        }

        private void print_options()
        {
            Console.WriteLine("Please choose algorithm by it's number: ");
            Console.WriteLine("1.String search");
            Console.WriteLine("2.N-Queens using Genetics Algorithms");
            Console.WriteLine("3.N-Queens using Minimal Conflits");
            Console.WriteLine("4.Bin Packing using Genetics Algorithms");
            Console.WriteLine("5.Bin Packing - First Fit");
            Console.WriteLine("6.Baldwin - Not finished");
        }

        private int get_input()
        {
            bool validInput = true;
            int input = 0;
            do
            {
                try
                {
                    input = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("");
                }
                catch (Exception)
                {
                    validInput = false;
                    Console.WriteLine("please enter a number");
                }

            } while (!validInput);
            return input;
        }
        private void choose_crossover_method(bool isOrdered = true)
        {
            Console.WriteLine("Please Choose CrossOver Method :");
            var methodsList = Enum.GetValues(typeof(CrossoverMethod)).Cast<CrossoverMethod>().ToList();
            if (isOrdered == true)
            {
                for (int i = 0; i < 3; i++)
                {
                    methodsList.RemoveAt(0);
                }
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    methodsList.Remove(methodsList.Last());
                }
            }
            for (int i = 0; i < methodsList.Count; i++)
            {
                Console.WriteLine((i + 1) + ". " + methodsList[i]);
            }
            int input = 0;
            do
            {
                input = get_input();

            } while (input <= 0 || input > methodsList.Count);
            _crossoverMethod = methodsList[input - 1];
        }
        private void choose_mutations_operator()
        {
            Console.WriteLine("Please Choose Mutation Operator :");
            var mutationList = Enum.GetValues(typeof(MutationOperator)).Cast<MutationOperator>().ToList();
            for (int i = 0; i < mutationList.Count; i++)
            {
                Console.WriteLine((i + 1) + ". " + mutationList[i]);
            }
            int input = 0;
            do
            {
                input = get_input();

            } while (input <= 0 || input > mutationList.Count);
            _mutationOperator = mutationList[input];
        }
        private void set_selection_method()
        {
            Console.WriteLine("Please set if Selection method: ");
            var selectionList = Enum.GetValues(typeof(SelectionMethod)).Cast<SelectionMethod>().ToList();
            for (int i = 0; i < selectionList.Count; i++)
            {
                var index = i + 1;
                Console.WriteLine(index + ". " + selectionList[i]);
            }
            int input = 0;
            do
            {
                input = get_input();

            } while (input <= 0 || input > selectionList.Count);
            _selectionMethod = selectionList[input];
        }
     

    }
}
