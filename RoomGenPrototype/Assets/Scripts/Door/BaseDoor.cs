using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class BasseDoor : MonoBehaviour
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

    [SerializeField]DoorState CurrentState;
    [SerializeField]float MovementSpeed;

    public abstract void Open();

    public abstract void Close();

    public abstract void Lock(string key);

    public abstract void Unlock(string key);

}
