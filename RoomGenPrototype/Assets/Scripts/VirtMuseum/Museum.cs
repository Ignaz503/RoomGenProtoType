﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.IO;
using System.Xml;
using System.Text;
using System.Linq;
using UnityEngine;

[DataContract]
public class Museum
{
    static Dictionary<RoomType, BaseRoomTypePlacableCheker> roomTypeToPlaceableChecker = new Dictionary<RoomType, BaseRoomTypePlacableCheker>()
    {
            { RoomType.Normal, new BaseRoomTypePlacableCheker(RoomType.Normal) },
            { RoomType.Long, new BaseRoomTypePlacableCheker(RoomType.Long) },
            { RoomType.Big, new BaseRoomTypePlacableCheker(RoomType.Big) },
            { RoomType.L, new BaseRoomTypePlacableCheker(RoomType.L) }
    };

    [DataMember]
    public int Size { get; set; }

    public int[,] RoomMap { get; protected set; }

    [DataMember]
    public List<Room> Rooms { get; protected set; }
    [DataMember]
    public List<Wall> Walls { get; protected set; }

    //[DataMember]
    public int CurrentNumberOfDoors
    {
        get
        {
            int count = 0;
            foreach (Wall w in Walls)
            {
                if (w.Type == Wall.WallType.Door)
                    count++;
            }
            return count;
        }
    }

    //[DataMember]
    public int NumberOfDisplays
    {
        get
        {
            int i = 0;
            foreach (Room r in Rooms)
                i += r.RoomTiles.Count;
            foreach (Wall w in Walls)
                i += w.DisplayInfos.Count;
            return i;
        }
    }

    public Museum(int size)
    {
        Size = size;
        RoomMap = new int[Size, Size];

        for (int x = 0; x < Size; x++)
        {
            for (int y = 0; y < Size; y++)
            {
                RoomMap[x, y] = -1;
            }
        }

        Rooms = new List<Room>();
        Walls = new List<Wall>();
    }

    /// <summary>
    /// Generates a Museum
    /// </summary>
    /// <param name="seed"></param>
    public void Generate(string seed)
    {
        //TODO add resourcemanager get numb resources for type
        System.Random rng = new System.Random(seed.GetHashCode());

        #region Start Tile setup
        int xStart = rng.Next(0, Size);
        int yStart = rng.Next(0, Size);

        RoomMap[xStart, yStart] = (int)RoomType.Normal;

        Rooms.Add(new Room(RoomType.Normal, new List<Vector2Int> { new Vector2Int(xStart, yStart) },this));

        List<Vector2Int> startSourrounding = GetSourroundingTiles(new Vector2Int(xStart, yStart));

        HashSet<int> outline = new HashSet<int>();

        foreach (Vector2Int t in startSourrounding)
        {
            outline.Add(t.x + (t.y * Size));
        }
        #endregion

        //TODO add condition currentNumberDisplays.Count < resourceManager.ResourceCount
        while (outline.Count > 0)
        {
            int newOr = outline.ToList()[rng.Next(0, outline.Count)];
            outline.Remove(newOr);

            Vector2Int newRoomOrigin = new Vector2Int(newOr % Size, newOr / Size);

            #region Get possible roomtypes and possible step sequences
            Dictionary<RoomType, List<Vector2Int[]>> typeToPossStepSequences = new Dictionary<RoomType, List<Vector2Int[]>>();

            foreach (RoomType t in Enum.GetValues(typeof(RoomType)))
            {
                List<Vector2Int[]> sequencesForType;
                if (roomTypeToPlaceableChecker.ContainsKey(t))
                {
                    if (roomTypeToPlaceableChecker[t].CheckIfPlacable(newRoomOrigin, out sequencesForType,this))
                    {
                        typeToPossStepSequences.Add(t, sequencesForType);
                    }
                }
            }
            #endregion

            if (typeToPossStepSequences.Keys.Count > 0)
            {
                RoomType typeToPlace = typeToPossStepSequences.Keys.ToList()[rng.Next(0, typeToPossStepSequences.Keys.Count)];

                Vector2Int[] sequence = typeToPossStepSequences[typeToPlace][rng.Next(0, typeToPossStepSequences[typeToPlace].Count)];

                List<Vector2Int> RoomTiles = new List<Vector2Int> { newRoomOrigin };

                HashSet<int> surroundingTiles = new HashSet<int>();

                foreach (Vector2Int t in GetSourroundingTiles(newRoomOrigin))
                {
                    surroundingTiles.Add(TransformTileCoordIntoOneD(t));
                }

                #region Add all tiles to room
                if (typeToPlace != RoomType.Normal)
                {
                    foreach (Vector2Int step in sequence)
                    {
                        Vector2Int coord = newRoomOrigin + step;

                        RoomTiles.Add(coord);

                        int coordOneD = coord.x + (coord.y * Size);

                        surroundingTiles.Remove(coordOneD);

                        outline.Remove(coordOneD);

                        if (RoomMap[coord.x, coord.y] != -1)
                            throw new Exception("Trying to overwrite already used tile");

                        //Debug.Log("Setting room map to " +(int)typeToPlace+ " at "+ coord);
                        RoomMap[coord.x, coord.y] = (int)typeToPlace;

                        foreach (Vector2Int sur in GetSourroundingTiles(coord))
                        {
                            surroundingTiles.Add(TransformTileCoordIntoOneD(sur));
                        }
                    }
                }
                #endregion

                //updating roommap for room origin tile
                RoomMap[newRoomOrigin.x, newRoomOrigin.y] = (int)typeToPlace;
                //Debug.Log("Setting room map to " + (int)typeToPlace+ " at " + newRoomOrigin);

                //removing origin form souruounding tiles 
                //if added by other room tiles
                surroundingTiles.Remove(newOr);

                #region updating outline
                foreach (int i in surroundingTiles)
                {
                    if (!outline.Contains(i))
                        if (RoomMap[i % Size, i / Size] == -1)
                            outline.Add(i);
                }

                foreach (Vector2Int roomTile in RoomTiles)
                {
                    outline.Remove(roomTile.x + (roomTile.y * Size));
                }
                #endregion

                #region creating room obj and adding doors
                Room r = new Room(typeToPlace, RoomTiles,this);
                int CreatedDoor = 0;
                foreach (Room otherRoom in Rooms)
                {
                    if (!(CreatedDoor >= 2))
                    {
                        bool createdDoor = Room.CreateDoorBetweenRooms(r, otherRoom);
                        if (createdDoor)
                            CreatedDoor++;
                    }
                }

                Rooms.Add(r);
                #endregion

                //LogMap();
                //LogOutline(outline);
            }// end if check poss placable type
        }// end while

        FillDisplays(rng);
    }

