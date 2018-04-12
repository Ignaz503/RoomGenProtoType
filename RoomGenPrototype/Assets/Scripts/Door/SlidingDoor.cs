using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingDoor : BaseDoor
{
    float defaultClosePosition;
    float maxMovement;
    float currentMovement;
    Coroutine openCoroutine;
    Coroutine closeCoroutine;

    private void Awake()
    {
        maxMovement = Mathf.Abs(transform.localScale.z);
        defaultClosePosition = transform.localPosition.z;
    }

    public override void Close()
    {
        if (currentState == DoorState.Closed || currentState == DoorState.Closing)
            return;

        if (currentState == DoorState.Opening && openCoroutine != null)
        {
            StopCoroutine(openCoroutine);
            openCoroutine = null;
        }

        closeCoroutine = StartCoroutine(CloseCorutine());
    }

    public override void Open()
    {
        if (currentState == DoorState.Open || currentState == DoorState.Opening)
            return;
        if (currentState == DoorState.Locked)
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
        throw new System.NotImplementedException();
    }

    public override void Unlock(string key)
    {
        throw new System.NotImplementedException();
    }

    #region coroutines
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
    }

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
    }
    #endregion
}
