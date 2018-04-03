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
    float DisplayXPositionModifier = 1f;

    [DataMember]
    public WallType Type { get; protected set; }
    //Used for equaality checking
    public Vector2[] Location { get; protected set; }
    [DataMember]
    public float PositionModifier { get; protected set; }
    [DataMember]
    public List<Vector2Int> Tiles { get; protected set; }
    [DataMember]
    public WallRotation Rotation { get; protected set; }
    [DataMember]
    public List<MuseumDisplayInfo> DisplayInfos { get; protected set; }

    public Wall(WallType t, Vector2[] location,float posMod , Vector2Int associatedTile, WallRotation rot, Museum virt)
    {
        VirtMuse = virt;
        Type = t;
        Location = location;
        PositionModifier = posMod; 
        Tiles = new List<Vector2Int>(2)
        {
            associatedTile
        };
        Rotation = rot;

        DisplayInfos = new List<MuseumDisplayInfo>(Type == WallType.Solid? 4 :0);
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
    public void AddNewDisplayInfo()
    {
        if (DisplayInfos.Count == DisplayInfos.Capacity)
            throw new System.Exception($"Can only have {DisplayInfos.Capacity} displays on wall of type {Type}");

        AddDisplayToWall();
        AddDisplayToWall(- 1f);

        DisplayXPositionModifier = -DisplayXPositionModifier;
    }

    /// <summary>
    /// adds a new display
    /// </summary>
    void AddDisplayToWall(float YposModSign =1f)
    {
        MuseumDisplayInfo displayInfo = new MuseumDisplayInfo();

        //figure out position

        displayInfo.PositionModifier.y = .25f * YposModSign;

        displayInfo.PositionModifier.x = DisplayXPositionModifier;

        //outer walls fix
        if ((Tiles[0].x == VirtMuse.Size - 1) && Rotation == WallRotation.Vertical)
            displayInfo.PositionModifier.x = -displayInfo.PositionModifier.x;

        if (Tiles[0].y == 0 && Rotation == WallRotation.Horizontal)
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

    public void AddTile(Vector2Int tile)
    {
        if(Tiles.Count < 2)
        {
            Tiles.Add(tile);
        }
    }

}



