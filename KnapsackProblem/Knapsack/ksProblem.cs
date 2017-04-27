using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using KnapsackProblem.GeneticsAlgorithms;

namespace KnapsackProblem.Knapsack
{
    class KsProblem : GeneticsAlgorithms<KnapsackGen>
    {
        private uint _opt;
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
                    _opt = UInt32.Parse(line);
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
                int sum = 0;
                foreach (var ks in knapsackGen.Knapsacks)
                {
                    ks.PackedItems.Clear();
                    ks.Value = 0;
                    ks.Weight = 0;
                }
                for (int i = 0; i < _numOfItems; i++)
                {
                    if (knapsackGen.ChosenItems[i] == 1)
                    {
                        for (int j = 0; j < _numOfknapsacks; j++)
                        {
                            Item item = new Item()
                            {
                                Weight = _weights[i],
                                Constrains = _constrains[j]
                            };
                            knapsackGen.Knapsacks[j].PackedItems.Add(item);
                            knapsackGen.Knapsacks[j].Weight += item.Weight;
                            knapsackGen.Knapsacks[j].Value += item.Constrains[j];                            
                        }                        
                    }
                }
                if (knapsackGen.Knapsacks.Last().Weight <= _opt)
                    knapsackGen.Fitness = _opt - knapsackGen.Knapsacks.Last().Weight;
                else knapsackGen.Fitness = _opt;
            }

        }

        protected override void Mutate(KnapsackGen member)
        {
            int ipos = Rand.Next() % _numOfItems;
            int val = (Rand.Next() % 1);
            member.ChosenItems[ipos] = val;
        }

        protected override void mate_by_method(KnapsackGen bufGen, KnapsackGen gen1, KnapsackGen gen2)
        {
            int spos = Rand.Next() % _numOfItems;
            int spos2 = Rand.Next() % (_numOfItems - spos) + spos;
            
            switch (CrosMethod)
            {
                case CrossoverMethod.SinglePoint:
                    Array.Copy(gen1.ChosenItems,bufGen.ChosenItems, spos);
                    Array.Copy(gen2.ChosenItems,spos+1,bufGen.ChosenItems, spos+1,_numOfItems- spos);
                    //bufGen.Str = gen1.Str.Substring(0, spos) + gen2.Str.Substring(spos, targetLenght - spos);
                    break;
                case CrossoverMethod.TwoPoint:
                    Array.Copy(gen1.ChosenItems, bufGen.ChosenItems, _numOfItems);
                    Array.Copy(gen2.ChosenItems, spos + 1, bufGen.ChosenItems, spos + 1, spos2 - spos);

                    //bufGen.Str = gen1.Str.Substring(0, spos) + gen2.Str.Substring(spos, spos2 - spos) + gen1.Str.Substring(spos2, targetLenght - spos2);
                    break;
                case CrossoverMethod.Uniform:
                    //StringBuilder sb = new StringBuilder(StrTarget);
                    for (int j = 0; j < _numOfItems; j++)
                    {
                        // randomlly choose char from either gens    
                        int genToChoose = Rand.Next() % 2;
                        bufGen.ChosenItems[j] = (genToChoose == 0) ? gen1.ChosenItems[j] : gen2.ChosenItems[j];
                    }
                    //bufGen.Str = sb.ToString();
                    break;
            }
        }

        protected override Tuple<string, uint> get_best_gen_details(KnapsackGen gen)
        {
            string str = String.Join("", new List<int>(Population[0].ChosenItems).ConvertAll(i => i.ToString()).ToArray());
            Tuple<string,uint> best = new Tuple<string, uint>(str,Population[0].Fitness);
            return best;
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
