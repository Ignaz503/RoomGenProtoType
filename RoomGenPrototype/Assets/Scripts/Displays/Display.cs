using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Display : MonoBehaviour {

    public enum DisplayType
    {
        MeshDisplay,
        ImageDisplay
    };

    public DisplayType Type { get; protected set; }
    string Metadata = "All speech is free speech";

    /// <summary>
    /// Applies a resource to a display
    /// </summary>
    /// <param name="obj"></param>
    public abstract void ApplyResource(UnityEngine.Object obj);

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

}
