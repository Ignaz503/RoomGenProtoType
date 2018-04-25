using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuseumController : MonoBehaviour {

    public static MuseumController Instance;

    [SerializeField] float xPositionScaler;
    [SerializeField] float zPositionScaler;
    [SerializeField] Transform player;

    MuseumGraph virtMuseGraph;
    int museumSize;

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
                roomPlayerIsIn = roomMap[x, y];
                if (roomPlayerIsIn != null)
                    Debug.Log(roomPlayerIsIn.RoomID);
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
            roomDictionary.Add(r.RoomID, r);
        }
    }

    bool InBounds(int x, int y)
    {
        return x >= 0 && x < museumSize && y >= 0 && y < museumSize;
    }

    private void OnDrawGizmos()
    {
        if(virtMuseGraph != null)
        {
            foreach(MuseumGraph.Node n in virtMuseGraph.Nodes)
            {
                if (roomDictionary.ContainsKey(n.NodeID))
                {
                    Vector2Int rLoc = roomDictionary[n.NodeID].RoomTiles[0];

                    Gizmos.color = Color.black;
                    Vector3 roomPos = new Vector3(rLoc.x * xPositionScaler, 20f, rLoc.y * zPositionScaler);
                    Gizmos.DrawSphere(roomPos, 1f);

                    foreach(uint edge in n.Edges)
                    {
                        if (roomDictionary.ContainsKey(edge))
                        {
                            Vector2Int edgeLoc = roomDictionary[edge].RoomTiles [0];
                            Gizmos.color = Color.cyan;
                            Gizmos.DrawLine(roomPos, new Vector3(edgeLoc.x * xPositionScaler, 20f, edgeLoc.y * zPositionScaler));
                        }// end if  room dict contains edge
                    }// end foreach edge
                }// end if room dictionary contains room
            }// end foreach virtmuse graph
        }// end if virt muse graph not null
    }

}