    /// <summary>
    /// Fills the displas that exist in the museum
    /// WARNING: CURRENTLY TEMP IMPLEMENTATION
    /// </summary>
    void FillDisplays(System.Random rng)
    {
        //TODO fill displays with info from resource manager

        //TEMP
        foreach(Room r in Rooms)
        {
            foreach(MuseumDisplayInfo dispInf in r.CenterDisplayInfos)
            {
                dispInf.Type = (rng.Next(0, 2) > 0) ? Display.DisplayType.MeshDisplay : Display.DisplayType.ImageDisplay;
            }
        }

        foreach(Wall w in Walls)
        {
            if (w.Type == Wall.WallType.Door)
                continue;

            foreach (MuseumDisplayInfo dispInf in w.DisplayInfos)
            {
                dispInf.Type = (rng.Next(0, 2) > 0) ? Display.DisplayType.MeshDisplay : Display.DisplayType.ImageDisplay;
            }

        }
    }

    /// <summary>
    /// returns the sourrounding tiles for a given position
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    List<Vector2Int> GetSourroundingTiles(Vector2Int pos)
    {
        List<Vector2Int> surr = new List<Vector2Int>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {

                if (x == 0 && y == 0)
                {
                    continue;
                }

                int x_pos = pos.x + x;
                int y_pos = pos.y + y;

                if (x_pos < 0 || x_pos >= Size ||
                    y_pos < 0 || y_pos >= Size)
                {
                    continue;
                }

                surr.Add(new Vector2Int(x_pos, y_pos));
            }
        }

        return surr;
    }

    /// <summary>
    /// Transforms a two dimensional coord(row|column)
    /// into a one dimensional coord for this museum
    /// </summary>
    /// <param name="coord"></param>
    /// <returns></returns>
    int TransformTileCoordIntoOneD(Vector2Int coord)
    {
        return coord.x + (coord.y * Size);
    }

    /// <summary>
    /// Transforms a one dimensional value into a row and column
    /// value for this museum
    /// item1 in tuple = x 
    /// item2 in tuple = y 
    /// </summary>
    Tuple<int,int> TransformOneDCoordIntoTwoD(int coord)
    {
        return new Tuple<int, int>(coord % Size, coord / Size);
    }

    [Obsolete]
    void LogMap()
    {
        string log = "";

        for (int x = 0; x < Size; x++)
        {
            for (int y = 0; y < Size; y++)
            {
                log += $" {RoomMap[x, y]}";
            }
            log += "\n";
        }
        Debug.Log(log);
    }

    [Obsolete]
    void LogOutline(HashSet<int> outline)
    {
        string log = "";

        foreach (int i in outline)
        {
            int x = i % Size;
            int y = i / Size;
            log += $"{x},{y}, ";
        }
        Debug.Log(log);
    }

    /// <summary>
    /// Adds a wall to the walls container if the wall doesn't exist
    /// otherwise returns the index of the wall that already exists
    /// (2 rooms share one wall in that case)
    /// </summary>
    public int AddWall(Wall w)
    {
        for (int i = 0; i < Walls.Count; i++)
        {
            if (Walls[i] == w)
            {
                Walls[i].AddNewDisplayInfo();
                Walls[i].AddTile(w.Tiles[0]);
                return i;
            }
        }
        w.AddNewDisplayInfo();
        Walls.Add(w);
        return Walls.Count - 1;
    }

    [Obsolete]
    void RemoveWallsContainigPoint(Vector2 p)
    {
        // remove all walls that contain this point
        for (int i = 0; i < Walls.Count; i++)
        {
            if (Walls[i].ContainsPoint(p))
            {
                Walls.RemoveAt(i);
                i--;
            }
        }
    }

    /// <summary>
    /// Serilaizes a museum to an xml string
    /// </summary>
    /// <returns></returns>
    public string Serialize()
    {
        MemoryStream stream = new MemoryStream();
        DataContractSerializer serializer = new DataContractSerializer(typeof(Museum));
        XmlWriterSettings settings = new XmlWriterSettings() { Indent = true };

        using (XmlWriter writer = XmlWriter.Create(stream, settings))
            serializer.WriteObject(writer, this);

        stream.Position = 0;
        StreamReader reader = new StreamReader(stream);
        string data = reader.ReadToEnd();
        reader.Close();
        stream.Close();
        return data;
    }

    /// <summary>
    /// creates a museum from an xml string
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static Museum Deserialize(string data)
    {
        MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(data));
        DataContractSerializer serializer = new DataContractSerializer(typeof(Museum));
        Museum museum = serializer.ReadObject(stream) as Museum;
        stream.Close();
        return museum;
    }
}
