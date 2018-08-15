using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace VirtMuseWeb.Utility
{
    /// <summary>
    /// A wall that seperates two rooms of a museum or is an outer wall of the museum
    /// </summary>
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

        //Museum VirtMuse;
        /// <summary>
        /// helper var for deciding on where to put the displays of the wall
        /// when walls are merged
        /// </summary>
        int TileIndexForXPositionModifierCalc = 0;

        /// <summary>
        /// Wall ID, uniquze string for this wall of the museum
        /// built via associated room IDS
        /// eg: wall between room 1 and 2 -> "1,2"
        /// if wall only associated with one room then -1 is second number
        /// </summary>
        public string WallID
        {
            get
            {
                return "" + AssociatedRoomIDs[0] + "," + ((AssociatedRoomIDs.Count == 2) ? AssociatedRoomIDs[1] : -1u);
            }
        }

        /// <summary>
        /// the type of wall (solid or door)
        /// </summary>
        [DataMember]
        public WallType Type { get; protected set; }
        //Used for equaality checking
        /// <summary>
        /// the location of the wall defined as the two vertices where the wall is inbetween
        /// vertices are room corners
        /// </summary>
        public Vector2[] Location { get; protected set; }

        /// <summary>
        /// position modifier when placing the wall gamobject for the museum
        /// </summary>
        [DataMember]
        public float PositionModifier { get; protected set; }

        /// <summary>
        /// tiles that are seperated from this wall
        /// </summary>
        [DataMember]
        public List<Vector2Int> Tiles { get; protected set; }

        /// <summary>
        /// defines if wall is vertical wall or horizontal wall in museum
        /// influneces y rotation of gamobject
        /// </summary>
        [DataMember]
        public WallRotation Rotation { get; protected set; }

        /// <summary>
        /// info for all the displays that this wall has
        /// </summary>
        [DataMember]
        public List<MuseumDisplayInfo> DisplayInfos { get; protected set; }

        /// <summary>
        /// Info about what textures this wall will have
        /// Every wall seperaates at max 2 rooms
        /// so it has a size of 2, when only one room is adjacent it only has one member
        /// </summary>
        [DataMember]
        public List<MuseumTextureInfo> TextureInfos { get; protected set; }

        /// <summary>
        /// IDs of all the rooms that are spaerated by this wall
        /// </summary>
        [DataMember]
        public List<uint> AssociatedRoomIDs { get; protected set; }

        public Wall(WallType t, Vector2[] location, float posMod, Vector2Int associatedTile, uint associatedRoomID, WallRotation rot, Museum virt)
        {
            //VirtMuse = virt;
            Type = t;
            Location = location;
            PositionModifier = posMod;
            Tiles = new List<Vector2Int>(2)
        {
            associatedTile
        };
            Rotation = rot;

            DisplayInfos = new List<MuseumDisplayInfo>(Type == WallType.Solid ? 4 : 0);

            AssociatedRoomIDs = new List<uint>(2)
        {
            associatedRoomID
        };

            TextureInfos = new List<MuseumTextureInfo>();
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
            foreach (Vector2 loc in lhs.Location)
            {
                foreach (Vector2 lo in rhs.Location)
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
        public void AddNewDisplayInfo(uint associatedRoomID)
        {
            if (DisplayInfos.Count == DisplayInfos.Capacity)
                throw new System.Exception($"Can only have {DisplayInfos.Capacity} displays on wall of type {Type}");

            AddDisplayToWall(associatedRoomID);
            AddDisplayToWall(associatedRoomID, -1f);

            TileIndexForXPositionModifierCalc++;
        }

        /// <summary>
        /// adds a new display
        /// </summary>
        void AddDisplayToWall(uint associatedRoomID, float YposModSign = 1f)
        {
            MuseumDisplayInfo displayInfo = new MuseumDisplayInfo()
            {
                AssociatedRoomID = associatedRoomID
            };

            //figure out position

            displayInfo.PositionModifier.y = .25f * YposModSign;

            if (Rotation == WallRotation.Vertical)
                displayInfo.PositionModifier.x = Mathf.Sign(Tiles[TileIndexForXPositionModifierCalc].x - Location[0].x);
            else
                displayInfo.PositionModifier.x = Mathf.Sign(Tiles[TileIndexForXPositionModifierCalc].y - Location[0].y) * -1f;


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

        /// <summary>
        /// Merges Wall given into wall called on
        /// </summary>
        /// <param name="w"></param>
        public void MergeWalls(Wall w)
        {
            if (Tiles.Count < 2)
            {
                Tiles.Add(w.Tiles[0]);
                AssociatedRoomIDs.Add(w.AssociatedRoomIDs[0]);
            }
        }
    }
}