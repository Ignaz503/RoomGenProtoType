using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public enum CornerIndex
{
    UpperRight,
    UpperLeft,
    LowerRight,
    LowerLeft
}

public enum RoomType
{
    Normal,
    Long,
    L,
    Big,
}
[DataContract]
public class Room
{
    #region Static Memebers
    static readonly Vector2[] cornerOffsets = new Vector2[]
    {
        new Vector2(.5f, .5f),//upper right
        new Vector2(-.5f, .5f),// upper left
        new Vector2(.5f, -.5f),// lower right
        new Vector2(-.5f, -.5f)//lower left
    };
    #endregion

    Museum VirtMuse;

    [DataMember]
    public RoomType Type { get; protected set; }
    [DataMember]
    public List<Vector2Int> RoomTiles { get; protected set; }
    [DataMember]
    public List<int> Walls;
    [DataMember]
    public MuseumDisplayInfo[] CenterDisplayInfos { get; protected set; }

    public Room(RoomType t, List<Vector2Int> Tiles, Museum virtMuse)
    {
        Type = t;
        RoomTiles = Tiles;
        VirtMuse = virtMuse;
        CenterDisplayInfos = new MuseumDisplayInfo[Tiles.Count];
        for(int i = 0; i < Tiles.Count; i++)
        {
            CenterDisplayInfos[i] = new MuseumDisplayInfo
            {
                LocalPosition = 1f
            };
        }

        Walls = new List<int>();
        FigureOutWallsForRoom();
    }

    /// <summary>
    /// Creates the correct walls for this room
    /// and ensures that the museum knows about them
    /// </summary>
    void FigureOutWallsForRoom()
    {
        List<Wall> tempWalls = new List<Wall>();
        Dictionary<float, uint> CornerPointAppearanceCount = new Dictionary<float, uint>();
        foreach (Vector2Int tile in RoomTiles)
        {

            Vector2 uR = tile + cornerOffsets[(int)CornerIndex.UpperRight];// upper right
            Vector2 uL = tile + cornerOffsets[(int)CornerIndex.UpperLeft];// upper left
            Vector2 lR = tile + cornerOffsets[(int)CornerIndex.LowerRight];// lower right
            Vector2 lL = tile + cornerOffsets[(int)CornerIndex.LowerLeft];// lower left

            float uROneD = uR.x + (uR.y * VirtMuse.Size);
            float uLOneD = uL.x + (uL.y * VirtMuse.Size);
            float lROneD = lR.x + (lR.y * VirtMuse.Size);
            float lLOneD = lL.x + (lL.y * VirtMuse.Size);

            Wall upper = new Wall(
                    Wall.WallType.Solid, //type
                    new Vector2[] { uR, uL },// location of corner points museum coords
                    tile, //associated tile
                    Wall.WallRotation.Horizontal,
                    VirtMuse
                    );
            tempWalls.Add(upper);
            Wall lower = new Wall(
                Wall.WallType.Solid,
                 new Vector2[] { lR, lL },
                 tile,
                Wall.WallRotation.Horizontal,
                VirtMuse
                );
            tempWalls.Add(lower);
            Wall right = new Wall(
                Wall.WallType.Solid,
                new Vector2[] { uR, lR }, 
                tile,
                Wall.WallRotation.Vertical,
                VirtMuse
                );
            tempWalls.Add(right);
            Wall left =new Wall
                (
                Wall.WallType.Solid,
                new Vector2[] { uL, lL },
                tile,
                Wall.WallRotation.Vertical,
                VirtMuse
                );
            tempWalls.Add(left);

            #region apperance count counting
            if (CornerPointAppearanceCount.ContainsKey(uROneD))
                CornerPointAppearanceCount[uROneD]++;
            else
                CornerPointAppearanceCount.Add(uROneD, 1);

            if (CornerPointAppearanceCount.ContainsKey(uLOneD))
                CornerPointAppearanceCount[uLOneD]++;
            else
                CornerPointAppearanceCount.Add(uLOneD, 1);

            if (CornerPointAppearanceCount.ContainsKey(lROneD))
                CornerPointAppearanceCount[lROneD]++;
            else
                CornerPointAppearanceCount.Add(lROneD, 1);

            if (CornerPointAppearanceCount.ContainsKey(lLOneD))
                CornerPointAppearanceCount[lLOneD]++;
            else
                CornerPointAppearanceCount.Add(lLOneD, 1);
            #endregion
        }
        RemoveUnnecessaryWalls(CornerPointAppearanceCount,tempWalls);

        // add walls to museum
        AddWallsToMuseum(tempWalls);

    }

    /// <summary>
    /// Adds the walls this room has to the walls container in the museum
    /// </summary>
    /// <param name="tempWalls"></param>
    private void AddWallsToMuseum(List<Wall> tempWalls)
    {
        // goes thorugh temp walls and adds to museum
        // stores index of wall in museum walls list into walls list
        foreach(Wall w in tempWalls)
        {
            Walls.Add(VirtMuse.AddWall(w));
        }
    }

    /// <summary>
    /// handles the removal of unncesarryly created walls in figure out walls
    /// removal is dependent on roomtype
    /// </summary>
    void RemoveUnnecessaryWalls(Dictionary<float, uint> AppearanceCount, List<Wall> tempWalls)
    {
        for (int i = 0; i < tempWalls.Count; i++)
        {
            Vector2 firstPoint = tempWalls[i].Location[0];
            Vector2 secondPoint = tempWalls[i].Location[1];
            float first1D = firstPoint.x + (firstPoint.y * VirtMuse.Size);
            float second1D = secondPoint.x + (secondPoint.y * VirtMuse.Size);

            uint firstCount = AppearanceCount[first1D];
            uint secondCount = AppearanceCount[second1D];

            //swap
            if (secondCount > firstCount)
            {
                uint tmp = firstCount;
                firstCount = secondCount;
                secondCount = tmp;
            }

            switch (Type)
            {
                case RoomType.Normal:
                    // do nothig 
                    break;
                case RoomType.Long:
                    if (firstCount > 1 && secondCount > 1)
                    {
                        tempWalls.RemoveAt(i);
                        i--;
                    }
                    break;
                case RoomType.L:
                    {
                        if (firstCount > 2 && secondCount > 1)
                        {
                            tempWalls.RemoveAt(i);
                            i--;
                        }
                    }
                    break;
                case RoomType.Big:
                    if (firstCount > 2)
                    {
                        tempWalls.RemoveAt(i);
                        i--;
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// Creats a single door between two rooms
    /// </summary>
    /// <returns>ture if a door was created false otherwise</returns>
    public static bool CreateDoorBetweenRooms(Room r1, Room r2)
    {
        bool addedOneDoor = false;
        foreach (int w1Idx in r1.Walls)
        {
            foreach (int w2Idx in r2.Walls)
            {
                if (w1Idx == w2Idx)
                {
                    r1.VirtMuse.Walls[w1Idx].ChangeWallType(Wall.WallType.Door);
                    r2.VirtMuse.Walls[w2Idx].ChangeWallType(Wall.WallType.Door);
                    addedOneDoor = true;
                    break;
                }
            }
            if (addedOneDoor)
                break;
        }
        return addedOneDoor;
    }
}
