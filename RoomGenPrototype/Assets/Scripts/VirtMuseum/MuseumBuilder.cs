﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MuseumBuilder : MonoBehaviour
{
    public static Queue<string> MuseumData = new Queue<string>();

    #region Prefabs
    public GameObject FloorPrefab;
    public GameObject WallPrefab;
    public GameObject WallWithDoorPrefab;

    public GameObject CenterImageDisplayPrefab;
    public GameObject MeshDisplayPrefab;
    public GameObject WallImageDisplayPrefab;

    public GameObject CeilingPrefab;

    public GameObject Player;

    #endregion

    public float ImageDisplayYPosSolidWall;

    public MeshFilter[] TestMesh;
    public Sprite TestSprite;

    public float XPosScale;
    public float ZPosScale;
    float displayXPosScale;
    float MeshDisplayXPosModifier;

    float wallHeight;

    Museum virtMuse = null;

    public bool DisableAfterRequest;

    private void Start()
    {
        XPosScale = FloorPrefab.transform.localScale.x;
        ZPosScale = FloorPrefab.transform.localScale.z;
        wallHeight = WallPrefab.transform.localScale.y + (.5f*CeilingPrefab.transform.localScale.y);

        #region MeshDisplayXPosModificiation
        Transform meshDisplayTrans  = MeshDisplayPrefab.transform.GetChild(MeshDisplayPrefab.transform.childCount - 1);

        float avg = (meshDisplayTrans.localScale.x + meshDisplayTrans.localScale.y + meshDisplayTrans.localScale.z) / 3f;

        MeshDisplayXPosModifier = avg * .5f;

        Debug.Log(MeshDisplayXPosModifier);
        #endregion

        Transform glassSphere = MeshDisplayPrefab.transform.GetChild(MeshDisplayPrefab.transform.childCount - 1);
        displayXPosScale = (1.5f * glassSphere.localScale.x) / (WallPrefab.transform.localScale.x / FloorPrefab.transform.localScale.x);

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
                List<GameObject> floors = new List<GameObject>();
                SetUpFloor(floors);
                SetUpWalls(floors);

                Player.transform.position = new Vector3(virtMuse.Rooms[0].RoomTiles[0].x * 10f, 4f, virtMuse.Rooms[0].RoomTiles[0].y * 10f);
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


                Vector3 pos = new Vector3(XPosScale * tile.x, FloorPrefab.transform.position.y, ZPosScale * tile.y);

                string name = r.Type.ToString() + " " + tile.ToString();

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

            GameObject parent = floors.Where((obj) => { return obj.name.Contains(w.Tiles[0].ToString()); }).First();
            if (parent != null)
                wallObj.transform.SetParent(parent.transform);

            float yPos = (w.Type == Wall.WallType.Solid ? wallObj.transform.localScale.y * .5f : wallObj.transform.localScale.y)+.5f;

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

            wallObj.name = w.Rotation.ToString() + " " + w.Tiles[0].ToString();

            #endregion

            if (w.Type == Wall.WallType.Solid)
                SetUpDisplaysForWall(w, wallObj);

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
                Instantiate(MeshDisplayPrefab);

            Display display = disp.GetComponentInChildren<Display>();

            if (display != null)
                LoadResource(display, "test");

            disp.name = "Display" + r.RoomTiles[0];
            disp.transform.SetParent(associatedFloorGameobjects[i].transform);
            disp.transform.localPosition = Vector3.zero;
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

            disp.transform.SetParent(wallObj.transform);

            Display display = disp.GetComponentInChildren<Display>();

            if (display != null)
                LoadResource(display, "test");

            if (display is MeshDisplay)
            {
                float xLocalPos = (displayXPosScale * wallObj.transform.localScale.x) + MeshDisplayXPosModifier;

                float yLocalPos = -.5f;

                disp.transform.localPosition = new Vector3(xLocalPos * dispInf.PositionModifier.x, yLocalPos, dispInf.PositionModifier.y);
            }
            else
            {
                //wall image display
                float xLocPos = .55f * dispInf.PositionModifier.x;

                disp.transform.localPosition = new Vector3(xLocPos, ImageDisplayYPosSolidWall, dispInf.PositionModifier.y);
                disp.transform.localEulerAngles = new Vector3(0, 90f, 0);
                float scale = 0.2f;
                disp.transform.localScale = new Vector3(scale*.75f, scale, 1);
            }
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
                disp.ApplyResource(TestSprite);
                break;
        }

    }
}

