using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the base class for any interaction
/// any class inhereting from this should 
/// </summary>
public abstract class Interaction : ScriptableObject
{
    /// <summary>
    /// Function that should be called on interact
    /// This Function should apply the InteractionComponents
    /// </summary>
    /// <param name="activator"></param>
    /// <param name="interactedUpon"></param>
    public abstract void StartInteraction(GameObject activator, GameObject interactedUpon);

    /// <summary>
    /// function called on end of interaction
    /// should cleanup any components added to any gamobject 
    /// </summary>
    public abstract void EndInteraction();

}

/// <summary>
/// Base Component class for any interaction that should be possible between 
/// two gameobjects
/// </summary>
public abstract class InteractionComponent : MonoBehaviour
{

    /// <summary>
    /// called from the OnDestroy event of the base class
    /// should cleanup after the object if any other components on any of the
    /// interacting object where disabled and reenable them
    /// </summary>
    protected abstract void Destroy();

    /// <summary>
    /// This functions is called after the component was added to the gameobject
    /// it is applied to
    /// </summary>
    /// <param name="activator"></param>
    /// <param name="interactedUpon"></param>
    public abstract void StartInteraction(GameObject activator, GameObject interactedUpon);
    
    private void OnDestroy()
    {
        Destroy();
    }
}
