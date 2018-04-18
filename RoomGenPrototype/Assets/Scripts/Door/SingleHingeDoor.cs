using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleHingeDoor : BaseDoor
{
    //TODO maybe set to no collisions during turning of door
    Transform parentTrans;
    Vector3 baseRotation;
    [SerializeField] float targetYRotation;
    float currentYRotation;
    Vector3 PointToRotateAround;

    Coroutine openCoroutine;
    Coroutine closeCoroutine;

    BoxCollider boxCollider;
    // Use this for initialization
    void Start()
    {
        parentTrans = transform.parent;
        baseRotation = transform.eulerAngles;
        //if already rotated because wall rotated just add it to the wall
        targetYRotation += transform.eulerAngles.y;
        PointToRotateAround = transform.TransformPoint(new Vector3(.5f, -1f, .5f));

        boxCollider = GetComponent<BoxCollider>();

        OnOpen += (door) => { boxCollider.enabled = true; };

    }

    public override void Close()
    {
        if (currentState == DoorState.Closed || currentState == DoorState.Closing || currentState == DoorState.LockedClose)
            return;

        if (currentState == DoorState.LockedOpen)
            return;

        if(currentState == DoorState.Opening && openCoroutine != null)
        {
            StopCoroutine(openCoroutine);
            openCoroutine = null;
        }

        currentState = DoorState.Closing;
        closeCoroutine = StartCoroutine(CloseCoroutine());
    }

    public override void Lock(string key)
    {
        throw new System.NotImplementedException();
    }

    public override void LockOpen()
    {
        if(currentState != DoorState.Open)
        {
            //UserFeedaback
            Debug.Log("Door needs to be open to be locked open");
            return;
        }

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, targetYRotation, transform.eulerAngles.z);
        currentState = DoorState.LockedOpen;
    }

    public override void Open()
    {
        if (currentState == DoorState.Open || currentState == DoorState.Opening || currentState == DoorState.LockedOpen)
            return;

        if (currentState == DoorState.LockedClose)
            return;

        if(currentState == DoorState.Closing && closeCoroutine != null)
        {
            StopCoroutine(closeCoroutine);
            closeCoroutine = null;
        }

        //disable boxcolider as to not clooide with player
        boxCollider.enabled = false;

        currentState = DoorState.Opening;
        openCoroutine = StartCoroutine(OpenCoroutine());
    }

    public override void Unlock(string key)
    {
        throw new System.NotImplementedException();
    }

    public override void UnlockOpen()
    {
        currentState = DoorState.Open;
    }

    #region Coroutines
    IEnumerator OpenCoroutine()
    {
        transform.SetParent(null);
        while(transform.eulerAngles.y < targetYRotation)
        {
            transform.RotateAround(PointToRotateAround, Vector3.up, movementSpeed);

            yield return null;
        }

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, targetYRotation, transform.eulerAngles.z);

        transform.SetParent(parentTrans);
        currentState = DoorState.Open;
        OnDoorOpen();
    }

    IEnumerator CloseCoroutine()
    {
        transform.SetParent(null);
        while(transform.eulerAngles.y > baseRotation.y)
        {
            transform.RotateAround(PointToRotateAround, Vector3.up, -movementSpeed);
            yield return null;
        }

        transform.eulerAngles = baseRotation;
        transform.SetParent(parentTrans);
        currentState = DoorState.Closed;
        OnDoorClose();
    }

    #endregion
}
