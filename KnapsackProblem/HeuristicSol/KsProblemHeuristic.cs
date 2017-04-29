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
    class KsProblemHeuristic
    {
        private uint _counter;
        private uint _estimationBound;
        private readonly uint _opt;
        private readonly int _numOfknapsacks;
        private readonly int _numOfItems;
        private readonly List<short> _capcities;
        private readonly List<uint> _weights;
        private readonly List<Item> _items;
        private readonly ObservableCollection<short[]> _constrains;
        private readonly SearchAlgorithm _searchAlgorithm;
        private readonly NeglectedConstrain _neglectedConstrain;
        
        private string _chosenItems; //binary string representing if item x was chosen in the solution
        private Node _best;

        public KsProblemHeuristic(SearchAlgorithm searchAlgorithm,NeglectedConstrain neglectedConstrain)
        {
            _counter = 0;
            _weights = new List<uint>();
            _capcities = new List<short>();
            _constrains = new ObservableCollection<short[]>();
            _items = new List<Item>();
            _chosenItems = "";
            string filePath = "pet3.DAT";
            ksIO.ReadDataFromFile(filePath,ref _numOfknapsacks,ref _numOfItems,_weights,_capcities,_constrains,ref _opt);
            _searchAlgorithm = searchAlgorithm;
            _neglectedConstrain = neglectedConstrain;
        }

        public void run_algorithm()
        {
            switch (_neglectedConstrain)
            {
                case NeglectedConstrain.Capacity:
                    _estimationBound = (uint)_weights.Sum(num => num);
                    break;
                case NeglectedConstrain.Integrality:
                    BuildItemsList();
                    _estimationBound = calc_estimate_neglecting_integrality();
                    break;
            }
            _best = new Node(0, _numOfknapsacks, _capcities.ToArray(), 0, 0);
            short[] rooms = new short[_numOfknapsacks];
            Array.Copy(_capcities.ToArray(), rooms, _numOfknapsacks);
            Node root = new Node(0, _numOfknapsacks, rooms, _estimationBound, 0);
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

        private void BuildItemsList()
        {
            for (int i = 0; i < _numOfItems; i++)
            {
                Item item = new Item()
                {
                    Constrains = new short[_numOfknapsacks],
                    Densities = new float[_numOfknapsacks],
                    DensitiesAvg = 0,
                    Weight = _weights[i]
                };
                for (int j = 0; j < _numOfknapsacks; j++)
                {
                    item.Constrains[j] = _constrains[j][i];
                    if (_constrains[j][i] != 0) item.Densities[j] += (float)_weights[i] / _constrains[j][i];
                }
                item.DensitiesAvg = item.Densities.Average();
                _items.Add(item);
            }
        }

        private uint calc_estimate_neglecting_integrality(string chosenItems = "")
        {
            // pre work - calculation the density: Vi/Wi for each knapsack
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
                        items.Add(_items[i]);
                    }
                }
            }            
            for (int i = count; i < _numOfItems; i++)
            {
                items.Add(_items[i]);
            }
            var itemsSorted = items.OrderByDescending(x => x.DensitiesAvg);     
            short[] rooms = new short[_numOfknapsacks];
            Array.Copy(_capcities.ToArray(), rooms, _numOfknapsacks);
            foreach (var item in itemsSorted)
            {
                bool canBeAddToAllSacks = true;
                for (int j = 0; j < _numOfknapsacks; j++)
                {
                    if (rooms[j] <item.Constrains[j])
                    {
                        canBeAddToAllSacks = false;
                        break;
                    }
                }
                if (canBeAddToAllSacks == true)
                {
                    for (int j = 0; j < _numOfknapsacks; j++)
                    {
                        rooms[j] = (short)(rooms[j] - item.Constrains[j]);
                    }
                    estimateBound += item.Weight;
                }
                else //add fraction
                {
                    double fraction = Int16.MaxValue;
                    for (int j = 0; j < _numOfknapsacks; j++)
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
            if (root.Level < _numOfItems)
            {
                short[] newRooms = new short[_numOfknapsacks];
                Array.Copy(root.Rooms, newRooms, _numOfknapsacks);
                for (int i = 0; i < _numOfknapsacks; i++)
                {
                    newRooms[i] = (short) (newRooms[i] - _constrains[i][root.Level]);
                }
                // allocation of the node's sons is exceuted only when necessary 
                root.Left = new Node((root.Value + _weights[root.Level]), _numOfknapsacks, newRooms, root.Estimate, (byte)(root.Level + 1));
                uint est = 0;
                if (_neglectedConstrain.Equals(NeglectedConstrain.Integrality))
                {
                    est = calc_estimate_neglecting_integrality(res +"0");
                }
                else est = (root.Estimate - _weights[root.Level]);
                root.Right = new Node(root.Value, _numOfknapsacks, root.Rooms,est, (byte) (root.Level + 1));
                DepthFirstSearch(root.Left ,res+ "1 ");
                DepthFirstSearch(root.Right,res+ "0 ");
            }
            else
            {
                _counter++;
                if (_best.Value < root.Value)
                {
                    _best = new Node(root);
                    _chosenItems = string.Copy(res);
                }
            }
        }

        private void RemoveItemById(object items, byte level)
        {
            throw new NotImplementedException();
        }

        private void BestFirstSearch(Node root,string res="")
        {
            if (root == null) return;
            if (root.check_rooms() == false) return;
            if (root.Level < _numOfItems)
            {
                if (root.Estimate > _best.Estimate)
                {
                    short[] newRooms = new short[_numOfknapsacks];
                    Array.Copy(root.Rooms, newRooms, _numOfknapsacks);
                    for (int i = 0; i < _numOfknapsacks; i++)
                    {
                        newRooms[i] = (short)(newRooms[i] - _constrains[i][root.Level]);
                    }

                    root.Left = new Node((root.Value + _weights[root.Level]), _numOfknapsacks, newRooms, root.Estimate, (byte)(root.Level + 1));
                    root.Right = new Node(root.Value, _numOfknapsacks, root.Rooms,
                                            (root.Estimate - _weights[root.Level]), (byte)(root.Level + 1));
                    BestFirstSearch(root.Left,res+ "1 ");
                    BestFirstSearch(root.Right, res + "0 ");                    
                }
            }
            else
            {
                _counter++;
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
        }
    }
}