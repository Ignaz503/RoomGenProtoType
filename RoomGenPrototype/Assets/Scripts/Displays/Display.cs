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
    string metadata = "All Speech is free speech";
    protected BaseInteractionContainer interactionContainer;

    /// <summary>
    /// Applies a resource to a display
    /// </summary>
    /// <param name="obj"></param>
    public virtual void ApplyResource(Resource resource)
    {
        interactionContainer = InteractionFactory.Instance.BuildInteractionContainer(resource.InteractionBehaviour, SetToDefaultInteractionBehaviour);
        //TODO
        //metadata = resource.MetaData.ToString();
        if(GetType() != typeof(CenterMeshDisplay))
            metadata = GetType().ToString();
    }

    public string GetMetadata()
    {
        return metadata;
    }

    public abstract void SetUp(MuseumDisplayInfo dispInfo, GameObject parent);

    protected abstract Type SetToDefaultInteractionBehaviour();

    virtual public void Interact(Player player)
    {
        player.OnInteractionEnd += OnInteractionEnded;
    }

    virtual public void OnInteractionEnded(PlayerInteractionEventArgs arg)
    {
        throw new System.NotImplementedException();
    }
}
