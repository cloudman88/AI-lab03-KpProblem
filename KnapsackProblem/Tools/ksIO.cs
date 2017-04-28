using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace KnapsackProblem.Tools
{
    class ksIO
    {
        public static void ReadDataFromFile(string filePath,ref int numOfknapsacks,ref int numOfItems, List<uint> weights,
            List<short> capcities, ObservableCollection<short[]> constrains,ref uint opt)
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
                while (count < numOfItems && (line = sr.ReadLine()) != null)
                {
                    string[] data = line.Split(new string[] { "\t", " " }, StringSplitOptions.RemoveEmptyEntries);
                    uint[] numbers = Array.ConvertAll(data, uint.Parse);
                    weights.AddRange(numbers);
                    count += numbers.Length;
                }

                //read capacities
                count = 0;
                while (count < numOfknapsacks && (line = sr.ReadLine()) != null)
                {
                    string[] data = line.Split(new string[] { "\t", " " }, StringSplitOptions.RemoveEmptyEntries);
                    short[] numbers = Array.ConvertAll(data, short.Parse);
                    capcities.AddRange(numbers);
                    count += numbers.Length;
                }

                //read constrains
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
    }
}
