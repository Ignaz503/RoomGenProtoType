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

    static Dictionary<RoomType, IWallValidator> wallValidators = new Dictionary<RoomType, IWallValidator>()
    {
        {RoomType.Normal, new NormalRoomWallValidator() },
        {RoomType.Long, new LongRoomWallValidator() },
        {RoomType.L, new LRoomWallVaildator() },
        {RoomType.Big, new BigRoomWallValidator() },

    };

    #endregion

    Museum virtMuse;

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
        this.virtMuse = virtMuse;
        CenterDisplayInfos = new MuseumDisplayInfo[Tiles.Count];
        for(int i = 0; i < Tiles.Count; i++)
        {
            CenterDisplayInfos[i] = new MuseumDisplayInfo
            {
                PositionModifier = Vector2.right
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

            float uROneD = uR.x + (uR.y * virtMuse.Size);
            float uLOneD = uL.x + (uL.y * virtMuse.Size);
            float lROneD = lR.x + (lR.y * virtMuse.Size);
            float lLOneD = lL.x + (lL.y * virtMuse.Size);

            Wall upper = new Wall(
                    Wall.WallType.Solid, //type
                    new Vector2[] { uR, uL },// location of corner points museum coords
                    1f,
                    tile, //associated tile
                    Wall.WallRotation.Horizontal,
                    virtMuse
                    );
            tempWalls.Add(upper);
            Wall lower = new Wall(
                Wall.WallType.Solid,
                 new Vector2[] { lR, lL },
                 -1f,
                 tile,
                Wall.WallRotation.Horizontal,
                virtMuse
                );
            tempWalls.Add(lower);
            Wall right = new Wall(
                Wall.WallType.Solid,
                new Vector2[] { uR, lR }, 
                1f,
                tile,
                Wall.WallRotation.Vertical,
                virtMuse
                );
            tempWalls.Add(right);
            Wall left =new Wall
                (
                Wall.WallType.Solid,
                new Vector2[] { uL, lL },
                -1f,
                tile,
                Wall.WallRotation.Vertical,
                virtMuse
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
            Walls.Add(virtMuse.AddWall(w));
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
            float first1D = firstPoint.x + (firstPoint.y * virtMuse.Size);
            float second1D = secondPoint.x + (secondPoint.y * virtMuse.Size);

            uint firstCount = AppearanceCount[first1D];
            uint secondCount = AppearanceCount[second1D];

            //swap
            if (secondCount > firstCount)
            {
                uint tmp = firstCount;
                firstCount = secondCount;
                secondCount = tmp;
            }

            if (wallValidators.ContainsKey(Type))
            {
                if(wallValidators[Type].WallNeedsRemoval(firstCount,secondCount))
                {
                    tempWalls.RemoveAt(i);
                    i--;
                }
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
                    r1.virtMuse.Walls[w1Idx].ChangeWallType(Wall.WallType.Door);
                    r2.virtMuse.Walls[w2Idx].ChangeWallType(Wall.WallType.Door);
                    addedOneDoor = true;
                    break;
                }
            }
            if (addedOneDoor)
                break;
        }
        return addedOneDoor;
    }

    public static void AddNewWallValidator(RoomType t, IWallValidator val, bool overrideExisting = false)
    {
        if (wallValidators.ContainsKey(t) && !overrideExisting)
            throw new Exception("There already exits a wall validator for this type");
        else if (wallValidators.ContainsKey(t))
            wallValidators[t] = val;
        else
            wallValidators.Add(t, val);
    }

}
