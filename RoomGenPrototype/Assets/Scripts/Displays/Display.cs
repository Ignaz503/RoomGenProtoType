using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Display : MonoBehaviour, IInteractable {

    public enum DisplayType
    {
        MeshDisplay,
        ImageDisplay,
    };

    public DisplayType Type { get; protected set; }

    public GameObject Object
    {
        get
        {
            return gameObject;
        }
    }

    string metadata = "All Speech is free speech";
    protected Type interactionType;
    Interaction interaction;

    /// <summary>
    /// Applies a resource to a display
    /// </summary>
    /// <param name="obj"></param>
    public virtual void ApplyResource(Resource resource)
    {
        SetInteractionType(resource.InteractionBehaviour);
        //TODO
        //metadata = resource.MetaData.ToString();
        if (GetType() != typeof(CenterMeshDisplay))
            metadata = GetType().ToString();
        else
            metadata = LoremIpsum.IpsumLorem;
    }

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

    public string GetMetadata()
    {
        return metadata;
    }

    public abstract void SetUp(MuseumDisplayInfo dispInfo, GameObject parent);

    protected abstract void InteractionStarted();

    protected abstract void InteractionEnded();

    protected abstract Type SetToDefaultInteractionBehaviour();

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
