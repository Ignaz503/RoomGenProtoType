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


    public GameObject Player;

    #endregion


    public float ImageDisplayYPosSolidWall;

    public MeshFilter[] TestMesh;
    public Sprite TestSprite;

    public float XPosScale;
    public float ZPosScale;
    float displayXPosScale;
    

    Museum virtMuse = null;

    public bool DisableAfterRequest;

    private void Start()
    {
        XPosScale = FloorPrefab.transform.localScale.x * 10f;
        ZPosScale = FloorPrefab.transform.localScale.z * 10f;

        Transform glassSphere = MeshDisplayPrefab.transform.GetChild(MeshDisplayPrefab.transform.childCount - 1);
        Debug.Log(glassSphere.gameObject.name);

        displayXPosScale = (1.5f*glassSphere.localScale.x ) / 0.24f; // 0.24 is local scale of wall objects

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
                MeshRenderer rend = floor.GetComponent<MeshRenderer>();

                floors.Add(floor);

                //TEMP
                //TODO REMOVE
                switch (r.Type)
                {
                    case RoomType.Normal:
                        rend.material.color = Color.yellow;
                        break;
                    case RoomType.Long:
                        rend.material.color = Color.red;
                        break;
                    case RoomType.Big:
                        rend.material.color = Color.green;
                        break;
                    case RoomType.L:
                        rend.material.color = Color.blue;
                        break;
                }


                Vector3 pos = new Vector3(XPosScale * tile.x, 0, ZPosScale * tile.y);

                floor.transform.position = pos;
                floor.name = r.Type.ToString() + " " + tile.ToString();
                roomFloors.Add(floor);

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
            Vector3 pos = new Vector3(XPosScale * w.Location[0].x, 5, ZPosScale * w.Location[0].y);

            if (w.Rotation == Wall.WallRotation.Vertical)
                pos.z -= (ZPosScale * .5f);
            else if (w.Rotation == Wall.WallRotation.Horizontal)
                pos.x -= (XPosScale * .5f);

            GameObject wallObj = w.Type == Wall.WallType.Solid ? Instantiate(WallPrefab) : Instantiate(WallWithDoorPrefab);

            wallObj.transform.position = pos;

            Vector3 newRot = new Vector3(0f, 90f * (int)w.Rotation, 0);

            wallObj.transform.eulerAngles = newRot;

            wallObj.name = w.Rotation.ToString() + " " + w.Tile.ToString();

            GameObject parent = floors.Where((obj) => { return obj.name.Contains(w.Tile.ToString()); }).First();
            if (parent != null)
                wallObj.transform.SetParent(parent.transform);
            #endregion

            if(w.Type == Wall.WallType.Solid)
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
                Instantiate(CenterImageDisplayPrefab) : Instantiate(MeshDisplayPrefab);

            Display display = disp.GetComponentInChildren<Display>();

            if (display != null)
                LoadResource(display, "test");

            disp.name = "Display" + r.RoomTiles[0];
            disp.transform.SetParent(associatedFloorGameobjects[i].transform);
            disp.transform.localPosition = Vector3.zero;// * dispInf.LocalPosition;
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
            //TODO: for display on both sides
            GameObject disp = (dispInf.Type == Display.DisplayType.ImageDisplay) ?
                Instantiate(WallImageDisplayPrefab) : Instantiate(MeshDisplayPrefab);

            disp.transform.SetParent(wallObj.transform);

            Display display = disp.GetComponentInChildren<Display>();

            if (display != null)
                LoadResource(display, "test");

            if (display is MeshDisplay)
            {
                float xLocalPos = displayXPosScale * ((w.Type == Wall.WallType.Solid) ? wallObj.transform.localScale.x : (1f - wallObj.transform.localScale.x));

                xLocalPos += (w.Type == Wall.WallType.Solid)? 1f:0f;
                float yLocalPos = (w.Type == Wall.WallType.Solid) ? -.5f : -1f;

                disp.transform.localPosition = new Vector3(xLocalPos * dispInf.PositionModifier.x, yLocalPos, dispInf.PositionModifier.y);
            }
            else
            {
                //wall image display
                //currently with poss for door displays can be simplified if decided on no door displays#
                // which is my current preference
                float xLocPos = (w.Type == Wall.WallType.Solid) ? .55f * dispInf.PositionModifier.x : dispInf.PositionModifier.x;
                if (xLocPos > 0 && w.Type == Wall.WallType.Door)
                    xLocPos += 0.02f;

                float yLocPos = (w.Type == Wall.WallType.Solid) ? ImageDisplayYPosSolidWall : -0.6f; 
                disp.transform.localPosition = new Vector3(xLocPos, yLocPos, dispInf.PositionModifier.y);
                disp.transform.localEulerAngles = new Vector3(0, 90f, 0);
                float scale = 0.2f * ((w.Type == Wall.WallType.Solid)? 1f:2f);
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

