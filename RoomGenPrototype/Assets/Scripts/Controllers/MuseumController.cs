using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuseumController : MonoBehaviour {

    public static MuseumController Instance;

    [SerializeField] float xPositionScaler;
    [SerializeField] float zPositionScaler;
    [SerializeField] Transform player;
    [SerializeField] bool manageRooms;
    [SerializeField] bool drawGraphGizmos;
    [SerializeField] bool drawRoomManagmentUnitGizmos;

    MuseumGraph virtMuseGraph;
    int museumSize;

    Dictionary<uint, RoomManagmentUnit> roomManagmentMap;
    //temp
    Dictionary<uint, Room> roomDictionary;
    Room[,] roomMap;
    Room roomPlayerIsIn;
      
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There already exists a museum controller");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    // Use this for initialization
    void Start () {

        roomManagmentMap = new Dictionary<uint, RoomManagmentUnit>();

        roomDictionary = new Dictionary<uint, Room>();

        xPositionScaler = MuseumBuilder.Instance.FloorXPosScale;
        zPositionScaler = MuseumBuilder.Instance.FloorZPosScale;
    }
	
	// Update is called once per frame
	void Update () {
		if(virtMuseGraph != null)
        {
            int x = Mathf.RoundToInt(player.position.x / xPositionScaler);
            int y = Mathf.RoundToInt(player.position.z / zPositionScaler);

            if (InBounds(x, y))
            {
                Room possNewRoom = roomMap[x, y];

                if(roomPlayerIsIn == null)
                {
                    roomPlayerIsIn = possNewRoom;
                    if(manageRooms)
                        LoadRoomAndNeighbors(roomPlayerIsIn);
                }

                if (possNewRoom != null && roomPlayerIsIn.RoomID != roomMap[x, y].RoomID)
                {
                    if (manageRooms)
                    {
                        //TODO no need to unload all and load again
                        // optimize this shit
                        //"unload" all old rooms
                        UnloadRoomAndNeighbors(roomPlayerIsIn);
                        //load new rooms
                        LoadRoomAndNeighbors(possNewRoom);
                    }
                    roomPlayerIsIn = possNewRoom;
                }// end if new room
            }// end if in bounds
        }// end if graph not null
	}

    void LoadRoomAndNeighbors(Room rToLoad)
    {
        if (roomManagmentMap.ContainsKey(rToLoad.RoomID))
            roomManagmentMap[rToLoad.RoomID].LoadRoom();

        foreach (uint edge in virtMuseGraph.GetNodeForRoom(rToLoad).Edges)
        {
            if (roomManagmentMap.ContainsKey(edge))
            {
                roomManagmentMap[edge].LoadRoom();
            }
        }
    }

    void UnloadRoomAndNeighbors(Room rToUnload)
    {
        if (roomManagmentMap.ContainsKey(rToUnload.RoomID))
            roomManagmentMap[rToUnload.RoomID].LoadRoom();

        foreach (uint edge in virtMuseGraph.GetNodeForRoom(rToUnload).Edges)
        {
            if (roomManagmentMap.ContainsKey(edge))
            {
                roomManagmentMap[edge].UnloadRoom();
            }
        }
    }

    public void SetMuseumToControl(Museum virMus)
    {
        virtMuseGraph = virMus.MuseumsGraph;
        roomMap = new Room[virMus.Size, virMus.Size];
        museumSize = virMus.Size;

        foreach(Room r in virMus.Rooms)
        {
            foreach(Vector2Int tile in r.RoomTiles)
            {
                if(tile.x >= 0 && tile.x < virMus.Size && tile.y >= 0 && tile.y < virMus.Size)
                    roomMap[tile.x, tile.y] = r;
            }
            roomManagmentMap.Add(r.RoomID, new RoomManagmentUnit(r.RoomID,true));
            roomDictionary.Add(r.RoomID, r);
        }
    }

    bool InBounds(int x, int y)
    {
        return x >= 0 && x < museumSize && y >= 0 && y < museumSize;
    }

    private void OnDrawGizmos()
    {
        if(virtMuseGraph != null && drawGraphGizmos)
        {
            DrawMuseumGraph();

        }// end if virt muse graph not null

        if(roomPlayerIsIn != null && drawRoomManagmentUnitGizmos)
        {
            if (roomManagmentMap.ContainsKey(roomPlayerIsIn.RoomID))
            {
                foreach (GameObject obj  in roomManagmentMap[roomPlayerIsIn.RoomID].GameobjectsInRoom)
                {
                    Gizmos.DrawSphere(obj.transform.position, .5f);
                }
            }
        }
    }

    void DrawMuseumGraph()
    {
        foreach (MuseumGraph.Node n in virtMuseGraph.Nodes)
        {
            if (roomManagmentMap.ContainsKey(n.NodeID))
            {
                Vector2Int rLoc = roomDictionary[n.NodeID].RoomTiles[0];

                Gizmos.color = Color.black;
                Vector3 roomPos = new Vector3(rLoc.x * xPositionScaler, 20f, rLoc.y * zPositionScaler);
                Gizmos.DrawSphere(roomPos, 1f);

                foreach (uint edge in n.Edges)
                {
                    if (roomManagmentMap.ContainsKey(edge))
                    {
                        Vector2Int edgeLoc = roomDictionary[edge].RoomTiles[0];
                        Gizmos.color = Color.cyan;
                        Gizmos.DrawLine(roomPos, new Vector3(edgeLoc.x * xPositionScaler, 20f, edgeLoc.y * zPositionScaler));
                    }// end if  room dict contains edge
                }// end foreach edge
            }// end if room dictionary contains room
        }// end foreach virtmuse graph node
    }

    public RoomManagmentUnit GetRoomManagmentUnitForRoom(uint r)
    {
        if (roomManagmentMap.ContainsKey(r))
            return roomManagmentMap[r];
        return null;
    }

    public void LogRoomManagmentUnitForRoom(uint roomID)
    {
        if (roomManagmentMap.ContainsKey(roomID))
        {
            Debug.Log(roomManagmentMap[roomID].ToString());
        }
    }
}
