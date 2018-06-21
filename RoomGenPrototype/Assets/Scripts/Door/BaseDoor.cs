using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class BaseDoor : MonoBehaviour
{
    public enum DoorState
    {
        Open,
        Opening,
        Closed,
        Closing,
        LockedClose,
        LockedOpen
    }

    /// <summary>
    /// event invoked when the door reaches open state
    /// </summary>
    public event Action<BaseDoor> OnOpen;

    /// <summary>
    /// event invoked when the door reaches closed state
    /// </summary>
    public event Action<BaseDoor> OnClose;

    /// <summary>
    /// the current state of the door
    /// </summary>
    [SerializeField]protected DoorState currentState;

    /// <summary>
    /// the movement speed of the door when opening and closing
    /// </summary>
    [SerializeField]protected float movementSpeed;

    /// <summary>
    /// function called to open the door
    /// </summary>
    public abstract void Open();

    /// <summary>
    /// function called to close door
    /// </summary>
    public abstract void Close();

    /// <summary>
    /// function called to lock a door closed
    /// </summary>
    /// <param name="key">the key needed to lock the door</param>
    public abstract void Lock(string key);

    /// <summary>
    /// function to unlock a door if it is locked
    /// </summary>
    /// <param name="key">the key needed to unlock it</param>
    public abstract void Unlock(string key);

    /// <summary>
    /// function to lock a door open and stop it from closing
    /// </summary>
    public abstract void LockOpen();

    /// <summary>
    /// function to unlock a door from it's forced open state
    /// </summary>
    public abstract void UnlockOpen();

    /// <summary>
    /// wrapper class to invoke event from sub classes
    /// </summary>
    protected void OnDoorOpen()
    {
        OnOpen?.Invoke(this);
    }

    /// <summary>
    /// wrapper function to invoke event from sub classes
    /// </summary>
    protected void OnDoorClose()
    {
        OnClose?.Invoke(this);
    }
}
