using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            throw new NotImplementedException();
        }

        public override void init_population()
        {
            Population = new List<KnapsackGen>();
            Buffer = new List<KnapsackGen>();
            for (int i = 0; i < GaPopSize; i++)
            {
                KnapsackGen ksGen = new KnapsackGen(_numOfknapsacks,);
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
