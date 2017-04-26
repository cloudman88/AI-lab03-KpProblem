using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using KnapsackProblem.GeneticsAlgorithms;

namespace KnapsackProblem.Knapsack
{
    class KsProblem : GeneticsAlgorithms<KnapsackGen>
    {
        private int _opt;
        private int _numOfknapsacks;
        private int _numOfItems;
        private List<int> _capcities;
        private List<uint> _weights;
        private ObservableCollection<int[]> _constrains;

        public KsProblem(CrossoverMethod crossMethod, SelectionMethod selectionMethod) : base(crossMethod, selectionMethod)
        {
            _capcities = new List<int>();
            _weights = new List<uint>();
            _constrains = new ObservableCollection<int[]>();
            ReadDataFromFile();
        }

        private void ReadDataFromFile()
        {
            string file = "HP1.DAT";// 
            using (StreamReader sr = new StreamReader(file))
            {
                string line;

                //read num of knapsacks, num of items
                if ((line = sr.ReadLine()) != null)
                {
                    string[] data = line.Split(new string[] { "\t", " " }, StringSplitOptions.RemoveEmptyEntries);
                    int[] numbers = Array.ConvertAll(data, int.Parse);
                    _numOfknapsacks = numbers[0];
                    _numOfItems = numbers[1];
                }

                //read the items weights
                int count = 0;
                while (count < _numOfItems && (line = sr.ReadLine()) != null)
                {                    
                    string[] data = line.Split(new string[] { "\t", " " }, StringSplitOptions.RemoveEmptyEntries);
                    uint[] numbers = Array.ConvertAll(data, uint.Parse);
                    _weights.AddRange(numbers);
                    count += numbers.Length;
                }

                //read capacities
                count = 0;
                while (count < _numOfknapsacks && (line = sr.ReadLine()) != null)
                {
                    string[] data = line.Split(new string[] { "\t", " " }, StringSplitOptions.RemoveEmptyEntries);
                    int[] numbers = Array.ConvertAll(data, int.Parse);
                    _capcities.AddRange(numbers);
                    count += numbers.Length;
                }

                //read constrains
                for (int i = 0; i < _numOfknapsacks; i++)
                {
                    count = 0;
                    List<int> constrain = new List<int>();
                    while (count < _numOfItems && (line = sr.ReadLine()) != null)
                    {
                        string[] data = line.Split(new string[] { "\t", " " }, StringSplitOptions.RemoveEmptyEntries);
                        int[] numbers = Array.ConvertAll(data, int.Parse);
                        //Array.Copy(numbers, 0, constrain, numbers.Length, count);
                        constrain.AddRange(numbers);
                        count += numbers.Length;
                    }
                    _constrains.Add(constrain.ToArray());
                }

                //read optimal solution  
                sr.ReadLine();              
                if ((line = sr.ReadLine()) != null)
                {
                    _opt = Int32.Parse(line);
                }
            }
        }

        public override void init_population()
        {
            Population = new List<KnapsackGen>();
            Buffer = new List<KnapsackGen>();
            for (int i = 0; i < GaPopSize; i++)
            {
                KnapsackGen ksGen = new KnapsackGen(_numOfknapsacks,_capcities.ToArray(),_numOfItems,_weights.ToArray(),_constrains);
                Population.Add(ksGen);
                Buffer.Add(ksGen);
            }
        }

        protected override void calc_fitness()
        {
            foreach (var knapsackGen in Population)
            {
                foreach (var sack in knapsackGen.Knapsacks)
                {
                    knapsackGen.Fitness += sack.GetTotalWeights();
                }
            }
        }

        protected override void Mutate(KnapsackGen member)
        {
            throw new NotImplementedException();
        }

        protected override void mate_by_method(KnapsackGen bufGen, KnapsackGen gen1, KnapsackGen gen2)
        {
            throw new NotImplementedException();
        }

        protected override Tuple<string, uint> get_best_gen_details(KnapsackGen gen)
        {
            throw new NotImplementedException();
        }

        protected override KnapsackGen get_new_gen()
        {
            throw new NotImplementedException();
        }

        protected override int calc_distance(KnapsackGen gen1, KnapsackGen gen2)
        {
            throw new NotImplementedException();
        }
    }
}
