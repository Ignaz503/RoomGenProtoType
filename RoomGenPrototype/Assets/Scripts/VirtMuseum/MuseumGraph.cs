using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[DataContract]
public class MuseumGraph
{
    [DataContract]
    public class Node
    {
        [DataMember]
        public uint NodeID { get; protected set; } 

        [DataMember]
        public HashSet<uint> Edges { get; protected set; }

        public Node(uint nodeID)
        {
            NodeID = nodeID;
            Edges = new HashSet<uint>();
        }

        public bool AddEdge(uint newEdge)
        {
            return Edges.Add(newEdge);
        }

        public static void ConnectNodes(Node a, Node b)
        {
            a.AddEdge(b.NodeID);
            b.AddEdge(a.NodeID);
        }

        public override bool Equals(object obj)
        {
            if(obj is Node)
            {
                Node o = obj as Node;
                return NodeID == o.NodeID;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return NodeID.GetHashCode();
        }

        public override string ToString()
        {
            return ""+NodeID;
        }
    }

    [DataMember]
    Dictionary<uint, Node> NodeMap;

    public List<Node> Nodes { get { return NodeMap.Values.ToList(); } }

    public MuseumGraph()
    {
        NodeMap = new Dictionary<uint, Node>();
    }

    public void BuildMuseumGraph(Museum virtMuse)
    {
        //create nodes for all rooms
        foreach(Room r in virtMuse.Rooms)
        {
            NodeMap.Add(r.RoomID, new Node(r.RoomID));
        }

        //loop frough all walls if door get associated rooms create
        //Connection between nodes
        foreach(Wall w in virtMuse.Walls)
        {
            if(w.Type == Wall.WallType.Door)
            {
                if (w.AssociatedRoomIDs.Count == 2)
                {
                    Node a = NodeMap[w.AssociatedRoomIDs[0]];
                    Node b = NodeMap[w.AssociatedRoomIDs[1]];
                    Node.ConnectNodes(a, b);
                }                
            }
        }
    }

    public Node GetNodeForRoom(Room r)
    {
        if (NodeMap.ContainsKey(r.RoomID))
            return NodeMap[r.RoomID];
        else
            return null; 
    }
}
