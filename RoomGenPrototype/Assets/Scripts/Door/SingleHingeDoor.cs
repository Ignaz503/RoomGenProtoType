using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// single hinged door that rotates open on a single corner
/// </summary>
public class SingleHingeDoor : BaseDoor
{
    
    /// <summary>
    /// transform of the parent
    /// </summary>
    Transform parentTrans;
    
    /// <summary>
    /// rotation of door when close 
    /// </summary>
    Vector3 baseRotation;

    /// <summary>
    /// rotation that door has when open
    /// </summary>
    [SerializeField] float targetYRotation;

    /// <summary>
    /// current rotation of the door
    /// </summary>
    float currentYRotation;
    
    /// <summary>
    /// the point the door rotates around
    /// </summary>
    Vector3 PointToRotateAround;

    /// <summary>
    /// corutine ref when door is opening
    /// </summary>
    Coroutine openCoroutine;

    /// <summary>
    /// coroutine ref when door is closing
    /// </summary>
    Coroutine closeCoroutine;

    /// <summary>
    /// the doors box collider disabled whilst opening or closing active when open or closed
    /// </summary>
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

    /// <summary>
    /// coroutine that opens the door
    /// </summary>
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

    /// <summary>
    /// coroutine to close the door
    /// </summary>
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
