using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// class that contains all gamobjects that are in a certain room
/// und will manage unloading and loading for it
/// </summary>
public class RoomManagmentUnit
{
    uint ManagedRoomID;
    //TODO mabye not public
    public List<GameObject> GameobjectsInRoom;
    public bool IsLoaded { get; protected set; }

    public RoomManagmentUnit(uint roomID,bool loadedState)
    {
        ManagedRoomID = roomID;
        GameobjectsInRoom = new List<GameObject>();
        IsLoaded = loadedState;
    }

    public void LoadRoom()
    {
        SetGameobjectsActiveTo(true);
        IsLoaded = true;
    }

    public void UnloadRoom()
    {
        SetGameobjectsActiveTo(false);
        IsLoaded = false;
    }

    public void ToggleRoom()
    {
        IsLoaded = !IsLoaded;
        SetGameobjectsActiveTo(IsLoaded);
    }

    void SetGameobjectsActiveTo(bool state)
    {
        foreach (GameObject obj in GameobjectsInRoom)
        {
            obj.SetActive(state);
        }
    }

    public void AddGameObject(GameObject obj)
    {
        GameobjectsInRoom.Add(obj);
    }

    public override int GetHashCode()
    {
        return ManagedRoomID.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        if(obj is RoomManagmentUnit)
        {
            return ManagedRoomID == (obj as RoomManagmentUnit).ManagedRoomID;
        }
        return false;
    }

    public override string ToString()
    {
        string str = ManagedRoomID.ToString();

        foreach(GameObject obj in GameobjectsInRoom)
        {
            str += " " + obj.name + "\n";
        }
        return str;
    }

}
