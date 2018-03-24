using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[DataContract]
public class Wall
{
    public enum WallType
    {
        Door,
        Solid
    }

    public enum WallRotation
    {
        Vertical,
        Horizontal
    }

    Museum VirtMuse;

    [DataMember]
    public WallType Type { get; protected set; }
    [DataMember]
    public Vector2[] Location { get; protected set; }
    [DataMember]
    public Vector2Int Tile { get; protected set; }
    [DataMember]
    public WallRotation Rotation { get; protected set; }
    [DataMember]
    public List<MuseumDisplayInfo> DisplayInfos { get; protected set; }

    public Wall(WallType t, Vector2[] location, Vector2Int associatedTile, WallRotation rot, Museum virt)
    {
        VirtMuse = virt;
        Type = t;
        Location = location;
        Tile = associatedTile;
        Rotation = rot;

        DisplayInfos = new List<MuseumDisplayInfo>(Type == WallType.Solid ? 4:0);
    }

    public override bool Equals(object obj)
    {
        if (!(obj is Wall))
            return false;
        else
        {
            Wall to_check = obj as Wall;

            int i = 0;
            foreach (Vector2 loc in Location)
            {
                foreach (Vector2 lo in to_check.Location)
                {
                    if (loc.x == lo.x && loc.y == lo.y)
                        i++;
                }
            }

            if (i >= 2)
                return true;
            else
                return false;
        }
    }

    public override int GetHashCode()
    {
        int hash1 = (int)(((((79 + Location[0].x) * 41) + Location[0].y) * 67) + ((int)Location[0].x << (int)Location[0].y)) * 29;
        int hash2 = (int)(((((79 + Location[1].x) * 41) + Location[1].y) * 67) + ((int)Location[1].x << (int)Location[1].y)) * 29;
        return hash1 ^ hash2;
    }

    [Obsolete]
    public bool ContainsPoint(Vector2 p)
    {
        foreach (Vector2 point in Location)
        {
            if (point.x == p.x && point.y == p.y)
                return true;
        }
        return false;
    }

    public static bool operator ==(Wall lhs, Wall rhs)
    {
        int i = 0;
        foreach(Vector2 loc in lhs.Location)
        {
            foreach(Vector2 lo in rhs.Location)
            {
                if (loc.x == lo.x && loc.y == lo.y)
                    i++;
            }
        }

        if (i >= 2)
            return true;
        else
            return false;
    }

    public static bool operator !=(Wall lhs, Wall rhs)
    {
        return !(lhs == rhs);
    }

    /// <summary>
    /// Adds a new display to this wall if possible
    /// and sets the local position of this display correctly
    /// </summary>
    /// <param name="displayInfo"></param>
    /// <param name="tile"></param>
    public void AddNewDisplayInfo(Vector2Int tile)
    {
        if (DisplayInfos.Count == DisplayInfos.Capacity)
            throw new System.Exception($"Can only have {DisplayInfos.Capacity} displays on wall of type {Type}");

        AddDisplayToWall(tile);
        AddDisplayToWall(tile,- 1f);

        //weird fix
        //TODO FIX in AddDisplay and remove this afterwards
        if (DisplayInfos.Count > 2)
        {
            switch(Rotation)
            {
                case WallRotation.Vertical:
                    DisplayInfos[2].PositionModifier.x = -DisplayInfos[0].PositionModifier.x;
                    break;
                case WallRotation.Horizontal:
                    DisplayInfos[2].PositionModifier.x = -DisplayInfos[3].PositionModifier.x;
                    DisplayInfos[3].PositionModifier.x = -DisplayInfos[3].PositionModifier.x;
                    break;
            }
        }

    }

    /// <summary>
    /// adds a new display
    /// </summary>
    void AddDisplayToWall(Vector2Int tile, float posModSign =1f)
    {
        float x = Location[0].x - tile.x;

        MuseumDisplayInfo displayInfo = new MuseumDisplayInfo();

        //figure out position

        displayInfo.PositionModifier.y = .25f * posModSign;

        if (x < 0)
            displayInfo.PositionModifier.x = -1;
        else
            displayInfo.PositionModifier.x = 1;

        if ((Tile.x == 0 || Tile.x == VirtMuse.Size - 1) && Rotation == WallRotation.Vertical)
            displayInfo.PositionModifier.x = -displayInfo.PositionModifier.x;

        if (Tile.y == 0 && Rotation == WallRotation.Horizontal)
            displayInfo.PositionModifier.x = -displayInfo.PositionModifier.x;

        DisplayInfos.Add(displayInfo);
    }

    /// <summary>
    /// Changes the type of the wall to t
    /// ensures that displayInfos are kept correctly
    /// </summary>
    /// <param name="t"></param>
    public void ChangeWallType(WallType t)
    {
        Type = t;
        if (t == WallType.Door)
        {
            DisplayInfos.Clear();
            DisplayInfos.Capacity = 0;
        }
        else
            DisplayInfos.Capacity = 4;

    }

}



