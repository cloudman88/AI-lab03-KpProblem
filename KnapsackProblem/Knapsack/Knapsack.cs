using System.Collections.Generic;

namespace KnapsackProblem.Knapsack
{
    class Knapsack
    {
        public int Id;
        public int Capacity;
        public List<Item> Items;

        public Knapsack(int id,int capcity) //, int[] constr,int n)
        {
            Id = id;
            Capacity = capcity;
            //Constrains = new int[n];
            //Array.Copy(constr,Constrains,n);
            Items = new List<Item>();
        }

        public uint GetTotalWeights()
        {
            uint sumWeights = 0;
            foreach (var item in Items)
            {
                sumWeights += item.Weight;
            }
            return sumWeights;
        }

        public int GetTotalValues()
        {
            int sumValues = 0;
            foreach (var item in Items)
            {
                sumValues += item.Constrains[Id];
            }
            return sumValues;
        }
    }
}