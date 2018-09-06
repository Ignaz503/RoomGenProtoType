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

    /// <summary>
    /// the museum being built
    /// </summary>
    public Museum VirtMuse;
    
    /// <summary>
    /// temp
    /// </summary>
    public bool DisableAfterRequest;

    /// <summary>
    /// instance of the museum controller
    /// </summary>
    MuseumController _MusemControllerInstance;

    Dictionary<int, GameObject> oneDTileCoordToFloorGameObjectMapping;

    /// <summary>
    /// event invoked when the builder received an museum to build
    /// </summary>
    public event Action<Museum> OnMuseumGotten;

    /// <summary>
    ///  event invoked when a wall was built
    /// </summary>
    public event Action<Room> OnRoomBuilt;

    /// <summary>
    /// event invoked when wall was built 
    /// </summary>
    public event Action<Wall> OnWallBuilt;

    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogError("There already exists a museum builder");
            Destroy(this);
            return;
        }
        
        Instance = this;

        oneDTileCoordToFloorGameObjectMapping = new Dictionary<int, GameObject>();

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
        //MuseumGenerator.Instance.RequestNewMuseum((new MuseumRequestData()
        //{
        //    MuseumType = "TestMuseum",
        //    Size = MuseumSize.Large
        //}).Serialize(),MuseumData);
        //if (DisableAfterRequest)
        //    gameObject.SetActive(false);
        VirtMuse = ResourceLoader.Instance.MuseumToBuild;
        OnMuseumGotten(VirtMuse);
        StartCoroutine(BuildMuseum());
    }

    private void Update()
    {
    }

    /// <summary>
    /// Sets player position to the first floor in the museum.
    /// </summary>
    public void SetToStartPosition(Transform t)
    {
        GameObject firstRoomFloorObj = oneDTileCoordToFloorGameObjectMapping[VirtMuse.TransformTileCoordIntoOneD(VirtMuse.Rooms[0].RoomTiles[0])];
        Transform cParent = t.parent;
        t.SetParent(firstRoomFloorObj.transform);
        t.localPosition = new Vector3(.35f, .25f, 0);
        t.SetParent(cParent);
        t.gameObject.SetActive(true);//????
        //gameObject.SetActive(false);
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
            GameObject disp = ((int)dispInf.Type == (int)Display.DisplayType.ImageDisplay) ?
                Instantiate(CenterImageDisplayPrefab) :
                Instantiate(CenterMeshDisplay);

            disp.name = "Display " + r.RoomID + " " + r.RoomTiles[0];

            AddToRoomManagmentUnit(dispInf.AssociatedRoomID, disp);

            Display display = disp.GetComponentInChildren<Display>();

            display.SetUp(dispInf, associatedFloorGameobjects[i]);

            if (display != null)
                LoadDisplayResource(display, dispInf.AssociatedResourceLocator);
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
            GameObject disp = (dispInf.Type == DisplayType.ImageDisplay) ?
                Instantiate(WallImageDisplayPrefab) : Instantiate(MeshDisplayPrefab);
            AddToRoomManagmentUnit(dispInf.AssociatedRoomID, disp);

            Display display = disp.GetComponentInChildren<Display>();
            disp.name = w.Rotation + " " + display.GetType() +" "+ dispInf.AssociatedRoomID;

            display.SetUp(dispInf, wallObj);

            if (display != null)
                LoadDisplayResource(display, dispInf.AssociatedResourceLocator);
        }
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

    public GameObject GetGameObjectForTile(Vector2Int t)
    {
        return oneDTileCoordToFloorGameObjectMapping[VirtMuse.TransformTileCoordIntoOneD(t)];
    }

    #region Build Museum
    
    /// <summary>
    /// Places a single room, and it's dilays
    /// </summary>
    /// <param name="r"></param>
    void PlaceRoom(Room r)
    {
        //create floor
        List<GameObject> roomFloors = new List<GameObject>();

        #region Floor Gameobject setup
        foreach (Vector2Int tile in r.RoomTiles)
        {
            GameObject floor = Instantiate(FloorPrefab);
            GameObject ceiling = Instantiate(CeilingPrefab);
            //TODO REMOVE
            //ceiling.SetActive(false);
            AddToRoomManagmentUnit(r.RoomID, floor);
            AddToRoomManagmentUnit(r.RoomID, ceiling);


            oneDTileCoordToFloorGameObjectMapping.Add(VirtMuse.TransformTileCoordIntoOneD(tile), floor);
            //floors.Add(floor);

            Vector3 pos = new Vector3(FloorXPosScale * tile.x, FloorPrefab.transform.position.y, FloorZPosScale * tile.y);

            string name = r.RoomID + " " + r.Type.ToString() + " " + tile.ToString();

            floor.transform.position = pos;
            floor.name = name;
            roomFloors.Add(floor);

            ceiling.name = name + "  ceiling";
            pos.y += wallHeight;
            ceiling.transform.position = pos;

            PostFloorTextureRequest(r, floor);
            PostCeilingTextureRequest(r, ceiling);
        }
        #endregion

        SetUpCenterDisplaysForRoom(r, roomFloors);
    }

    /// <summary>
    /// places a singe wall
    /// </summary>
    /// <param name="w">the wall to palce</param>
    void PlaceWall(Wall w)
    {
        #region Wall gamobject setup
        GameObject wallObj = w.Type == Wall.WallType.Solid ? Instantiate(WallPrefab) : Instantiate(WallWithDoorPrefab);

        //position parenting
        GameObject parent = oneDTileCoordToFloorGameObjectMapping[VirtMuse.TransformTileCoordIntoOneD(w.Tiles[0])];
        if (parent != null)
            wallObj.transform.SetParent(parent.transform);

        float yPos = (w.Type == Wall.WallType.Solid ? wallObj.transform.localScale.y * .5f : wallObj.transform.localScale.y) + .5f;

        //rotation
        //TODO: Remove fixed values
        if (w.Rotation == Wall.WallRotation.Horizontal)
        {
            wallObj.transform.localPosition = new Vector3(0, yPos, .5f * w.PositionModifier);
        }
        else
        {
            wallObj.transform.localPosition = new Vector3(.5f * w.PositionModifier, yPos, 0);
        }

        Vector3 newRot = new Vector3(0f, 90f * (int)w.Rotation, 0);

        wallObj.transform.eulerAngles = newRot;

        wallObj.name = w.WallID + " " + w.Rotation.ToString() + " " + w.Tiles[0].ToString();

        //name
        if (w.AssociatedRoomIDs.Count < 2)
        {
            wallObj.name += " " + w.AssociatedRoomIDs[0];
        }
        else
        {
            wallObj.name += " " + w.AssociatedRoomIDs[0];
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

        //TEMPApplyTextureToWall(w, wallObj);
        PostWallTextureRequest(w, wallObj);
    }

    /// <summary>
    /// Builds a Museum, over multiple frames(as a coroutine)
    /// </summary>
    /// <param name="M">Museum that needs building</param>
    IEnumerator BuildMuseum()
    {
        foreach(Room r in VirtMuse.Rooms)
        {
            //place one room each frame
            PlaceRoom(r);
            OnRoomBuilt(r);
            yield return null;
        }
        foreach(Wall w in VirtMuse.Walls)
        {
            PlaceWall(w);
            OnWallBuilt(w);
            yield return null;
        }
        //SetPlayerPositionToStart();
        gameObject.SetActive(false);
    }

    #endregion
    
    #region TEMP
    /// <summary>
    /// TEMPORARY function to showcase room styles
    /// returns first hit in prefab list with same name as param name
    /// </summary>
    /// <param name="name">the name of the texture</param>
    /// <returns>the texutre with the name</returns>
    Texture2D GetTexture(string name)
    {
        Texture2D te = texturePrefabs[0];
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
            if (!existingWallTexture.ContainsKey(w.TextureInfos[0].AssociatedResourceLocators.ToString() + w.TextureInfos[1].AssociatedResourceLocators.ToString() + w.Rotation + sign))
            {
                //different wall textures
                Texture2D[] textures = new Texture2D[2];
                textures[w.TextureInfos[0].PositionModifier] = GetTexture(w.TextureInfos[0].AssociatedResourceLocators.ToString());
                textures[w.TextureInfos[1].PositionModifier] = GetTexture(w.TextureInfos[1].AssociatedResourceLocators.ToString());

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
                existingWallTexture.Add(w.TextureInfos[0].AssociatedResourceLocators.ToString() + w.TextureInfos[1].AssociatedResourceLocators.ToString() + w.Rotation + sign, tex);
                re.material.mainTexture = tex;
            }
            else
            {
                //Debug.Log("hi");
                re.material.mainTexture = existingWallTexture[w.TextureInfos[0].AssociatedResourceLocators.ToString() + w.TextureInfos[1].AssociatedResourceLocators.ToString() +w.Rotation + sign];
            }
        }
        else
        {
            //same texture
            re.material.mainTexture = GetTexture(w.TextureInfos[0].AssociatedResourceLocators.ToString());
        }
    }
    #endregion

    /// <summary>
    /// Loads Resource for a dsiplay 
    /// </summary>
    void LoadDisplayResource(Display objInNeedOfResource, int resourceLocator)
    {
        switch (objInNeedOfResource.Type)
        {
            case Display.DisplayType.MeshDisplay:
                PostMeshDisplayRequest(objInNeedOfResource as MeshDisplay, resourceLocator);
                break;
            case Display.DisplayType.ImageDisplay:
                PostImageDisplayRequest(objInNeedOfResource, resourceLocator);
                break;
        }
    }

    /// <summary>
    /// posts a mesh resource request
    /// </summary>
    /// <param name="disp">the display that wants the mesh</param>
    /// <param name="resLoc">the ID of the resource</param>
    void PostMeshDisplayRequest(MeshDisplay disp, int resLoc)
    {
        DisplayResourceRequest<DisplayMeshResource> req = new DisplayResourceRequest<DisplayMeshResource>(resLoc, disp, MeshPreProcessing);
        ResourceLoader.Instance.PostRequest(req, ResourceLoader.RequestType.Other);
    }

    /// <summary>
    /// Posts an image request to the resource loader
    /// </summary>
    /// <param name="disp">the display taht wants the resource</param>
    /// <param name="resourceLocator">ID of resource</param>
    void PostImageDisplayRequest(Display disp, int resourceLocator)
    {
        DisplayResourceRequest<DisplayImageResource> req = new DisplayResourceRequest<DisplayImageResource>(resourceLocator, disp, null);

        ResourceLoader.Instance.PostRequest(req, ResourceLoader.RequestType.Other);
    }

    /// <summary>
    /// posts a wall texture request to resource loader
    /// </summary>
    /// <param name="w">the wall that defines the request</param>
    /// <param name="wallObj">the gamobject the texture should be applied to</param>
    private void PostWallTextureRequest(Wall w, GameObject wallObj)
    {
        IResourceRequest req = null;
        if(w.TextureInfos.Count > 1)
        {
            req = new MultiWallTextureRequest(w, wallObj);
        }
        else
        {
            req = new SingleWallTextureRequest(w.TextureInfos[0].AssociatedResourceLocators, wallObj);
        }
        ResourceLoader.Instance.PostRequest(req, ResourceLoader.RequestType.Other);
    }

    /// <summary>
    /// posts a floor texture request to the resource loader
    /// </summary>
    /// <param name="r">the room that defines the room style</param>
    /// <param name="floor">the gameobjec tthe texture should be applied to</param>
    void PostFloorTextureRequest(Room r, GameObject floor)
    {
        FloorTextureRequest floorReq = new FloorTextureRequest(r.StyleInfo.AssociatedResourceLocators, floor);
        ResourceLoader.Instance.PostRequest(floorReq, ResourceLoader.RequestType.Other);
    }

    /// <summary>
    /// Posts a ceiling texutre requesst to the resource loader
    /// </summary>
    /// <param name="r">the room that defines the style</param>
    /// <param name="ceiling">the gameobject that the texture should be applied to</param>
    void PostCeilingTextureRequest(Room r, GameObject ceiling)
    {
        CeilingTextureRequest ceilReq = new CeilingTextureRequest(r.StyleInfo.AssociatedResourceLocators, ceiling);

        ResourceLoader.Instance.PostRequest(ceilReq, ResourceLoader.RequestType.Other);
    }

    /// <summary>
    /// Preprocessing helper coroutine for mesh display
    /// </summary>
    /// <param name="res">the display resource that needs pre processing</param>
    /// <param name="preObj">the preprocessing object that is filled</param>
    /// <param name="display">the displaay that requested the resource</param>
    IEnumerator MeshPreProcessing(DisplayMeshResource res, PreProcessingGameObjectInformation preObj, Display display)
    {
        MeshDisplay disp = display as MeshDisplay;
        BoundingSphere boundsChild = BoundingSphere.Calculate(res.Mesh.vertices);
        yield return null;
        BoundingSphere boundsParent = BoundingSphere.Calculate(disp.ParentMesh.sharedMesh.vertices);
        float avg = (disp.transform.localScale.x + disp.transform.localScale.y + disp.transform.localScale.z) / 3f;
        float scale = ((boundsParent.radius / boundsChild.radius) * avg);

        preObj.Scale = new Vector3(scale, scale, scale);
    }
}