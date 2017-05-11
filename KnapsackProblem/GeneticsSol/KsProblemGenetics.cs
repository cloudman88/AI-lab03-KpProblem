using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using KnapsackProblem.GeneticsAlgorithms;
using KnapsackProblem.Tools;

namespace KnapsackProblem.GeneticsSol
{
    class KsProblemGenetics : GeneticsAlgorithms<KnapsackGen>
    {
        private uint _opt;
        private int _numOfknapsacks;
        private int _numOfItems;
        private readonly List<short> _capcities;
        private readonly List<uint> _weights;
        private readonly List<Item> _items;
        private readonly ObservableCollection<short[]> _constrains;

        public KsProblemGenetics(CrossoverMethod crossMethod, SelectionMethod selectionMethod) : base(crossMethod, selectionMethod)
        {
            _capcities = new List<short>();
            _weights = new List<uint>();
            _items = new List<Item>();
            _constrains = new ObservableCollection<short[]>();
            //string filePath = "WEISH11.DAT";
            //KsProblem.ReadDataFromFile(filePath,ref _numOfknapsacks,ref _numOfItems ,_weights,_capcities,_constrains,ref _opt);
            //BuildItemsList();
        }

        private void BuildItemsList()
        {
            _items.Clear();
            for (int i = 0; i < _numOfItems; i++)
            {
                Item item = new Item()
                {
                    Weight = _weights[i],
                    Constrains = new short[_numOfknapsacks],
                };
                for (int j = 0; j < _numOfknapsacks; j++)
                {
                    item.Constrains[j] = _constrains[j][i];
                }
                _items.Add(item);
            }
        }
        public override void init_population()
        {
            Population = new List<KnapsackGen>();
            Buffer = new List<KnapsackGen>();
            for (int i = 0; i < GaPopSize; i++)
            {
                KnapsackGen ksGen = new KnapsackGen(_numOfknapsacks,_capcities.ToArray(),_numOfItems,_items);
                Population.Add(ksGen);
                Buffer.Add(ksGen);
            }
        }

        public override void run_algorithm()
        {
            string text = "";
            var ksProbelms = Enum.GetValues(typeof(KsProbelmFiles)).Cast<KsProbelmFiles>()
                                                    .Select(x => x.ToString()).ToArray();
            foreach (var problem in ksProbelms)
            {
                double fitAvg = 0;
                double timeAvg = 0;
                int successCount = 0;
                for (int j = 0; j < 10; j++)
                {
                    _numOfknapsacks = 0;
                    _numOfItems = 0;
                    BestGensHistory.Clear();
                    HyperMutWasCalled = false;
                    KsProblem.ReadDataFromFile(problem + ".dat", ref _numOfknapsacks, ref _numOfItems, _weights, _capcities, _constrains, ref _opt);
                    BuildItemsList();
                    init_population();
                    long totalTicks = 0;
                    int totalIteration = -1;
                    Stopwatch stopWatch = new Stopwatch(); //stopwatch is used for both clock ticks and elasped time measuring
                    stopWatch.Start();
                    int i;
                    for (i = 0; i < GaMaxiter; i++)
                    {
                        calc_fitness();      // calculate fitness
                        sort_by_fitness();   // sort them
                        var avg = calc_avg(); // calc avg
                        var stdDev = calc_std_dev(avg); //calc std dev

                        //calculate time differences                
                        stopWatch.Stop();
                        double ticks = (stopWatch.ElapsedTicks / (double)Stopwatch.Frequency) * 1000;
                        totalTicks += (long)ticks;

                        print_result_details(Population[0], avg, stdDev, i);  // print the best one, average and std dev by iteration number                
                        if (LocalOptSearchEnabled == true) search_local_optima(avg, stdDev, i);

                        stopWatch.Restart(); // restart timers for next iteration
                        if ((Population)[0].Fitness == 0)
                        {
                            successCount++;
                            totalIteration = i + 1; // save number of iteration                                                           
                            break;
                        }
                        Mate();     // mate the population together
                        swap_population_with_buffer();       // swap buffers
                    }
                    if (i == GaMaxiter)
                    {                        
                        Console.WriteLine("Failed to find solution in " + i + " iterations.");
                    }
                    else
                    {
                        Console.WriteLine("Iterations: " + totalIteration);
                    }
                    fitAvg += Population[0].Fitness;
                    timeAvg += totalTicks;
                    Console.WriteLine("\nTimig in milliseconds:");
                    Console.WriteLine(problem+" Total Ticks " + totalTicks+"\n");
                }
                text += problem + " Value avg: " + (fitAvg/10) + " Opt: " + _opt + " Clock ticks avg: " + (timeAvg/10) +" rate: "+successCount+"/10"+ Environment.NewLine;
                File.WriteAllText("output_genetics.txt", text);                
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
                //once mate had done, the knapsack  items may have exceed its capcity,
                //fitness will calculated accordingly
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
