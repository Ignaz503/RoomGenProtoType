using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCPlayerHand : PlayerHand
{
    /// <summary>
    /// the position the mouse has when in rest position
    /// </summary>
    [SerializeField] Vector3 restPosition;

    /// <summary>
    /// the position the hand has when held infront of the face
    /// </summary>
    [SerializeField] Vector3 infrontOfFacePosition;

    /// <summary>
    /// the key that starts the process of moving the hand eiter into rest or infront of face position
    /// </summary>
    [SerializeField] KeyCode moveHandKey;

    /// <summary>
    /// used so that player shows up in editor
    /// </summary>
    [SerializeField] Player p { get { return OwningPlayer; } set { OwningPlayer = value; } }

    /// <summary>
    /// the meta display held from the start
    /// </summary>
    [SerializeField] MetaDataDisplay metaDisplay;

    /// <summary>
    /// Time To reach one position to another in seconds
    /// </summary>
    [SerializeField] float movementTime;

    /// <summary>
    /// velocity vector used by smoothdamp
    /// </summary>
    Vector3 velocity = Vector3.zero;
    
    private void Start()
    {
        transform.localPosition = restPosition;
        CurrentState = HandState.InRestingPosition;
        GrabObject(metaDisplay);

        InvokeInRestingPosition();
    }

    public override void DropObject()
    {
        throw new System.NotImplementedException();
    }

    public override void GrabObject(IHoldableObject holdObj)
    {
        holdObj.Object.transform.SetParent(transform);
        base.GrabObject(holdObj);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 0.2f);
    }

    private void Update()
    {
        HandleMovement();
    }

    /// <summary>
    /// handels state transitions of hand and how to move it
    /// in the case of move hand key being pressed
    /// </summary>
    void HandleMovement()
    {
        if (Input.GetKeyDown(moveHandKey))
        {
            switch (CurrentState)
            {
                case HandState.InRestingPosition:
                    CurrentState = HandState.MovingInfrontOfFace;
                    StartCoroutine(MoveTo(infrontOfFacePosition,HandState.InfrontOfFace));
                    break;
                case HandState.InfrontOfFace:
                    CurrentState = HandState.MovingTowardRestingPosition;
                    StartCoroutine(MoveTo(restPosition, HandState.InRestingPosition));
                    break;
                case HandState.MovingInfrontOfFace:
                    CurrentState = HandState.MovingTowardRestingPosition;
                    StopAllCoroutines();
                    StartCoroutine(MoveTo(restPosition, HandState.InRestingPosition));
                    break;
                case HandState.MovingTowardRestingPosition:
                    CurrentState = HandState.MovingInfrontOfFace;
                    StopAllCoroutines();
                    StartCoroutine(MoveTo(infrontOfFacePosition, HandState.InfrontOfFace));
                    break;
            }
        }
    }

    /// <summary>
    /// corutione that moves to a local position
    /// </summary>
    /// <param name="position">the position to move to</param>
    /// <param name="nextState">the state the hand enters next</param>
    IEnumerator MoveTo(Vector3 position, HandState nextState)
    {
        while ((transform.localPosition - position).magnitude >= 0.05f) 
        {
            
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, position, ref velocity,movementTime);
            yield return null;
        }
        //done
        Debug.Log("Done with moving towards");
        CurrentState = nextState;
        transform.localPosition = position;
        velocity = Vector3.zero;
    }

}
