using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class PlayerHand : MonoBehaviour {

    public enum HandState
    {
        InRestingPosition,
        InfrontOfFace,
        MovingTowardRestingPosition,
        MovingInfrontOfFace
    }

    #region Events
    /// <summary>
    /// called after state is set to rest position
    /// </summary>
    public event System.Action InRestingPosition;
    /// <summary>
    /// called after state is set to InfrontOfFace
    /// </summary>
    public event System.Action OnReachedInfronOfFacePosition;
    /// <summary>
    /// called after state is set to MovingTowardRestingPosition
    /// </summary>
    public event System.Action OnStartMovingToRestPosition;
    /// <summary>
    /// called after state is set to MovingInfrontOfFace

    /// </summary>
    public event System.Action OnStartMovingInfrontOfFace;

    /// <summary>
    /// called after(same frame) objectInHand is set and object was subscribed to all hand events it needs to know
    /// </summary>
    public event Action<IHoldableObject, PlayerHand> OnGrabObject;

    /// <summary>
    /// called the frame the object is dropped, when the obj was unsbscribed from all events, and the objectInHand was set to null
    /// </summary>
    public event Action<IHoldableObject, PlayerHand> OnDropObject;
    #endregion

    HandState _currentState;
    /// <summary>
    /// The current state of the hand
    /// </summary>
    public HandState CurrentState
    {  get { return _currentState; }
        protected set {
            _currentState = value;
            switch (_currentState)
            {
                case HandState.InfrontOfFace:
                    OnReachedInfronOfFacePosition?.Invoke();
                    break;
                case HandState.InRestingPosition:
                    InRestingPosition?.Invoke();
                    break;
                case HandState.MovingInfrontOfFace:
                    OnStartMovingInfrontOfFace?.Invoke();
                    break;
                case HandState.MovingTowardRestingPosition:
                    OnStartMovingToRestPosition?.Invoke();
                    break;
            }
        } }
    
    /// <summary>
    /// The player this hand belongs to
    /// </summary>
    public Player OwningPlayer { get;  set; }

    /// <summary>
    /// the object currently held by the hand
    /// </summary>
    protected IHoldableObject objectInHand;

    /// <summary>
    /// subscribes object to all relevent events and calles on obj grabbed
    /// </summary>
    public virtual void GrabObject(IHoldableObject holdObj)
    {
        InRestingPosition += holdObj.OnPutAway;
        OnReachedInfronOfFacePosition += holdObj.OnHeldInfrontOfFace;
        OnStartMovingInfrontOfFace += holdObj.OnStartedMoveInfrontOfFace;
        OnStartMovingToRestPosition += holdObj.OnStartedMoveAwayFromFace;

        if (holdObj.IsInterestedInInteractableObjects())
        {
            OwningPlayer.OnInInteractionRange += ForwardInteractableObject;
            //TODO maybe update if out of range? or jsut show as long until next
        }
        objectInHand = holdObj;
        holdObj.OnGrabed(this);
        OnGrabObject?.Invoke(objectInHand, this);

    }

    /// <summary>
    /// drops any held object, unssubscribes this object from all events
    /// </summary>
    public virtual  void DropObject()
    {
        if(objectInHand != null)
        {
            InRestingPosition -= objectInHand.OnPutAway;
            OnReachedInfronOfFacePosition -= objectInHand.OnHeldInfrontOfFace;
            OnStartMovingInfrontOfFace -= objectInHand.OnStartedMoveInfrontOfFace;
            OnStartMovingToRestPosition -= objectInHand.OnStartedMoveAwayFromFace;

            if (objectInHand.IsInterestedInInteractableObjects())
            {
                OwningPlayer.OnInInteractionRange -= ForwardInteractableObject;
                //TODO maybe update if out of range? or jsut show as long until next
            }
        }
        IHoldableObject objHeld = objectInHand;
        objectInHand = null;
        objHeld.OnDropped(this);
        OnDropObject(objHeld, this);
    }

    /// <summary>
    /// Forwards the IInteractable information to the held object if it is intersted in
    /// so that not all object need to know about the player
    /// makes sure that the IInteracteable is not null
    /// or it wont be called so that the held obj doesn't need to worry about it
    /// </summary>
    /// <param name="interactObj">the interacteb object interaction event params</param>
    public void ForwardInteractableObject(PlayerInteractionEventArgs interactObj)
    {
        if(objectInHand != null && interactObj.InteractedObject != null)
        {
            objectInHand.SetInteractedObject(interactObj.InteractedObject);
        }
    }

    #region event invocation warappers
    /// <summary>
    /// invokes event InRestingPosition
    /// </summary>
    protected void InvokeInRestingPosition()
    {
        InRestingPosition?.Invoke();
    }

    /// <summary>
    /// invokes invent OnReachedInFrontOfFacePosition
    /// </summary>
    protected void InvokeOnReachedInfrontOfFacePosition()
    {
        OnReachedInfronOfFacePosition?.Invoke();
    }

    /// <summary>
    /// invokes OnStartMovingToRestPosition event
    /// </summary>
    protected void InvokeOnStartMovingToRestPosition()
    {
        OnStartMovingToRestPosition?.Invoke();
    }

    /// <summary>
    /// invokes OnStartMovingInfrontOFFace event
    /// </summary>
    protected void InvokeOnStartMovingInfrontOfFace()
    {
        OnStartMovingInfrontOfFace?.Invoke();
    }

    #endregion
}
