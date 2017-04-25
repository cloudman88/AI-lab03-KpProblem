using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using KnapsackProblem.GeneticsAlgorithms;

namespace KnapsackProblem.Knapsack
{
    class KnapsackGen : Gen
    {
        public int NumOfKnapsacks;
        public List<Knapsack> Knapsacks;
        public List<Item> Items;

        public KnapsackGen(int numOfks, int[] capacities, int numOfItems,uint[] weights ,ObservableCollection<int[]> constrains )
        {
            NumOfKnapsacks = numOfks;
            Knapsacks = new List<Knapsack>();
            Items = new List<Item>();
            for (int i = 1; i <= NumOfKnapsacks; i++)
            {
                Knapsacks.Add(new Knapsack(i, capacities[i-1]));
            }
            for (int i = 0; i < numOfItems; i++)
            {
                Item item = new Item()
                {
                    Weight = weights[i],
                    Constrains = new int[NumOfKnapsacks],
                };
                Array.Copy(constrains[i], item.Constrains, numOfItems);
            }

        }
    }
}

public struct Item
{
    public uint Weight;
    public int[] Constrains;
}