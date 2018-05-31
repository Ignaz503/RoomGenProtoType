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
    string Metadata = "All speech is free speech";
    protected Type InteractionBehaviour;

    /// <summary>
    /// Applies a resource to a display
    /// </summary>
    /// <param name="obj"></param>
    public abstract void ApplyResource(Resource resource);

    public virtual void ApplyMetadata(string metadata)
    {
        Metadata = metadata;
        //TEMP
        Metadata = "Metadata pending";
    }

    public string GetMetadata()
    {
        return Metadata;
    }

    public abstract void SetUp(MuseumDisplayInfo dispInfo, GameObject parent);

    protected abstract void SetToDefaultInteractionBehaviour();

    public void SetInteractionBehaviour(string wanted_behaviour)
    {
        Type t = System.Type.GetType("wanted_behaviour");

        if (t == null || !(t.IsSubclassOf(typeof(Component))))
        {
            SetToDefaultInteractionBehaviour();
            return;
        }
        InteractionBehaviour = t;
    }

    public void SetInteractionBehaviour(Type t)
    {
        if(!t.IsSubclassOf(typeof(Component)))
        {
            SetToDefaultInteractionBehaviour();
            return;
        }
        InteractionBehaviour = t;
    }

    virtual public void Interact(Player player)
    {
        player.OnInteractionEnd += OnInteractionEnded;
    }

    virtual public void OnInteractionEnded(PlayerInteractionEventArgs arg)
    {
        throw new System.NotImplementedException();
    }
}
