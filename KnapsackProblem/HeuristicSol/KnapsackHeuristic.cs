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
    class KnapsackHeuristic
    {
        private uint _counter;
        private uint _opt;
        private uint _estimate;
        private readonly int _numOfknapsacks;
        private readonly int _numOfItems;
        private readonly List<short> _capcities;
        private readonly List<uint> _weights;
        private readonly ObservableCollection<short[]> _constrains;
        private readonly SearchAlgorithm _searchAlgorithm;
        private readonly NeglectedConstrain _neglectedConstrain;
        private string _chosenItems; //binary string representing if item x was chosen in the solution
        private Node _best;
        //private List<Tools.Knapsack> _knapsacks;

        public KnapsackHeuristic(SearchAlgorithm searchAlgorithm,NeglectedConstrain neglectedConstrain)
        {
            _counter = 0;
            _weights = new List<uint>();
            _capcities = new List<short>();
            _constrains = new ObservableCollection<short[]>();
            _chosenItems = "";
            string filePath = "pet3.DAT";
            ksIO.ReadDataFromFile(filePath,ref _numOfknapsacks,ref _numOfItems,_weights,_capcities,_constrains,ref _opt);
            //_knapsacks = new List<Tools.Knapsack>();
            //for (int i = 0; i < _numOfknapsacks; i++)
            //{
            //    Tools.Knapsack ks = new Tools.Knapsack(i+1, _capcities[i]);
            //    _knapsacks.Add(ks);
            //}
            _searchAlgorithm = searchAlgorithm;
            _neglectedConstrain = neglectedConstrain;
        }

        public void run_algorithm()
        {
            switch (_neglectedConstrain)
            {
                case NeglectedConstrain.Capacity:
                    _estimate = (uint)_weights.Sum(num => num);
                    break;
                case NeglectedConstrain.Integrality:
                    //_estimate = calc_estimate_neglecting_integrality();
                    break;
            }
            _best = new Node(0, _numOfknapsacks, _capcities.ToArray(), 0, 0);
            short[] rooms = new short[_numOfknapsacks];
            Array.Copy(_capcities.ToArray(), rooms, _numOfknapsacks);
            Node root = new Node(0, _numOfknapsacks, rooms, _estimate, 0);
            //solving while iterating between Branch and Bound
            switch (_searchAlgorithm)
            {
                case SearchAlgorithm.BestFirstSearch:
                    BestFirstSearch(root, "");
                    break;
                case SearchAlgorithm.DepthFirstSearch:
                    DepthFirstSearch(root,"");
                    break;
            }
            print_result_details();
        }

        private void DepthFirstSearch(Node root, string res)
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
                root.Right = new Node(root.Value, _numOfknapsacks, root.Rooms,
                                        (root.Estimate - _weights[root.Level]), (byte) (root.Level + 1));
                DepthFirstSearch(root.Left ,res+ "1 ");
                DepthFirstSearch(root.Right,res+"0 ");
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

        private void BestFirstSearch(Node root,string res)
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