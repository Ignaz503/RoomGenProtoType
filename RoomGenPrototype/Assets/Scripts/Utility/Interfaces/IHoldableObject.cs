using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHoldableObject
{
    
    GameObject Object { get; }
    /// <summary>
    /// called when hand reaches infront of face positio 
    /// right after state was set to infront of face
    /// </summary>
    void OnHeldInfrontOfFace();

    /// <summary>
    /// Called when the hand reaches it's rest posiiton
    /// right after the state is set to RestPosition
    /// </summary>
    void OnPutAway();
    /// <summary>
    /// called when hand starts moving towards the infront of face position
    /// right after the state is set to MovingInfrontofFace
    /// </summary>
    void OnStartedMoveInfrontOfFace();

    /// <summary>
    /// called when the hand starts moving away from the face
    /// right after the hand state is set to MovingToRestPosition
    /// </summary>
    void OnStartedMoveAwayFromFace();
    /// <summary>
    /// used to probably parent obj to hand gamobjt
    /// and set local position
    /// called by grab object of the hand
    /// </summary>
    /// <param name="hand"></param>
    void PositionObjectInHand(PlayerHand hand);

    /// <summary>
    /// this functions defines iof the held object gets
    /// information about an interactable object forwarded to him
    /// </summary>
    /// <returns></returns>
    bool IsInterestedInInteractableObjects();

    /// <summary>
    /// function get's called by hand if the held object is interessted in
    /// interacted objects
    /// </summary>
    /// <param name="iObj">the object the player interacted with</param>
    void SetInteractedObject(IInteractable iObj);


    /* maybe
     * bool IsUsable();
     * void Use(); 
     */
}
