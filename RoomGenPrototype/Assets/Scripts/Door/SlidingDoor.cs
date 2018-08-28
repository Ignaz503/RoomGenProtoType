using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// door that slides open to one side
/// </summary>
public class SlidingDoor : BaseDoor
{
    /// <summary>
    /// local position when door is closed
    /// </summary>
    float defaultClosePosition;

    /// <summary>
    /// maximum movment to the side the door needs to do to be open
    /// </summary>
    float maxMovement;

    /// <summary>
    /// the current movement of the door when opening or closing
    /// </summary>
    float currentMovement;

    /// <summary>
    /// coroutine ref when openeing
    /// </summary>
    Coroutine openCoroutine;

    /// <summary>
    /// coroutine ref when closing
    /// </summary>
    Coroutine closeCoroutine;

    private void Awake()
    {
        maxMovement = Mathf.Abs(transform.localScale.z);
        defaultClosePosition = transform.localPosition.z;
    }

    public override void Close()
    {
        if (currentState == DoorState.Closed || currentState == DoorState.Closing || currentState == DoorState.LockedClose)
            return;

        if (currentState == DoorState.LockedOpen)
            return; //TODO UserFeedback

        if (currentState == DoorState.Opening && openCoroutine != null)
        {
            StopCoroutine(openCoroutine);
            openCoroutine = null;
        }
        currentState = DoorState.Closing;
        closeCoroutine = StartCoroutine(CloseCorutine());
    }

    public override void Open()
    {
        if (currentState == DoorState.Open || currentState == DoorState.Opening || currentState == DoorState.LockedOpen)
            return;
        if (currentState == DoorState.LockedClose)
            return; //TODO player feedback

        if(currentState == DoorState.Closing && closeCoroutine != null)
        {
            StopCoroutine(closeCoroutine);
            closeCoroutine = null;
        }

        currentState = DoorState.Opening;
        openCoroutine = StartCoroutine(OpenCorutine());
    }

    public override void Lock(string key)
    {
        if(currentState != DoorState.Closed)
        {
            //TODO: feedback
            throw new Exception("Door needs to be closed to lock, try LockOpen if you want to keep it from closing");
        }
        currentState = DoorState.LockedClose;
        currentMovement = 0f;
        transform.localPosition = new Vector3(transform.localPosition.x,transform.localPosition.y,defaultClosePosition);
        currentMovement = 0f;
    }

    public override void Unlock(string key)
    {
        //TODO: key comparison and so on
        if (currentState != DoorState.LockedClose)
        {
            Debug.Log("Door isn't locekd");
            return;
        }
        currentState = DoorState.Closed;
    }

    public override void LockOpen()
    {
        if(currentState != DoorState.Open)
        {
            //TODO User Feedback
            Debug.Log("Open first");
            return;
        }

        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, maxMovement);
        currentState = DoorState.LockedOpen;
        currentMovement = maxMovement;
    }

    public override void UnlockOpen()
    {
        currentState = DoorState.Open;
    }

    #region coroutines

    /// <summary>
    /// coroutine to open sliding door
    /// </summary>
    IEnumerator OpenCorutine()
    {
        Vector3 locPos = Vector3.zero;
        locPos.y = transform.localPosition.y;
        while (currentMovement < maxMovement)
        {
            currentMovement += movementSpeed * Time.deltaTime;
            locPos.z = currentMovement;
            transform.localPosition = locPos;
            
            yield return null;
        }
        currentState = DoorState.Open;
        locPos.z = maxMovement;
        transform.localPosition = locPos;

        openCoroutine = null;
        OnDoorOpen();
    }

    /// <summary>
    /// coroutine to close the door 
    /// </summary>
    /// <returns></returns>
    IEnumerator CloseCorutine()
    {
        Vector3 localPos = Vector3.zero;
        localPos.y = transform.localPosition.y;

        while(currentMovement > defaultClosePosition)
        {
            currentMovement -= movementSpeed * Time.deltaTime;
            localPos.z = currentMovement;
            transform.localPosition = localPos;
            yield return null;
        }

        currentState = DoorState.Closed;
        currentMovement = 0f;
        localPos.z = defaultClosePosition;
        transform.localPosition = localPos;
        closeCoroutine = null;
        OnDoorClose();
    }
    #endregion
}
