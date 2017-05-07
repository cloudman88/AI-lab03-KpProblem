using System;

namespace KnapsackProblem.HeuristicSol
{
    class Node
    {
        public short[] Rooms;
        public byte Level;
        public uint Value;
        public uint Estimate;        

        public Node(uint value,int n,short[] rooms, uint estimate, byte lvl)
        {
            Value = value;
            Rooms = new short[n];
            Array.Copy(rooms,Rooms,n);
            Estimate = estimate;
            Level = lvl;
        }
        public Node(Node root)
        {
            Value = root.Value;
            Rooms = new short[root.Rooms.Length];
            Array.Copy(root.Rooms, Rooms, root.Rooms.Length);
            Estimate = root.Estimate;
            Level = root.Level;
        }
        public bool check_rooms() //check if a node has at least one sack with overweight
        {
            foreach (var room in Rooms)
            {
                if (room < 0) return false;
            }
            return true;
        }
    }
}