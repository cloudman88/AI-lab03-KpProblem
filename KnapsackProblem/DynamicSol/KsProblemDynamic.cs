using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
            string filePath = "pet2.DAT";
            ReadDataFromFile(filePath);
            _tables = new ObservableCollection<int[,]>();
        }

        public void Init()
        {
            for (int i = 0; i < NumOfknapsacks; i++)
            {
                int[,] table = new int[Capcities[i] + 1, NumOfItems + 1];
                _tables.Add(table);
            }
            BuildItemsList(false);
        }

        public void run_algorithm()
        {
            for (int j = 1; j <= NumOfItems; j++)
            {
                for (int k = 0; k <= Capcities.Min(); k++)
                {
                    bool result = CanAddItemToAllSacks(k, j);
                    if (result == true)
                    {
                        for (int i = 0; i < _tables.Count; i++)
                        {
                            var table = _tables[i];
                            var previousResult = table[k, j - 1];
                            var plusItem = Items[j - 1].Weight + table[k - Items[j - 1].Constrains[i], j - 1];
                            table[k, j] =(int)Math.Max(previousResult,plusItem);
                        }
                        int temp = _tables.First().Cast<int>().Max();
                        foreach (var table in _tables)
                        {
                            var max = table.Cast<int>().Max();
                            if (temp != max)
                            {
                                var x = 0;
                            }
                            Console.WriteLine(max);                            
                        }
                        Console.WriteLine();
                    }
                    else
                    {
                        foreach (var table in _tables)
                        {
                            table[k, j] = table[k, j - 1];
                        }
                    }
                }
            }
            var index = 1;
            foreach (var t in _tables)
            {
                Console.WriteLine(index++);
                for (int j = 0; j <= Capcities.Min(); j++)
                {
                    for (int i = 0; i <= NumOfItems; i++)
                    {
                        Console.Write(t[j, i] + " ");
                    }
                    Console.WriteLine(" ");
                }
            }

            print_result_details();
        }

        private bool CanAddItemToAllSacks(int k, int j)
        {
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
            Console.WriteLine("Total weights: ");
            foreach (var table in _tables)
            {
                int max = table.Cast<int>().Max();
                Console.WriteLine(max);
            }
            //print_chosen_items();
        }

        private void print_chosen_items()
        {
            string res = "";
            foreach (var table in _tables)
            {
                //find the 
                int j = NumOfItems ;
                int i = Capcities.Min();
                while (i > 0 && j >0)
                {
                    if (table[i, j] != 0)
                    {
                        int temp = table[i, j];
                        while (j > 0 && temp == table[i, j-1])
                        {
                            res += "0 ";
                            j--;
                        }
                        res += "1 ";
                        while (i > 0 && temp == table[i-1, j])
                        {
                            i--;
                        }
                    }
                    j--;
                }
            }
           Console.WriteLine(res);
        }
    }
}

//private void BuildItemsList()
//{
//    Items.Clear();
//    for (int i = 0; i < NumOfItems; i++)
//    {
//        Item item = new Item()
//        {
//            Constrains = new short[NumOfknapsacks],
//            Densities = null,
//            DensitiesAvg = 0,
//            Weight = Weights[i]
//        };
//        Items.Add(item);
//    }
//}
