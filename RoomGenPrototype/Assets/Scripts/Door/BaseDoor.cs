using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BaseDoor : MonoBehaviour, IDoor
{
    //TODO Door base class that allows for diff kind of doors
    public enum DoorState
    {
        Open,
        Opening,
        Closed,
        Closing,
        Locked
    }

    [SerializeField]protected DoorState currentState;
    [SerializeField]protected float movementSpeed;

    public virtual void Open()
    {
        throw new NotImplementedException();
    }

    public virtual void Close()
    {
        throw new NotImplementedException();
    }

    public virtual void Lock(string key)
    {
        throw new NotImplementedException();
    }

    public virtual void Unlock(string key)
    {
        throw new NotImplementedException();
    }
}
