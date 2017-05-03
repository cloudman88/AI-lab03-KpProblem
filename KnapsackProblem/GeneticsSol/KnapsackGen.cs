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
        public int[] ChosenItems;

        public KnapsackGen(int numOfks, short[] capacities, int numOfItems,List<Item> items  )
        {
            NumOfKnapsacks = numOfks;
            Knapsacks = new List<Knapsack>();
            ChosenItems = new int[numOfItems];            
            for (int i = 1; i <= NumOfKnapsacks; i++)
            {
                Knapsacks.Add(new Knapsack(i, capacities[i-1]));
            }           
            PackItemsRandomlly(items);
        }

        public void PackItemsRandomlly(List<Item> items)
        {
            bool result;
            Random rand = new Random();
            List<int> itemsId = Enumerable.Range(0, items.Count).ToList();
            do
            {
                var chosenItemIndex = rand.Next() % itemsId.Count;
                //try to add item to the knapsack
                result = AddItemToKnapsacks(items[chosenItemIndex],chosenItemIndex); 
                itemsId.Remove(chosenItemIndex);
            } while (result == true && itemsId.Count > 0);
        }

        private bool AddItemToKnapsacks(Item item, int index)
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

