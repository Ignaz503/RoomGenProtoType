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
    public event Action InRestingPosition;
    /// <summary>
    /// called after state is set to InfrontOfFace
    /// </summary>
    public event Action OnReachedInfronOfFacePosition;
    /// <summary>
    /// called after state is set to MovingTowardRestingPosition
    /// </summary>
    public event Action OnStartMovingToRestPosition;
    /// <summary>
    /// called after state is set to MovingInfrontOfFace

    /// </summary>
    public event Action OnStartMovingInfrontOfFace;

    public event Action<IHoldableObject> OnGrabObject;

    public event Action<IHoldableObject> OnDropObject;
    #endregion

    HandState _currentState;
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
                    OnStartMovingToRestPosition?.Invoke();
                    break;
                case HandState.MovingInfrontOfFace:
                    OnStartMovingInfrontOfFace?.Invoke();
                    break;
                case HandState.MovingTowardRestingPosition:
                    OnStartMovingToRestPosition?.Invoke();
                    break;
            }
        } }

    protected IHoldableObject objectInHand;

    /// <summary>
    /// probably somehow with raycast or boxcollider
    /// </summary>
    public abstract void GrabObject(IHoldableObject holdObj);
    public abstract void DropObject();

    public void ForwardInteractableObject(IInteractable interactObj)
    {
        if(objectInHand != null && objectInHand.IsInterestedInInteractableObjects())
        {
            objectInHand.SetInteractedObject(interactObj);
        }
    }

    #region event invocation warappers
    protected void InvokeInRestingPosition()
    {
        InRestingPosition?.Invoke();
    }

    protected void InvokeOnReachedInfrontOfFacePosition()
    {
        OnReachedInfronOfFacePosition?.Invoke();
    }

    protected void InvokeOnStartMovingToRestPosition()
    {
        OnStartMovingToRestPosition?.Invoke();
    }

    protected void InvokeOnStartMovingInfrontOfFace()
    {
        OnStartMovingInfrontOfFace?.Invoke();
    }

    protected void InvokeOnGrabObject()
    {
        OnGrabObject?.Invoke(objectInHand);
    }

    protected void InvokeOnDropObject(IHoldableObject prevHeldObject)
    {
        OnDropObject?.Invoke(prevHeldObject);
    }
    #endregion
}
