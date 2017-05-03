using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace KnapsackProblem.Tools
{
    abstract class KsProblem
    {
        protected uint Opt;
        protected int NumOfknapsacks;
        protected int NumOfItems;
        protected readonly List<short> Capcities;
        protected readonly List<uint> Weights;
        protected readonly ObservableCollection<short[]> Constrains;
        protected readonly List<Item> Items;

        protected KsProblem()
        {
            Items = new List<Item>();
            Weights = new List<uint>();
            Capcities = new List<short>();
            Constrains = new ObservableCollection<short[]>();
        }
        
        public static void ReadDataFromFile(string filePath, ref int numOfknapsacks, ref int numOfItems, List<uint> weights,
            List<short> capcities, ObservableCollection<short[]> constrains, ref uint opt)
        {

            using (StreamReader sr = new StreamReader(filePath))
            {
                string line;

                //read num of knapsacks, num of items
                if ((line = sr.ReadLine()) != null)
                {
                    string[] data = line.Split(new string[] { "\t", " " }, StringSplitOptions.RemoveEmptyEntries);
                    int[] numbers = Array.ConvertAll(data, int.Parse);
                    numOfknapsacks = numbers[0];
                    numOfItems = numbers[1];
                }

                //read the items weights
                int count = 0;
                weights.Clear();
                while (count < numOfItems && (line = sr.ReadLine()) != null)
                {
                    string[] data = line.Split(new string[] { "\t", " " }, StringSplitOptions.RemoveEmptyEntries);
                    uint[] numbers = Array.ConvertAll(data, uint.Parse);
                    weights.AddRange(numbers);
                    count += numbers.Length;
                }

                //read capacities
                count = 0;
                capcities.Clear();
                while (count < numOfknapsacks && (line = sr.ReadLine()) != null)
                {
                    string[] data = line.Split(new string[] { "\t", " " }, StringSplitOptions.RemoveEmptyEntries);
                    short[] numbers = Array.ConvertAll(data, short.Parse);
                    capcities.AddRange(numbers);
                    count += numbers.Length;
                }

                //read constrains
                constrains.Clear();
                for (int i = 0; i < numOfknapsacks; i++)
                {
                    count = 0;
                    List<short> constrain = new List<short>();
                    while (count < numOfItems && (line = sr.ReadLine()) != null)
                    {
                        string[] data = line.Split(new string[] { "\t", " " }, StringSplitOptions.RemoveEmptyEntries);
                        short[] numbers = Array.ConvertAll(data, short.Parse);
                        //Array.Copy(numbers, 0, constrain, numbers.Length, count);
                        constrain.AddRange(numbers);
                        count += numbers.Length;
                    }
                    constrains.Add(constrain.ToArray());
                }

                //read optimal solution  
                sr.ReadLine();
                while ((line = sr.ReadLine()) != null)
                {
                    if (!line.Equals(""))
                    {
                        opt = UInt32.Parse(line);
                        break;
                    }
                }
            }
        }

        protected void ReadDataFromFile(string filePath)
        {

            using (StreamReader sr = new StreamReader(filePath))
            {
                string line;

                //read num of knapsacks, num of items
                if ((line = sr.ReadLine()) != null)
                {
                    string[] data = line.Split(new string[] { "\t", " " }, StringSplitOptions.RemoveEmptyEntries);
                    int[] numbers = Array.ConvertAll(data, int.Parse);
                    NumOfknapsacks = numbers[0];
                    NumOfItems = numbers[1];
                }

                //read the items weights
                int count = 0;
                Weights.Clear();
                while (count < NumOfItems && (line = sr.ReadLine()) != null)
                {
                    string[] data = line.Split(new string[] { "\t", " " }, StringSplitOptions.RemoveEmptyEntries);
                    uint[] numbers = Array.ConvertAll(data, uint.Parse);
                    Weights.AddRange(numbers);
                    count += numbers.Length;
                }

                //read capacities
                count = 0;
                Capcities.Clear();
                while (count < NumOfknapsacks && (line = sr.ReadLine()) != null)
                {
                    string[] data = line.Split(new string[] { "\t", " " }, StringSplitOptions.RemoveEmptyEntries);
                    short[] numbers = Array.ConvertAll(data, short.Parse);
                    Capcities.AddRange(numbers);
                    count += numbers.Length;
                }

                //read constrains
                Constrains.Clear();
                for (int i = 0; i < NumOfknapsacks; i++)
                {
                    count = 0;
                    List<short> constrain = new List<short>();
                    while (count < NumOfItems && (line = sr.ReadLine()) != null)
                    {
                        string[] data = line.Split(new string[] { "\t", " " }, StringSplitOptions.RemoveEmptyEntries);
                        short[] numbers = Array.ConvertAll(data, short.Parse);
                        //Array.Copy(numbers, 0, constrain, numbers.Length, count);
                        constrain.AddRange(numbers);
                        count += numbers.Length;
                    }
                    Constrains.Add(constrain.ToArray());
                }

                //read optimal solution  
                sr.ReadLine();
                while ((line = sr.ReadLine()) != null)
                {
                    if (!line.Equals(""))
                    {
                        Opt = UInt32.Parse(line);
                        break;
                    }
                }
            }
        }

        protected void BuildItemsList(bool calcDensity)
        {
            Items.Clear();
            for (int i = 0; i < NumOfItems; i++)
            {
                Item item = new Item()
                {
                    Constrains = new short[NumOfknapsacks],
                    Densities = new float[NumOfknapsacks],
                    DensitiesAvg = 0,
                    Weight = Weights[i]
                };
                if (calcDensity == true)
                {
                    item.Densities = new float[NumOfknapsacks];
                    for (int j = 0; j < NumOfknapsacks; j++)
                    {
                        item.Constrains[j] = Constrains[j][i];
                        if (Constrains[j][i] != 0)
                            item.Densities[j] += (float)Weights[i] / Constrains[j][i];
                        else
                            item.Densities[j] += float.MaxValue; //if constrain is zero, then most weight per constrain is optimal
                    }
                    item.DensitiesAvg = item.Densities.Average();
                }
                else
                {
                    item.Densities = null;
                }
                Items.Add(item);
            }
        }
    }
}