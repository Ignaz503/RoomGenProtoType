using System;
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
    /// <summary>
    /// helper dictionary that maps from room type to placable checker of this room
    /// </summary>
    static Dictionary<RoomType, IRoomPlacableChecker> roomTypeToPlaceableChecker = new Dictionary<RoomType, IRoomPlacableChecker>()
    {
            { RoomType.Normal, new BaseRoomTypePlacableCheker(RoomType.Normal) },
            { RoomType.Long, new BaseRoomTypePlacableCheker(RoomType.Long) },
            { RoomType.Big, new BaseRoomTypePlacableCheker(RoomType.Big) },
            { RoomType.L, new BaseRoomTypePlacableCheker(RoomType.L) }
    };

    /// <summary>
    /// defines size of museum 
    /// they currently are always same width and height
    /// </summary>
    [DataMember]
    public int Size { get; set; }

    /// <summary>
    /// map of all the tiles in the museum and which room type is placed in this tile
    /// -1 if there is currently no room on this tile
    /// </summary>
    public int[,] RoomMap { get; protected set; }

    /// <summary>
    /// list of all the rooms that build this museum
    /// </summary>
    [DataMember]
    public List<Room> Rooms { get; protected set; }

    /// <summary>
    /// list of all the walls in the museum
    /// </summary>
    [DataMember]
    public List<Wall> Walls { get; protected set; }

    /// <summary>
    /// a graph that defines the connectivty of the rooms with each other
    /// </summary>
    [DataMember]
    public MuseumGraph MuseumsGraph { get; protected set; }

    /// <summary>
    /// the current number of doors in the museum
    /// </summary>
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

    /// <summary>
    /// the current number of displays in the museum
    /// </summary>
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
        MuseumsGraph = new MuseumGraph();
    }

    /// <summary>
    /// Generates a Museum
    /// baisc algortihm:
    /// 1. palce random 1x1 room as start 
    /// 2. get outline around this room
    /// 3. choose random tile from outline
    /// 4. find out what rooms can be placed at this tile
    /// 5. place one of the possible rooms
    /// 6. connect new room to already existing rooms
    /// 7. update outline
    /// 8. repeat from step 3. until no more tiles in outline
    /// 9. fill all the displays that are in the museum
    /// 10. build museum graph
    /// </summary>
    /// <param name="seed"></param>
    public void Generate(string seed, bool buildMuseumsGraph = true)
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
                List<Vector2Int[]> possibleSequencesForType;
                if (roomTypeToPlaceableChecker.ContainsKey(t))
                {
                    if (roomTypeToPlaceableChecker[t].CheckIfPlacable(newRoomOrigin, out possibleSequencesForType,this))
                    {
                        typeToPossStepSequences.Add(t, possibleSequencesForType);
                    }
                }
            }
            #endregion

            if (typeToPossStepSequences.Keys.Count > 0)
            {
                //there are possible rooms to place
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
                    //normal already added by starting tile no need to add anything else
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

        if (buildMuseumsGraph)
            MuseumsGraph.BuildMuseumGraph(this);
    }

    /// <summary>
    /// Fills the displas that exist in the museum
    /// also creates texture info for walls
    /// WARNING: CURRENTLY TEMP IMPLEMENTATION
    /// </summary>
    void FillDisplays(System.Random rng)
    {
        System.Random styleRng = new System.Random();
        //TODO fill displays with info from resource manager

        //TEMP
        foreach(Room r in Rooms)
        {
            //TEMPORARY ROOMSTYLE CHOOSING for showcase
            RoomStyle s = MuseumGenerator.Instance.RoomStyles[styleRng.Next(0, MuseumGenerator.Instance.RoomStyles.Count)];

            foreach(MuseumDisplayInfo dispInf in r.CenterDisplayInfos)
            {
                dispInf.Type = (rng.Next(0, 2) > 0) ? Display.DisplayType.MeshDisplay : Display.DisplayType.ImageDisplay;
            }

            r.FloorTexture = new MuseumTextureInfo()
            {
                AssociatedID = r.RoomID.ToString(),
                PositionModifier = 0,
                AssociatedResourceLocators = s.floorTexture.name
            };

            r.CeilingTexture = new MuseumTextureInfo()
            {
                AssociatedID = r.RoomID.ToString(),
                PositionModifier = 0,
                AssociatedResourceLocators = s.ceilingTexture.name
            };

            foreach(int wID in r.Walls)
            {
                Wall w = Walls[wID];
                w.TextureInfos.Add(new MuseumTextureInfo() { AssociatedResourceLocators = s.wallTexture.name });
            }
            //TODO: fill associated resource locator
        }

        foreach(Wall w in Walls)
        {
            int i = 0;
            foreach(Vector2 tile in w.Tiles)
            {
                //figure out position modifier
                //0 means texture from 0 to 0.5
                //1 means texture from 0.5 to 1
                // (everything in uv coords)
                int posMod = 0;
                switch (w.Rotation)
                {
                    case Wall.WallRotation.Horizontal:
                        if (Mathf.Sign(tile.y - w.Location[0].y) < 0f)
                            posMod = 1;
                        else
                            posMod = 0;
                        break;
                    case Wall.WallRotation.Vertical:
                        if (Mathf.Sign(tile.x - w.Location[0].x) < 0f)
                            posMod = 0;
                        else
                            posMod = 1;
                        break;
            }// end switch
            w.TextureInfos[i].PositionModifier = posMod;
                w.TextureInfos[i].AssociatedID = w.WallID;
                i++;
                //TODO: FILL Associated resource locator
            }// end foreach tile associated with wall
            if (w.Type == Wall.WallType.Door)
                continue;

            foreach (MuseumDisplayInfo dispInf in w.DisplayInfos)
            {
                dispInf.Type = (rng.Next(0, 2) > 0) ? Display.DisplayType.MeshDisplay : Display.DisplayType.ImageDisplay;
            }

        }// end foreach wall
    }

    /// <summary>
    /// Finds a room via the ID of it
    /// </summary>
    /// <param name="RoomID">the room ID of the room we are looking for</param>
    /// <returns>The room with the ID</returns>
    public Room GetRoomByID(uint RoomID)
    {
        return Rooms.First(i => { return i.RoomID == RoomID; });
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
    /// <returns>the location of the wall in the museum walls array</returns>
    public int AddWall(Wall w)
    {
        for (int i = 0; i < Walls.Count; i++)
        {
            if (Walls[i] == w)
            {
                Walls[i].MergeWalls(w);
                Walls[i].AddNewDisplayInfo(w.AssociatedRoomIDs[0]);
                return i;
            }
        }
        Walls.Add(w);
        w.AddNewDisplayInfo(w.AssociatedRoomIDs[0]);
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

    /// <summary>
    /// adds a new room placable checker
    /// </summary>
    /// <param name="t">new room type</param>
    /// <param name="checker">the placable checker for this type</param>
    /// <param name="overrideExisting">flag if already existing placable checker should be overwritten</param>
    public static void AddNewRoomPlacableChecker(RoomType t, IRoomPlacableChecker checker, bool overrideExisting)
    {
        if (roomTypeToPlaceableChecker.ContainsKey(t) && overrideExisting)
        {
            roomTypeToPlaceableChecker[t] = checker;
        }
        else if (!roomTypeToPlaceableChecker.ContainsKey(t))
        {
            roomTypeToPlaceableChecker.Add(t, checker);
        }
        else
            throw new Exception("There already exits a placable checker for this type");
    }

}
