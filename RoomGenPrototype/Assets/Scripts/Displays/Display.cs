using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The base class for displaying resources
/// </summary>
public abstract class Display : MonoBehaviour, IInteractable {

    public enum DisplayType
    {
        MeshDisplay,
        ImageDisplay,
    };

    /// <summary>
    /// Type of display
    /// </summary>
    public DisplayType Type { get; protected set; }

    public GameObject Object
    {
        get
        {
            return gameObject;
        }
    }

    /// <summary>
    /// the metadata that further describes the resource held by the display
    /// </summary>
    public MetaData MetaData { get; private set; }

    /// <summary>
    /// System type of the interaction the display provides
    /// </summary>
    protected Type interactionType;

    /// <summary>
    /// the interaction that is currently taking place
    /// </summary>
    Interaction interaction;

    /// <summary>
    /// Applies a resource to a display
    /// </summary>
    /// <param name="obj"></param>
    public virtual void ApplyResource(BaseDisplayResource resource)
    {
        SetInteractionType(resource.InteractionBehaviour);
        //TODO
        MetaData = resource.MetaData;
    }

    public abstract void ApplyPreProcessingInformation(PreProcessingGameObjectInformation info);
    /// <summary>
    /// sets the interaction type of this display
    /// </summary>
    /// <param name="wanted_behaviour">the wanted behaviour</param>
    private void SetInteractionType(string wanted_behaviour)
    {
        Type t = System.Type.GetType(wanted_behaviour);

        if (t == null || !(t.IsSubclassOf(typeof(Interaction))))
        {
            //defaul behaviour it is
            Type def = SetToDefaultInteractionBehaviour();
            if (def == null)
                interactionType = null;
            else
            {
                interactionType = def;
            }
        }
        else
        {
            interactionType = t;
        }
    }

    /// <summary>
    /// Set up the display in the museum, position and rotation
    /// </summary>
    /// <param name="dispInfo"></param>
    /// <param name="parent"></param>
    public abstract void SetUp(MuseumDisplayInfo dispInfo, GameObject parent);

    /// <summary>
    /// called when interaction with this display started
    /// used to maybe disable anything on the display gamobject if needed
    /// </summary>
    protected abstract void InteractionStarted();

    /// <summary>
    /// called when an interaction ends
    /// used to reactivate any component deactivated during the interaction
    /// </summary>
    protected abstract void InteractionEnded();

    /// <summary>
    /// if the wanted behaviour is not possible or not exitent 
    /// the behaviour will be set to this default
    /// </summary>
    protected abstract Type SetToDefaultInteractionBehaviour();

    /// <summary>
    /// called when the player interacts with this display
    /// </summary>
    /// <param name="player"></param>
    public void Interact(Player player)
    {
        player.OnInteractionEnd += OnInteractionEnded;
        if(interactionType != null)
        {
            interaction = ScriptableObject.CreateInstance(interactionType) as Interaction;
            interaction.StartInteraction(player.gameObject, gameObject);
            InteractionStarted();
        }
    }

    /// <summary>
    /// called from event when the interction is ended
    /// </summary>
    /// <param name="arg">event arguments</param>
    public void OnInteractionEnded(PlayerInteractionEventArgs arg)
    {
        if (arg.InteractionType == PlayerInteractionEventArgs.InteractingWith.Display)
        {
            PlayerDisplayInteractionEventArgs dispArgs = arg as PlayerDisplayInteractionEventArgs;

            //check if we are this
            if (dispArgs.DisplayInteractedWith == this && interaction != null)
            {
                interaction.EndInteraction();
            }
            InteractionEnded();
            //remove self from event
            arg.InteractingPlayer.OnInteractionEnd -= OnInteractionEnded;
        }
    }
}
