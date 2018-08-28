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
        /// <summary>
        /// Unique id of node in this graph
        /// </summary>
        [DataMember]
        public uint NodeID { get; protected set; }

        /// <summary>
        /// list of node ids where this node has edges to
        /// </summary>
        [DataMember]
        public HashSet<uint> Edges { get; protected set; }

        public Node(uint nodeID)
        {
            NodeID = nodeID;
            Edges = new HashSet<uint>();
        }

        /// <summary>
        /// adss an edge to this node
        /// </summary>
        /// <param name="newEdge">the id of the node where the new edge goes to</param>
        /// <returns></returns>
        public bool AddEdge(uint newEdge)
        {
            return Edges.Add(newEdge);
        }

        /// <summary>
        /// creates edges between two nodes
        /// </summary>
        public static void ConnectNodes(Node a, Node b)
        {
            a.AddEdge(b.NodeID);
            b.AddEdge(a.NodeID);
        }

        public override bool Equals(object obj)
        {
            if (obj is Node)
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
            return "" + NodeID;
        }
    }

    /// <summary>
    /// map that maps from node id to ref
    /// </summary>
    [DataMember]
    Dictionary<uint, Node> NodeMap;

    /// <summary>
    /// list of all the nodes in the graph
    /// </summary>
    public List<Node> Nodes { get { return NodeMap.Values.ToList(); } }

    public MuseumGraph()
    {
        NodeMap = new Dictionary<uint, Node>();
    }

    /// <summary>
    /// builds a graph of the museum
    /// nodes are rooms
    /// edges are when two rooms have a door between each other
    /// </summary>
    /// <param name="virtMuse">the museum the graph should be built for</param>
    public void BuildMuseumGraph(Museum virtMuse)
    {
        //create nodes for all rooms
        foreach (Room r in virtMuse.Rooms)
        {
            NodeMap.Add(r.RoomID, new Node(r.RoomID));
        }

        //loop frough all walls if door get associated rooms create
        //Connection between nodes
        foreach (Wall w in virtMuse.Walls)
        {
            if (w.Type == Wall.WallType.Door)
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

    /// <summary>
    /// gets a museum node that represents this room
    /// </summary>
    public Node GetNodeForRoom(Room r)
    {
        if (NodeMap.ContainsKey(r.RoomID))
            return NodeMap[r.RoomID];
        else
            return null;
    }
}
