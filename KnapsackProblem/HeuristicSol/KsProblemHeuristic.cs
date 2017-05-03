using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using KnapsackProblem.Tools;

namespace KnapsackProblem.HeuristicSol
{
    enum NeglectedConstrain //Relaxing the problem
    {
        Capacity,
        Integrality
    }
    enum SearchAlgorithm
    {
        DepthFirstSearch,
        BestFirstSearch
    }
    enum KsProbelmFiles //the .dat files containing the knapsack problems
    {
        Flei,
        Hp3,
        Pb5,       
        Pet2,
        Pet3,
        Pet4,       
    }
    class KsProblemHeuristic : KsProblem
    {
        private uint _estimationBound;
        private readonly SearchAlgorithm _searchAlgorithm;
        private readonly NeglectedConstrain _neglectedConstrain;        
        private string _chosenItems; //binary string representing if item x was chosen in the solution
        private Node _best;
        
        public KsProblemHeuristic(SearchAlgorithm searchAlgorithm,NeglectedConstrain neglectedConstrain)
        {
            _chosenItems = "";            
            _searchAlgorithm = searchAlgorithm;
            _neglectedConstrain = neglectedConstrain;
        }

        public void run_algorithm()
        {
            var ksProbelms = Enum.GetValues(typeof(KsProbelmFiles)).Cast<KsProbelmFiles>().Select(x => x.ToString()).ToArray();
            foreach (var problem in ksProbelms)
            {
                _chosenItems = "";
               ReadDataFromFile(problem + ".dat");
                switch (_neglectedConstrain)
                {
                    case NeglectedConstrain.Capacity:
                        _estimationBound = (uint) Weights.Sum(num => num);
                        break;
                    case NeglectedConstrain.Integrality:
                        BuildItemsList(true);
                        _estimationBound = calc_estimate_neglecting_integrality();
                        break;
                }
                _best = new Node(0, NumOfknapsacks, Capcities.ToArray(), 0, 0);
                short[] rooms = new short[NumOfknapsacks];
                Array.Copy(Capcities.ToArray(), rooms, NumOfknapsacks);
                Node root = new Node(0, NumOfknapsacks, rooms, _estimationBound, 0);
                //solving while iterating between Branch and Bound
                switch (_searchAlgorithm)
                {
                    case SearchAlgorithm.BestFirstSearch:
                        BestFirstSearch(root);
                        break;
                    case SearchAlgorithm.DepthFirstSearch:
                        DepthFirstSearch(root);
                        break;
                }
                print_result_details();
            }
        }
        private uint calc_estimate_neglecting_integrality(string chosenItems = "")
        {
            double estimateBound = 0;
            List<Item> items = new List<Item>();
            int count = 0;
            if (!chosenItems.Equals(""))
            {
                var binaryNumbers = chosenItems.Replace(" ", "");
                List<int> numbers = new List<int>();
                foreach (var bin in binaryNumbers)
                {
                    numbers.Add(Int32.Parse(bin.ToString()));
                }            
                count = numbers.Count;                
                for (int i = 0; i < count; i++)
                {
                    if (numbers[i] == 1)
                    {
                        items.Add(Items[i]);
                    }
                }
            }            
            for (int i = count; i < NumOfItems; i++)
            {
                items.Add(Items[i]);
            }
            var itemsSorted = items.OrderByDescending(x => x.DensitiesAvg);     
            short[] rooms = new short[NumOfknapsacks];
            Array.Copy(Capcities.ToArray(), rooms, NumOfknapsacks);
            foreach (var item in itemsSorted)
            {
                bool canBeAddToAllSacks = true;
                for (int j = 0; j < NumOfknapsacks; j++)
                {
                    if (rooms[j] <item.Constrains[j])
                    {
                        canBeAddToAllSacks = false;
                        break;
                    }
                }
                if (canBeAddToAllSacks == true)
                {
                    for (int j = 0; j < NumOfknapsacks; j++)
                    {
                        rooms[j] = (short)(rooms[j] - item.Constrains[j]);
                    }
                    estimateBound += item.Weight;
                }
                else //add fraction
                {
                    double fraction = Int16.MaxValue;
                    for (int j = 0; j < NumOfknapsacks; j++)
                    {
                        if (item.Constrains[j] != 0)
                        {
                            double temp = (rooms[j]/(double)item.Constrains[j])*item.Weight;
                            if (fraction > temp) fraction = temp; //take the smallest fraction
                        }
                    }
                    estimateBound += fraction;                        
                    break; //stop because we filled up all the rooms
                }
            }
            return (uint) estimateBound;
        }
        private void DepthFirstSearch(Node root, string res = "")
        {
            if (root == null) return ;
            if (root.check_rooms() == false) return ;
            if (root.Level < NumOfItems)
            {
                short[] newRooms = new short[NumOfknapsacks];
                Array.Copy(root.Rooms, newRooms, NumOfknapsacks);
                for (int i = 0; i < NumOfknapsacks; i++)
                {
                    newRooms[i] = (short) (newRooms[i] - Constrains[i][root.Level]);
                }
                // allocation of the node's sons is exceuted only when necessary 
                root.Left = new Node((root.Value + Weights[root.Level]), NumOfknapsacks, newRooms, root.Estimate, (byte)(root.Level + 1));
                uint est = 0;
                if (_neglectedConstrain.Equals(NeglectedConstrain.Integrality))
                {
                    est = calc_estimate_neglecting_integrality(res +"0");
                }
                else est = (root.Estimate - Weights[root.Level]);
                root.Right = new Node(root.Value, NumOfknapsacks, root.Rooms,est, (byte) (root.Level + 1));
                DepthFirstSearch(root.Left ,res+ "1 ");
                DepthFirstSearch(root.Right,res+ "0 ");
            }
            else
            {
                if (_best.Value < root.Value)
                {
                    _best = new Node(root);
                    _chosenItems = string.Copy(res);
                }
            }
        }      
        private void BestFirstSearch(Node root,string res="")
        {
            if (root == null) return;
            if (root.check_rooms() == false) return;
            if (root.Level < NumOfItems)
            {
                if (root.Estimate >= _best.Estimate)
                {
                    short[] newRooms = new short[NumOfknapsacks];
                    Array.Copy(root.Rooms, newRooms, NumOfknapsacks);
                    for (int i = 0; i < NumOfknapsacks; i++)
                    {
                        newRooms[i] = (short)(newRooms[i] - Constrains[i][root.Level]);
                    }
                    root.Left = new Node((root.Value + Weights[root.Level]), NumOfknapsacks, newRooms, root.Estimate, (byte)(root.Level + 1));
                    uint est = 0;
                    if (_neglectedConstrain.Equals(NeglectedConstrain.Integrality))
                    {
                        est = calc_estimate_neglecting_integrality(res + "0");
                    }
                    else est = (root.Estimate - Weights[root.Level]);
                    root.Right = new Node(root.Value, NumOfknapsacks, root.Rooms, est, (byte)(root.Level + 1));
                    BestFirstSearch(root.Left,res+ "1 ");
                    BestFirstSearch(root.Right, res + "0 ");                    
                }
            }
            else
            {
                if (_best.Value < root.Value)
                {
                    _best = new Node(root);
                    _chosenItems = res;
                }
            }
        }
        private void print_result_details()
        {
            Console.WriteLine("Best node:");
            Console.WriteLine("Value: "+_best.Value);
            string rooms = "";
            foreach (int room in _best.Rooms)
            {
                rooms += room + " ";
            }
            Console.WriteLine("Rooms: "+ rooms);
            Console.WriteLine("Estimate: "+ _best.Estimate);
            Console.WriteLine("Chosen items : "+ _chosenItems);
            Console.WriteLine("Optimum solution : "+ Opt+"\n");
        }
    }
}
//private void BuildItemsList()
//{
//    Items.Clear();
//    for (int i = 0; i < NumOfItems; i++)
//    {
//        Item item = new Item()
//        {
//            Constrains = new short[NumOfknapsacks],
//            Densities = new float[NumOfknapsacks],
//            DensitiesAvg = 0,
//            Weight = Weights[i]
//        };
//        for (int j = 0; j < NumOfknapsacks; j++)
//        {
//            item.Constrains[j] = Constrains[j][i];
//            if (Constrains[j][i] != 0)
//                item.Densities[j] += (float) Weights[i]/Constrains[j][i];
//            else
//                item.Densities[j] += float.MaxValue; //if constrain is zero, then most weight per constrain is optimal
//        }
//        item.DensitiesAvg = item.Densities.Average();
//        Items.Add(item);
//    }
//}

//enum KsProbelms //the .dat files containing the knapsack problems
//{
//    Flei,
//    Hp1,
//    Hp2,
//    Hp3,
//    Pb1,
//    Pb2,
//    Pb3,
//    Pb4,
//    Pb5,
//    Pb6,
//    Pb7,
//    Pet2,
//    Pet3,
//    Pet4,
//    Pet5,
//    Pet6,
//    Pet7,
//    Sento1,
//    Sento2,
//    Weing1,
//    Weing2,
//    Weing3,
//    Weing4,
//    Weing5,
//    Weing6,
//    Weing7,
//    Weing8,
//    Weish01,
//    Weish02,
//    Weish03,
//    Weish04,
//    Weish05,
//    Weish06,
//    Weish07,
//    Weish08,
//    Weish09,
//    Weish10,
//    Weish11,
//    Weish12,
//    Weish13,
//    Weish14,
//    Weish15,
//    Weish16,
//    Weish17,
//    Weish18,
//    Weish19,
//    Weish20,
//    Weish21,
//    Weish22,
//    Weish23,
//    Weish24,
//    Weish25,
//    Weish26,
//    Weish27,
//    Weish28,
//    Weish29,
//    Weish30
//}