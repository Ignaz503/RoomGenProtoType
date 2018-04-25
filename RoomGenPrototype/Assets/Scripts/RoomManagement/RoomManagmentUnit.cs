using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManagmentUnit
{
    uint ManagedRoomID;
    List<GameObject> gameobjectsInRoom;
    public bool IsLoaded { get; protected set; }

    public RoomManagmentUnit(uint roomID,bool loadedState)
    {
        ManagedRoomID = roomID;
        gameobjectsInRoom = new List<GameObject>();
        IsLoaded = loadedState;
    }

    public void LoadRoom()
    {
        if (IsLoaded)
            return;
        SetGameobjectsActiveTo(true);
        IsLoaded = true;
    }

    public void UnloadRoom()
    {
        if (!IsLoaded)
            return;

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
        foreach (GameObject obj in gameobjectsInRoom)
            obj.SetActive(state);
    }

    public void AddGameObject(GameObject obj)
    {
        gameobjectsInRoom.Add(obj);
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
        return ManagedRoomID.ToString();
    }
}
