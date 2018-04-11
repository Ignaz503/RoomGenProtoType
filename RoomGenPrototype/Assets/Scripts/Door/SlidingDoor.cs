using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingDoor : BaseDoor
{
    Vector3 defaultClosePosition;
    float maxMovement;
    float currentMovement;

    private void Awake()
    {
        maxMovement = transform.localScale.z;
        defaultClosePosition = transform.localPosition;
    }

    public override void Close()
    {
        if (currentState == DoorState.Closed || currentState == DoorState.Closing)
            return;

        //TODO implement
        //temp
        if (currentState == DoorState.Opening)
            StopCoroutine(OpenCorutine());

        transform.localPosition = defaultClosePosition;

        currentState = DoorState.Closed;
    }

    public override void Open()
    {
        if (currentState == DoorState.Open || currentState == DoorState.Opening)
            return;
        if (currentState == DoorState.Locked)
            return; //TODO player feedback

        currentState = DoorState.Opening;
        StartCoroutine(OpenCorutine());
    }

    public override void Lock(string key)
    {
        throw new System.NotImplementedException();
    }

    public override void Unlock(string key)
    {
        throw new System.NotImplementedException();
    }

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
        currentMovement = 0f;
        Debug.Log(currentState);
    }

}
