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
            string filePath = "pet3.DAT";
            ReadDataFromFile(filePath);
            _tables = new ObservableCollection<int[,]>();
        }
        public void Init()
        {
            for (int i = 0; i < NumOfknapsacks; i++)
            {
                int[,] table = new int[Capcities[i]+1,NumOfItems+1];
                _tables.Add(table);
            }
            BuildItemsList(false);
        }
        public void run_algorithm()
        {           
            for (int j = 1; j <= NumOfItems; j++)
            {
                for (int k = 1; k <= Capcities.Min(); k++)
                {
                    bool result = CanAddItemToAllSacks(k,j);
                    if (result == true)
                    {
                        for (int i = 0; i < _tables.Count; i++)
                        {
                            var table = _tables[i];                           
                            table[k, j] = (int)Math.Max(table[k, j - 1], Items[j - 1].Weight + table[k - Items[j - 1].Constrains[i], j - 1]);                        
                        }
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
            print_result_details();
        }     
        private bool CanAddItemToAllSacks(int k, int j)
        {
            for (int i = 0; i < NumOfknapsacks; i++)
            {
                if (Items[j - 1].Constrains[i] > k)
                    return false;
            }
            return true;
        }
        private void print_result_details()
        {
            Console.WriteLine("Total weights: ");
            foreach (var table in _tables)
            {
                int max = table.Cast<int>().Max();
                Console.WriteLine(max);                
            }

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
