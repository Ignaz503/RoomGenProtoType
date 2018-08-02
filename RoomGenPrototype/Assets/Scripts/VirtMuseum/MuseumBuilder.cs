using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MuseumBuilder : MonoBehaviour
{
    /// <summary>
    /// instance of the builder
    /// </summary>
    public static MuseumBuilder Instance;

    /// <summary>
    /// temp queue for testing
    /// </summary>
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

    public List<Texture2D> texturePrefabs;

    #endregion

    /// <summary>
    /// temp dictionary containing already gnerated wall texture
    /// to speed up start up
    /// </summary>
    Dictionary<string, Texture2D> existingWallTexture = new Dictionary<string, Texture2D>();

    /// <summary>
    /// temp test meshes
    /// </summary>
    public MeshFilter[] TestMesh;
    /// <summary>
    /// temp mesh textures
    /// </summary>
    public Texture2D[] TestTextures;

    /// <summary>
    /// scaling of position in x direction
    /// </summary>
    public float FloorXPosScale;
    /// <summary>
    /// caling of floor position in z direction
    /// </summary>
    public float FloorZPosScale;

    /// <summary>
    /// height of walls in the msuem
    /// </summary>
    float wallHeight;

    //Temp
    int TestTextSize;
    //temp
    int currIdx = 0;

    /// <summary>
    /// the museum being built
    /// </summary>
    Museum virtMuse = null;

    /// <summary>
    /// temp
    /// </summary>
    public bool DisableAfterRequest;

    /// <summary>
    /// instance of the museum controller
    /// </summary>
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
                //GameObject ceiling = Instantiate(CeilingPrefab);
                //TODO REMOVE
                //ceiling.SetActive(false);
                AddToRoomManagmentUnit(r.RoomID, floor);
                //AddToRoomManagmentUnit(r.RoomID, ceiling);

                //MeshRenderer rend = floor.GetComponent<MeshRenderer>();
                //MeshRenderer ceilRend = ceiling.GetComponent<MeshRenderer>();
                floors.Add(floor);

                //TEMP
                //TODO REMOVE
                //switch (r.Type)
                //{
                //    case RoomType.Normal:
                //        rend.material.color = Color.yellow;
                //        ceilRend.material.color = Color.yellow;
                //        break;
                //    case RoomType.Long:
                //        rend.material.color = Color.red;
                //        ceilRend.material.color = Color.red;
                //        break;
                //    case RoomType.Big:
                //        rend.material.color = Color.green;
                //        ceilRend.material.color = Color.green;
                //        break;
                //    case RoomType.L:
                //        rend.material.color = Color.blue;
                //        ceilRend.material.color = Color.blue;
                //        break;
                //}

                Vector3 pos = new Vector3(FloorXPosScale * tile.x, FloorPrefab.transform.position.y, FloorZPosScale * tile.y);

                string name = r.RoomID + " " + r.Type.ToString() + " " + tile.ToString();

                floor.transform.position = pos;
                floor.name = name;
                roomFloors.Add(floor);

                //ceiling.name = name + "  ceiling";
                //pos.y += wallHeight;
                //ceiling.transform.position = pos;

                //load floor texture#
                //TODO UNCOMMENT WHEN TESTING RESOURCE LOADER
                //ResourceLoader.Instance.RequestResource(
                //    res =>
                //    {
                //        res.res.ApplyToGameobject(floor);
                //TODO apply PreProcessingGameobject information
                //    },
                //    r.FloorTexture.AssociatedResourceLocators,
                //    typeof(TextureResource).ToString()
                //    );
                //ResourceLoader.Instance.RequestResource(
                //    res =>
                //    {
                //        res.res.ApplyToGameobject(ceiling);
                //TODO apply PreProcessingGameobject information
                //    },
                //    r.CeilingTexture.AssociatedResourceLocators,
                //    typeof(TextureResource).ToString()
                //    );
                TEMPApplyTextureToNonWalls(r.FloorTexture.AssociatedResourceLocators, floor);
                //TEMPApplyTextureToNonWalls(r.CeilingTexture.AssociatedResourceLocators, ceiling);
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

            wallObj.name =  w.WallID + " " + w.Rotation.ToString() + " " + w.Tiles[0].ToString();

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

            TEMPApplyTextureToWall(w, wallObj);
            //request wall texture
            //MeshRenderer re = wallObj.GetComponent<MeshRenderer>();
            //ResourceLoader.Instance.RequestResource(
            //    res =>{
            //        //TODO CHECK IF RES IS TEXTURE RESOURCE
            //        TextureResource texRes = res.res as TextureResource;
            //        if(re.material.mainTexture != null && re.material.mainTexture.width == 2 * texRes.Image.width)
            //        {
            //            Texture2D tex = new Texture2D(re.material.mainTexture.width,re.material.mainTexture.height);

            //            tex.SetPixels((re.material.mainTexture as Texture2D).GetPixels(0,0,re.material.mainTexture.width,re.material.mainTexture.height));
            //            //texture already big enough set texture depending on position modifier
            //            //start from 0 if pos mod 0 else from half width
            //            int xStart = (int)(re.material.mainTexture.width * (w.TextureInfos[0].PositionModifier * 0.5f));
            //            //loop to end if pos mod is 1 else to halfe width
            //            int xEnd = (int)(re.material.mainTexture.width * (w.TextureInfos[0].PositionModifier > 0 ? 1f : 0.5f));
            //            //TODO CHECK IF xEND calc needs +1 if only to half width
            //            //Probably not
            //            for(int x = xStart;x < xEnd; x++)
            //            {
            //                for(int y = 0; y < texRes.Image.height; y++)
            //                {
            //                    //TODO set texture pixels of new texture
            //                    tex.SetPixel(x, y, tex.GetPixel(x - xStart, y));
            //                }
            //            }
            //        }
            //    },
            //    w.TextureInfos[0].AssociatedResourceLocators,
            //    typeof(TextureResource).ToString()
            //    );
            //TODO TEXTURE LOADING FOR associated wall texutre [1]

        }
    }

    /// <summary>
    /// Creates the center displays for a room
    /// List used for correct parenting of gamobject
    /// </summary>
    void SetUpCenterDisplaysForRoom(Room r, List<GameObject> associatedFloorGameobjects)
    {
        //used to get associated floor gamobjects
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
                LoadDisplayResource(display, "test");
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
                LoadDisplayResource(display, "test");
        }
    }

    /// <summary>
    /// TEMPORARY IMPLEMENTATION
    /// Loads Resource for a dsiplay 
    /// </summary>
    void LoadDisplayResource(Display objInNeedOfResource, string resourceLocator)
    {
        System.Random rng = new System.Random((int)DateTime.Now.Ticks);

        switch (objInNeedOfResource.Type)
        {
            case Display.DisplayType.MeshDisplay:
                int rngNum = rng.Next(2);
                GameObject obj = Instantiate(TestMesh[rngNum].gameObject);
                Mesh mesh = obj.GetComponent<MeshFilter>().sharedMesh;
                Material mat = obj.GetComponent<MeshRenderer>().material;
                objInNeedOfResource.ApplyResource(new DisplayMeshResource(mesh, mat));
                Destroy(obj);
                break;
            case Display.DisplayType.ImageDisplay:
                objInNeedOfResource.ApplyResource(new DisplayImageResource(TestTextures[currIdx++ % TestTextSize]));
                break;
        }
        //actual implementation
        //ResourceLoader.Instance.RequestResource(
        //    res =>
        //    {
        //        if (res.res is BaseDisplayResource)
        //        {
        //            res.res.ApplyToGameobject(objInNeedOfResource.gameObject);
        //            objInNeedOfResource.ApplyResource(res.res as BaseDisplayResource);
        //        }
        //        else
        //        {
        //            throw new Exception("Trying to assign non display resource to display");
        //        }
        //    },
        //    resourceLocator,
        //    objInNeedOfResource.Type.ToString()
        //    );
    }

    /// <summary>
    /// adds a room gamobject to a room managment unit
    /// </summary>
    /// <param name="roomID">the room id this gameobject is associated with</param>
    /// <param name="objToAdd">the object to add</param>
    void AddToRoomManagmentUnit(uint roomID,GameObject objToAdd)
    {
        RoomManagmentUnit rManage = _MusemControllerInstance.GetRoomManagmentUnitForRoom(roomID);

        if (rManage != null)
            rManage.AddGameObject(objToAdd);
        else
            Debug.LogError($"Could Not find associated room in mamangment units for: {objToAdd.name}");
    }

    /// <summary>
    /// TEMPORARY function to showcase room styles
    /// returns first hit in prefab list with same name as param name
    /// </summary>
    /// <param name="name">the name of the texture</param>
    /// <returns>the texutre with the name</returns>
    Texture2D GetTexture(string name)
    {
        Texture2D te = texturePrefabs.First((t) => { return t.name == name; });
        if(te == null)
        {
            throw new Exception("somthing wrong texture man");
        }
        return te;
    }

    /// <summary>
    /// TEMPORAARY function that is used to showcase room styles
    /// applies a texture to this gamobject
    /// </summary>
    /// <param name="name">name of the texture</param>
    /// <param name="obj">gmaobject to apply to</param>
    void TEMPApplyTextureToNonWalls(string name, GameObject obj)
    {
        MeshRenderer re = obj.GetComponent<MeshRenderer>();

        re.material.mainTexture = GetTexture(name);
    }

    /// <summary>
    /// TEMPORARY function to showcase room styles
    /// </summary>
    /// <param name="w">the wall for which the texure is</param>
    /// <param name="obj">gamobject of this wall</param>
    void TEMPApplyTextureToWall(Wall w, GameObject obj)
    {
        MeshRenderer re = obj.GetComponent<MeshRenderer>();
        if (w.TextureInfos.Count > 1)
        {
            float sign = w.Rotation == Wall.WallRotation.Vertical ? Mathf.Sign(w.Tiles[0].x - w.Tiles[1].x) : Mathf.Sign(w.Tiles[0].y - w.Tiles[1].y);
            if (!existingWallTexture.ContainsKey(w.TextureInfos[0].AssociatedResourceLocators + w.TextureInfos[1].AssociatedResourceLocators + w.Rotation + sign))
            {
                //different wall textures
                Texture2D[] textures = new Texture2D[2];
                textures[w.TextureInfos[0].PositionModifier] = GetTexture(w.TextureInfos[0].AssociatedResourceLocators);
                textures[w.TextureInfos[1].PositionModifier] = GetTexture(w.TextureInfos[1].AssociatedResourceLocators);

                Texture2D tex = new Texture2D(2 * textures[0].width, textures[0].height);

                for (int x = 0; x < tex.width/2; x++)
                {
                    for(int y = 0; y < tex.height; y++)
                    {
                        Color t1 = textures[0].GetPixel(x, y);
                        Color t2 = textures[1].GetPixel(x, y);

                        tex.SetPixel(x, y, t1);
                        tex.SetPixel(x + (tex.width / 2), y, t2);
                    }
                }

                tex.Apply();
                existingWallTexture.Add(w.TextureInfos[0].AssociatedResourceLocators + w.TextureInfos[1].AssociatedResourceLocators + w.Rotation + sign, tex);
                re.material.mainTexture = tex;
            }
            else
            {
                re.material.mainTexture = existingWallTexture[w.TextureInfos[0].AssociatedResourceLocators + w.TextureInfos[1].AssociatedResourceLocators+w.Rotation + sign];
            }
        }
        else
        {
            //same texture
            re.material.mainTexture = GetTexture(w.TextureInfos[0].AssociatedResourceLocators);
        }
    }

}

