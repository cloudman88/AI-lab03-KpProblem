using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using KnapsackProblem.GeneticsAlgorithms;
using KnapsackProblem.Tools;

namespace KnapsackProblem.GeneticsSol
{
    class KnapsackGen : Gen
    {
        public int NumOfKnapsacks;
        public List<Knapsack> Knapsacks;
        public List<Item> Items;
        public int[] ChosenItems;

        public KnapsackGen(int numOfks, short[] capacities, int numOfItems, uint[] weights ,ObservableCollection<short[]> constrains )
        {
            NumOfKnapsacks = numOfks;
            Knapsacks = new List<Knapsack>();
            Items = new List<Item>();
            ChosenItems = new int[numOfItems];            
            for (int i = 1; i <= NumOfKnapsacks; i++)
            {
                Knapsacks.Add(new Knapsack(i, capacities[i-1]));
            }
            for (int i = 0; i < numOfItems; i++)
            {
                Item item = new Item()
                {
                    Weight = weights[i],
                    Constrains = new short[NumOfKnapsacks],
                };
                for (int j = 0; j < NumOfKnapsacks; j++)
                {
                    item.Constrains[j] = constrains[j][i];
                }
                Items.Add(item);
            }
            PackItemsRandomlly();
        }

        public void PackItemsRandomlly()
        {
            bool result;
            Random rand = new Random();
            var numOfItems = Items.Count;
            List<int> itemsId = Enumerable.Range(0, numOfItems).ToList();
            do
            {
                var chosenItemIndex = rand.Next() % itemsId.Count;
                result = AddToKnapsacks(Items[chosenItemIndex],chosenItemIndex);

                itemsId.Remove(chosenItemIndex);
            } while (result == true && itemsId.Count > 0);
        }

        private bool AddToKnapsacks(Item item, int index)
        {
            //check if adding the item wont exceed capcity of one of the knapsacks
            foreach (var ks in Knapsacks)
            {
                if (ks.Value + item.Constrains[ks.Id-1] > ks.Capacity) return false;                
            }
            // add item to all Knapsacks
            foreach (var ks in Knapsacks)
            {
                ks.PackedItems.Add(item);
                ks.Value += item.Constrains[ks.Id-1];
                ks.Weight += item.Weight;
                ChosenItems[index] = 1;
            }
            return true;
        }
    }
}

