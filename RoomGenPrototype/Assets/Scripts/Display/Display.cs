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

    /// <summary>
    /// Applies a resource to a display
    /// </summary>
    /// <param name="obj"></param>
    public abstract void ApplyResource(UnityEngine.Object obj);
}
