using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MuseumBuilder : MonoBehaviour
{

    public static MuseumBuilder Instance;
    public static Queue<string> MuseumData = new Queue<string>();

    #region Prefabs
    public GameObject FloorPrefab;
    public GameObject WallPrefab;
    public GameObject WallWithDoorPrefab;

    public GameObject CenterImageDisplayPrefab;
    public GameObject CenterMeshDisplay;
    public GameObject MeshDisplayPrefab;
    public GameObject WallImageDisplayPrefab;

    public GameObject CeilingPrefab;

    public GameObject Player;

    #endregion

    public MeshFilter[] TestMesh;
    public Texture[] TestTextures;

    public float FloorXPosScale;
    public float FloorZPosScale;

    float wallHeight;

    //Temp
    int TestTextSize;
    int currIdx = 0;

    Museum virtMuse = null;

    public bool DisableAfterRequest;

    MuseumController _MusemControllerInstance;

    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogError("There already exists a museum builder");
            Destroy(this);
            return;
        }

        Instance = this;

        TestTextSize = TestTextures.Length;

        FloorXPosScale = FloorPrefab.transform.localScale.x;
        FloorZPosScale = FloorPrefab.transform.localScale.z;

        wallHeight = WallPrefab.transform.localScale.y + (.5f * CeilingPrefab.transform.localScale.y);
    }

    private void Start()
    {
        #region MeshDisplayXPosModificiation
        //TODO remove and move to display that needs it
        Transform meshDisplayTrans  = MeshDisplayPrefab.transform.GetChild(MeshDisplayPrefab.transform.childCount - 1);

        float avg = (meshDisplayTrans.localScale.x + meshDisplayTrans.localScale.y + meshDisplayTrans.localScale.z) / 3f;

        MeshDisplay.XPosModifier = avg * .5f;

        MeshDisplay.XPosScale = (1.5f * meshDisplayTrans.localScale.x) / (WallPrefab.transform.localScale.x / FloorPrefab.transform.localScale.x);

        #endregion

        _MusemControllerInstance = MuseumController.Instance;

        //requesting museum (simulate comunication with server^^)
        //for testing of serialization of museum and deserialization
        MuseumGenerator.Instance.RequestNewMuseum((new MuseumRequest()
        {
            MuseumType = "TestMuseum",
            Size = MuseumSize.Large
        }).Serialize(),MuseumData);
        if (DisableAfterRequest)
            gameObject.SetActive(false);
    }

    private void Update()
    {
        if(MuseumData.Count > 0)
        {
            string data = null;
            if((data = MuseumData.Dequeue())!= null)
            {
                virtMuse = Museum.Deserialize(data);
                MuseumController.Instance.SetMuseumToControl(virtMuse);

                List<GameObject> floors = new List<GameObject>();
                SetUpFloor(floors);
                SetUpWalls(floors);

                //_MusemControllerInstance.LogRoomManagmentUnitForRoom(0);
                //_MusemControllerInstance.LogRoomManagmentUnitForRoom(17);

                Player.transform.position = new Vector3(floors[0].transform.position.x, 4f, floors[0].transform.position.z);
                Player.SetActive(true);
                gameObject.SetActive(false);
            }
        }
    }

 /// <summary>
    /// Sets up the floor for a museum
    /// fills list of gameobjects used for parenting information
    /// when setting up walls
    /// </summary>
    void SetUpFloor(List<GameObject> floors)
    {
        foreach (Room r in virtMuse.Rooms)
        {
            //create floor
            List<GameObject> roomFloors = new List<GameObject>();

            #region Floor Gameobject setup
            foreach (Vector2Int tile in r.RoomTiles)
            {
                GameObject floor = Instantiate(FloorPrefab);
                GameObject ceiling = Instantiate(CeilingPrefab);

                AddToRoomManagmentUnit(r.RoomID, floor);
                AddToRoomManagmentUnit(r.RoomID, ceiling);

                MeshRenderer rend = floor.GetComponent<MeshRenderer>();
                MeshRenderer ceilRend = ceiling.GetComponent<MeshRenderer>();
                floors.Add(floor);

                //TEMP
                //TODO REMOVE
                switch (r.Type)
                {
                    case RoomType.Normal:
                        rend.material.color = Color.yellow;
                        ceilRend.material.color = Color.yellow;
                        break;
                    case RoomType.Long:
                        rend.material.color = Color.red;
                        ceilRend.material.color = Color.red;
                        break;
                    case RoomType.Big:
                        rend.material.color = Color.green;
                        ceilRend.material.color = Color.green;
                        break;
                    case RoomType.L:
                        rend.material.color = Color.blue;
                        ceilRend.material.color = Color.blue;
                        break;
                }


                Vector3 pos = new Vector3(FloorXPosScale * tile.x, FloorPrefab.transform.position.y, FloorZPosScale * tile.y);

                string name = r.RoomID + " " + r.Type.ToString() + " " + tile.ToString();

                floor.transform.position = pos;
                floor.name = name;
                roomFloors.Add(floor);

                ceiling.name = name;
                pos.y += wallHeight;
                ceiling.transform.position = pos;

            }
            #endregion

            SetUpCenterDisplaysForRoom(r,roomFloors);
        }
    }

    /// <summary>
    /// Sets up wall gameobjects for a museum
    /// </summary>
    /// <param name="floors"></param>
    void SetUpWalls(List<GameObject> floors)
    {
        foreach (Wall w in virtMuse.Walls)
        {
            #region Wall gamobject setup
            GameObject wallObj = w.Type == Wall.WallType.Solid ? Instantiate(WallPrefab) : Instantiate(WallWithDoorPrefab);



            //position parenting
            GameObject parent = floors.Where((obj) => { return obj.name.Contains(w.Tiles[0].ToString()); }).First();
            if (parent != null)
                wallObj.transform.SetParent(parent.transform);

            float yPos = (w.Type == Wall.WallType.Solid ? wallObj.transform.localScale.y * .5f : wallObj.transform.localScale.y)+.5f;

            //rotation
            //TODO: Remove fixed values
            if (w.Rotation == Wall.WallRotation.Horizontal)
            {
                wallObj.transform.localPosition = new Vector3(0, yPos, .5f *w.PositionModifier);
            }
            else
            {
                wallObj.transform.localPosition = new Vector3(.5f * w.PositionModifier, yPos, 0);
            }

            Vector3 newRot = new Vector3(0f, 90f * (int)w.Rotation, 0);

            wallObj.transform.eulerAngles = newRot;

            wallObj.name =  w.Rotation.ToString() + " " + w.Tiles[0].ToString();

            //name
            if(w.AssociatedRoomIDs.Count < 2)
            {
                wallObj.name += " " + w.AssociatedRoomIDs[0];
            }
            else
            {
                wallObj.name += " "+w.AssociatedRoomIDs[0];
                wallObj.name += " " + w.AssociatedRoomIDs[1];
            }

            foreach (uint assIds in w.AssociatedRoomIDs)
            {
                AddToRoomManagmentUnit(assIds, wallObj);
            }

            #endregion

            if (w.Type == Wall.WallType.Solid)
                SetUpDisplaysForWall(w, wallObj);

            wallObj.transform.SetParent(null);

        }
    }

    /// <summary>
    /// Creates the center displays for a room
    /// List used for correct parenting of gamobject
    /// </summary>
    void SetUpCenterDisplaysForRoom(Room r, List<GameObject> associatedFloorGameobjects)
    {
        int i = 0;
        foreach(MuseumDisplayInfo dispInf in r.CenterDisplayInfos)
        {
            GameObject disp = (dispInf.Type == Display.DisplayType.ImageDisplay) ?
                Instantiate(CenterImageDisplayPrefab) :
                Instantiate(CenterMeshDisplay);

            disp.name = "Display " + r.RoomID + " " + r.RoomTiles[0];

            AddToRoomManagmentUnit(dispInf.AssociatedRoomID, disp);

            Display display = disp.GetComponentInChildren<Display>();

            display.SetUp(dispInf, associatedFloorGameobjects[i]);

            if (display != null)
                LoadResource(display, "test");
            i++;
        }
    }

    /// <summary>
    /// Creates wall displays for museum
    /// </summary>
    void SetUpDisplaysForWall(Wall w, GameObject wallObj)
    {
        foreach(MuseumDisplayInfo dispInf in w.DisplayInfos)
        {
            GameObject disp = (dispInf.Type == Display.DisplayType.ImageDisplay) ?
                Instantiate(WallImageDisplayPrefab) : Instantiate(MeshDisplayPrefab);
            AddToRoomManagmentUnit(dispInf.AssociatedRoomID, disp);

            Display display = disp.GetComponentInChildren<Display>();
            disp.name = w.Rotation + " " + display.GetType() +" "+ dispInf.AssociatedRoomID;

            display.SetUp(dispInf, wallObj);

            if (display != null)
                LoadResource(display, "test");
        }
    }

    /// <summary>
    /// Loads Resource for a dsiplay
    /// </summary>
    void LoadResource(Display disp, string resourceLocator)
    {
        System.Random rng = new System.Random((int)DateTime.Now.Ticks);
        //TODO start thread that gets the resource and applies it to the display
        switch (disp.Type)
        {
            case Display.DisplayType.MeshDisplay:
                disp.ApplyResource(Instantiate(TestMesh[rng.Next(2)]));
                break;
            case Display.DisplayType.ImageDisplay:
                disp.ApplyResource(TestTextures[currIdx++ % TestTextSize]);
                break;
        }
    }

    void AddToRoomManagmentUnit(uint roomID,GameObject objToAdd)
    {
        RoomManagmentUnit rManage = _MusemControllerInstance.GetRoomManagmentUnitForRoom(roomID);

        if (rManage != null)
            rManage.AddGameObject(objToAdd);
        else
            Debug.LogError($"Could Not find associated room in mamangment units for: {objToAdd.name}");
    }

    private void OnDrawGizmos()
    {
        if(virtMuse != null)
        {

        }
    }
}

