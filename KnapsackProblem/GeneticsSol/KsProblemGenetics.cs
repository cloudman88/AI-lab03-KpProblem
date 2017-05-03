﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using KnapsackProblem.GeneticsAlgorithms;
using KnapsackProblem.HeuristicSol;
using KnapsackProblem.Tools;

namespace KnapsackProblem.GeneticsSol
{
    class KsProblemGenetics : GeneticsAlgorithms<KnapsackGen>
    {
        private readonly uint _opt;
        private readonly int _numOfknapsacks;
        private readonly int _numOfItems;
        private readonly List<short> _capcities;
        private readonly List<uint> _weights;
        private readonly ObservableCollection<short[]> _constrains;

        public KsProblemGenetics(CrossoverMethod crossMethod, SelectionMethod selectionMethod) : base(crossMethod, selectionMethod)
        {
            _capcities = new List<short>();
            _weights = new List<uint>();
            _constrains = new ObservableCollection<short[]>();
            string filePath = "hp1.DAT";
            KsProblem.ReadDataFromFile(filePath,ref _numOfknapsacks,ref _numOfItems ,_weights,_capcities,_constrains,ref _opt);
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
                        Item item = new Item()
                        {
                            Weight = _weights[i],
                            Constrains = new short[_numOfknapsacks]
                        };
                        for (int k = 0; k < _numOfknapsacks; k++)
                        {
                            item.Constrains[k] = _constrains[k][i];
                        }
                        for (int j = 0; j < _numOfknapsacks; j++)
                        {
                            knapsackGen.Knapsacks[j].PackedItems.Add(item);
                            knapsackGen.Knapsacks[j].Weight += item.Weight;
                            knapsackGen.Knapsacks[j].Value += item.Constrains[j];                            
                        }                        
                    }
                }
                bool capcityExceeded = false;
                foreach (var ks in knapsackGen.Knapsacks)
                {
                    if (ks.Value > ks.Capacity) capcityExceeded = true;
                }
                //once mate had done, the knapsack  items may have exceed its capcity, fitness will calculated accordingly
                if (knapsackGen.Knapsacks.Last().Weight > _opt || capcityExceeded==true)  
                    knapsackGen.Fitness = _opt;
                else knapsackGen.Fitness = _opt - knapsackGen.Knapsacks.Last().Weight;
            }
        }

        protected override void Mutate(KnapsackGen member)
        {
            int ipos = Rand.Next() % _numOfItems;
            int val = (Rand.Next() % 2);
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
                    Array.Copy(gen2.ChosenItems,spos,bufGen.ChosenItems, spos,_numOfItems- spos);
                    break;
                case CrossoverMethod.TwoPoint:
                    Array.Copy(gen1.ChosenItems, bufGen.ChosenItems, _numOfItems);
                    Array.Copy(gen2.ChosenItems, spos , bufGen.ChosenItems, spos , spos2 - spos);
                    break;
                case CrossoverMethod.Uniform:
                    for (int j = 0; j < _numOfItems; j++)
                    {
                        // randomlly choose char from either gens    
                        int genToChoose = Rand.Next() % 2;
                        bufGen.ChosenItems[j] = (genToChoose == 0) ? gen1.ChosenItems[j] : gen2.ChosenItems[j];
                    }
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
            int sum = 0;
            for (int i = 0; i < _numOfItems; i++)
            {
                if (gen1.ChosenItems[i] != gen2.ChosenItems[i]) sum += 1;
            }
            return sum;
        }
    }
}
