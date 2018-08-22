using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// base interface for a resource request, usable
/// by both style request and image/mesh requests
/// </summary>
public interface IResourceRequest
{
    /// <summary>
    /// Called when the resource 
    /// starts working, should most likely start a new thread
    /// </summary>
    IEnumerator StartWorkRequest();
    /// <summary>
    /// called when done with request, and it is now time
    /// to apply it to a gameobject
    /// </summary>
    void WhenDone();

    /// <summary>
    /// Indicator if request finished and it can no be applied to gObj
    /// </summary>
    bool IsDone { get; }

    /// <summary>
    /// URL of server to get resource, thread should only need
    /// to append the ResourceLocator at the end of the string
    /// should be supplied by resource loader
    /// </summary>
    string BaseURL { get; set; }
}
