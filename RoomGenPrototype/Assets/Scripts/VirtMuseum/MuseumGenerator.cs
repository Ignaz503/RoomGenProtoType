using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;



public class MuseumGenerator : MonoBehaviour {

    public static MuseumGenerator Instance;

    static Queue<Tuple<MuseumRequest,Queue<string>>> MuseumRequests = new Queue<Tuple<MuseumRequest, Queue<string>>>();

    [Range(1, 10)]
    public int SIZE;

    public Color[] RoomTypeColors;

    public string Seed = "";

    public GameObject cubePrefab;

    public RoomType tpyeToDrawWallsFor;

    public bool DrawAllWalls;

    public bool DrawGizmos;

    public Museum VirtMuse = null;

    public bool CreateDebugGameobject;

    private void OnEnable()
    {
        Instance = this;
    }

    private void Update()
    {
        if(MuseumRequests.Count > 0)
        {
            //TODO Move away from update
            Tuple<MuseumRequest,Queue<string>> req = null;
            if ((req = MuseumRequests.Peek())!= null)
            {
                Museum virt = new Museum((int)req.Item1.Size);
                virt.Generate(Seed);// temp
                VirtMuse = virt;

                if (CreateDebugGameobject)
                    GenerateTempRoomDisplay();

                //using (FileStream f = File.Open(Application.persistentDataPath + @"\VirtMuse.xml", FileMode.OpenOrCreate))
                //{
                //    using (StreamWriter w = new StreamWriter(f))
                //    {
                //        w.Write(VirtMuse.Serialize());
                //    }
                //}
                req.Item2.Enqueue(VirtMuse.Serialize());
                gameObject.SetActive(false);
        }
        }
    }

    public void RequestNewMuseum(string request, Queue<string> requester)
    {
        MuseumRequests.Enqueue(new Tuple<MuseumRequest, Queue<string>>(
                                    MuseumRequest.Deserialize(request),
                                    requester));
    }

    [Obsolete]
    private void GenerateTempRoomDisplay(int height, HashSet<int> outline)
    {
        for (int x = 0; x < SIZE; x++)
        {
            for (int y = 0; y < SIZE; y++)
            {
                int id = x + (y * SIZE);

                GameObject obj = Instantiate(cubePrefab);

                obj.transform.position = new Vector3(x, height * 10.5f, y);
                obj.name = $"{(RoomType)VirtMuse.RoomMap[x, y]}: {x},{y},{height}";

                MeshRenderer re = obj.GetComponent<MeshRenderer>();
                if (VirtMuse.RoomMap[x, y] != -1)
                    re.material.color = RoomTypeColors[VirtMuse.RoomMap[x, y]];
                else
                    re.material.color = Color.magenta;

                if (outline.Contains(id))
                    re.material.color = Color.black;

            }
        }
    }

    /// <summary>
    /// gnerates a temp display of the museum but onyl floor
    /// walls handled by draw gizmos
    /// </summary>
    private void GenerateTempRoomDisplay()
    {
        for (int x = 0; x < VirtMuse.Size; x++)
        {
            for (int y = 0; y < VirtMuse.Size; y++)
            {
                GameObject obj = Instantiate(cubePrefab);

                obj.transform.position = new Vector3(x,0, y);
                obj.name = $"{(RoomType)VirtMuse.RoomMap[x, y]}: {x},{y}";

                MeshRenderer re = obj.GetComponent<MeshRenderer>();
                if (VirtMuse.RoomMap[x, y] != -1)
                    re.material.color = RoomTypeColors[VirtMuse.RoomMap[x, y]];
                else
                    re.material.color = Color.magenta;
            }
        }
    }

    /// <summary>
    /// gnerates walls for museum in editor and color codes them depending on type
    /// </summary>
    private void OnDrawGizmos()
    {
        if (VirtMuse != null && VirtMuse.Rooms != null &&DrawGizmos)
        {
            foreach (Room r in VirtMuse.Rooms)
            {
                if(r.Type == tpyeToDrawWallsFor || DrawAllWalls)
                {
                    foreach (int wallIdx in r.Walls)
                    {
                        Wall wall = VirtMuse.Walls[wallIdx];

                        Gizmos.color = wall.Type == Wall.WallType.Solid ? Color.black : Color.green;
                        Vector3 cubeLocation = new Vector3(wall.Location[0].x , 1f, wall.Location[0].y);
                        Vector3 cubeSize = wall.Location[0] - wall.Location[1];
                        Gizmos.DrawCube(cubeLocation - (new Vector3(cubeSize.x,0,cubeSize.y)*.5f), new Vector3(cubeSize.x +0.01f, 1f, cubeSize.y + 0.01f));
                    }// end foreach wall
                }
            }// end foreach room
        }// end if rooms not null
    }
}