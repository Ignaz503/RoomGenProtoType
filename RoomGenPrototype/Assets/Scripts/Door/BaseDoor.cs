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

    public event Action<BaseDoor> OnOpen;
    public event Action<BaseDoor> OnClose;

    [SerializeField]protected DoorState currentState;
    [SerializeField]protected float movementSpeed;

    public abstract void Open();

    public abstract void Close();

    public abstract void Lock(string key);

    public abstract void Unlock(string key);

    public abstract void LockOpen();

    public abstract void UnlockOpen();

    protected void OnDoorOpen()
    {
        OnOpen?.Invoke(this);
    }

    protected void OnDoorClose()
    {
        OnClose?.Invoke(this);
    }
}
