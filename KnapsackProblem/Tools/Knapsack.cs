using System.Collections.Generic;
using System.ComponentModel;

namespace KnapsackProblem.Tools
{
    class Knapsack
    {
        public int Id;
        public int Capacity;
        public List<Item> PackedItems;
        public int Value;
        public uint Weight;

        public Knapsack(int id,int capcity) 
        {
            Id = id;
            Capacity = capcity;
            Value = 0;
            Weight = 0;
            PackedItems = new List<Item>();
        }        
    }

    public struct Item
    {
        public int Id;
        public uint Weight;
        public short[] Constrains;
        public float[] Densities; //used for neglecting integrality constraint
        public float DensitiesAvg;
    }
}