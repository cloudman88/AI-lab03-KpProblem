using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using KnapsackProblem.Tools;

namespace KnapsackProblem.DynamicSol
{
    class KsProblemDynamic : KsProblem
    {
        private readonly ObservableCollection<int[,]> _tables;

        public KsProblemDynamic()
        {
            string filePath = "pet3.DAT";
            ReadDataFromFile(filePath);
            _tables = new ObservableCollection<int[,]>();
        }

        public void Init()
        {
            for (int i = 0; i < NumOfknapsacks; i++)
            {
                int[,] table = new int[Capacities[i] + 1, NumOfItems + 1];
                _tables.Add(table);
            }
            BuildItemsList(false);
        }     

        public void run_algorithm()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            var max = Capacities.Min();
            int indexMax = -1;
            for (int i = 0; i < Capacities.Count; i++)
            {
                if (max <= Capacities[i])
                {
                    max = Capacities[i];
                    indexMax = i;
                }
            }

            for (int j = 1; j <= NumOfItems; j++)
            {
                for (int k = 0; k <= Capacities[indexMax]; k++)
                {
                    if (j==4 && k >280)
                    {
                        var x = 0;
                    }
                    var table = _tables[indexMax];
                    bool result = CanAddItemToAllSacks(k, j);
                    if (result == true)
                    {
                        var previousResult = table[k, j - 1];
                        var plusItem = Items[j - 1].Weight + table[k - Items[j - 1].Constrains[indexMax], j - 1];
                        table[k, j] = (int)Math.Max(previousResult, plusItem);
                    }
                    else
                    {
                        table[k, j] = table[k, j - 1];
                    }
                }
            }
            stopWatch.Stop();
            double totalTicks = (stopWatch.ElapsedTicks / (double)Stopwatch.Frequency) * 1000;
            for (int j = 0; j <= Capacities[indexMax]; j++)
            {
                for (int i = 0; i <= NumOfItems; i++)
                {
                    Console.Write(_tables[indexMax][j, i] + " ");
                }
                Console.WriteLine(" ");
            }
            print_result_details();
            print_chosen_items(_tables[indexMax],indexMax);
            Console.WriteLine("Total Ticks " + (long)totalTicks);
        }

        private bool CanAddItemToAllSacks(int k, int j)
        {
            if (k > Capacities.Min()) return false;
            for (int i = 0; i < NumOfknapsacks; i++)
            {
                if (Items[j-1].Constrains[i] > k)
                    return false;
            }
            return true;
        }

        private void print_result_details()
        {
            Console.WriteLine("Optimal: " + Opt);
            //Console.WriteLine("Total weights: ");
            //foreach (var table in _tables)
            //{
            //    int max = table.Cast<int>().Max();
            //    Console.WriteLine(max);
            //}
            //print_chosen_items();
        }

        private void print_chosen_items(int[,] table,int chosenIndex)
        {
            string res = "";
            var row = Capacities[chosenIndex];
            var col = Items.Count;
            while (col > 0)
            {
                var a = table[row, col];
                var x = Items[col - 1].Constrains[chosenIndex];
                if (row - x >= 0)
                {
                    var b = table[row - x, col - 1];
                    var d = Items[col - 1].Weight;
                    if (a - b == d)
                    {
                        //the element 'i' is in the knapsack
                        res += "1 ";
                        col--;
                        if (col != 0) row = (short) (row - Items[col].Constrains[chosenIndex]);
                    }
                    else
                    {
                        res += "0 ";
                        col--;
                    }
                }
                else
                {
                    res += "0 ";
                    col--;
                }
            }
            string str = res.Substring(0, res.Length - 1);
            Console.WriteLine(str.Reverse().ToArray());
        }
    }
}